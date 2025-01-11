using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using y_nuget.RabbitMq;
using YprojectUserService.Database;
using YprojectUserService.Razor;
using YprojectUserService.Razor.Models;

namespace YprojectUserService.UserFolder.Commands.CreateRecoveryCode;

public record CreateRecoveryCodeRequest() : IHttpRequest<EmptyValue>;
public class Handler : IRequestHandler<CreateRecoveryCodeRequest, y_nuget.Endpoints.Response<EmptyValue>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly RazorRenderer _razorRenderer;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Handler(
        ApplicationDbContext dbContext, 
        IPublishEndpoint publishEndpoint, 
        RazorRenderer razorRenderer, 
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _razorRenderer = razorRenderer;
        _httpContextAccessor = httpContextAccessor;
    }
     
    public async Task<y_nuget.Endpoints.Response<EmptyValue>> Handle(CreateRecoveryCodeRequest request, CancellationToken cancellationToken)
    {
        //TODO це має тягнутися з AuthService перегялнути по всьому проекті
        var userEmailFromToken = _httpContextAccessor.HttpContext?
            .User
            .Claims
            .FirstOrDefault(c => c.Type == "userEmail")?
            .Value;

        if (string.IsNullOrEmpty(userEmailFromToken))
        {
            return FailureResponses.BadRequest("Token has not email data");
        }
        //TODO тут краще шукати по id користувача, а не по email

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(e => e.Email == userEmailFromToken, cancellationToken);
        
        if (user == null)
        {
            return FailureResponses.BadRequest<EmptyValue>(
                "User not found."
            );
        } 
        
        var random = new Random();
        var code = random.Next(100000, 999999).ToString();
        
        var hashCode = BCrypt.Net.BCrypt.HashPassword(code);
        
        //TODO чому ми тут сетаємо згенерований код в CodeWord, а не в recoveryCode
        user.CodeWord = hashCode;
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var emailModel = new EmailModel
        {
            Title = "Reset your secret",
            Subtitle = "You have requested to reset your personal data. Please use the code below to proceed with the reset process.",
            Code = code
        };

        var html = await _razorRenderer.RenderAsync("EmailTemplate.cshtml", emailModel);

        await _publishEndpoint.Publish(new EmailMessage(
            To: userEmailFromToken,
            Subject: "Recovery Code Request",
            Html: html
        ));
        
        return SuccessResponses.Ok();
    }
}
