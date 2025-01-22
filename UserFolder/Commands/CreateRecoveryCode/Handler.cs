using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using y_nuget.Auth;
using y_nuget.Endpoints;
using y_nuget.RabbitMq;
using YprojectUserService.Database;
using YprojectUserService.Razor;
using YprojectUserService.Razor.Models;
using YprojectUserService.Razor.Templates;

namespace YprojectUserService.UserFolder.Commands.CreateRecoveryCode;

public record CreateRecoveryCodeRequest() : IHttpRequest<EmptyValue>;
public class Handler : IRequestHandler<CreateRecoveryCodeRequest, y_nuget.Endpoints.Response<EmptyValue>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly RazorRenderer _razorRenderer;
    private readonly AuthService _authService;
    private readonly IBus _bus;
    
    public Handler(
        ApplicationDbContext dbContext, 
        RazorRenderer razorRenderer, 
        AuthService authService,
        IBus bus
        )
    {
        _dbContext = dbContext;
        _razorRenderer = razorRenderer;
        _authService = authService;
        _bus = bus;
    }
     
    public async Task<y_nuget.Endpoints.Response<EmptyValue>> Handle(CreateRecoveryCodeRequest request, CancellationToken cancellationToken)
    {
        var currentUser = _authService.GetCurrentUser();

        if (currentUser is null)
        {
            return FailureResponses.NotFound("userNotFound");
        }
        
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(e => e.Id == currentUser.Id, cancellationToken);
        
        if (user == null)
        {
            return FailureResponses.NotFound("userNotFound");
        } 
        
        var random = new Random();
        var code = random.Next(100000, 999999).ToString();
        
        var hashCode = BCrypt.Net.BCrypt.HashPassword(code);
        
        user.RecoveryCode = hashCode;
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var emailModel = new EmailModel
        {
            Title = "Reset your secret",
            Subtitle = "You have requested to reset your personal data. Please use the code below to proceed with the reset process.",
            Code = code
        };
        
        var parameters = new Dictionary<string, object?>
        {
            { "Model", emailModel }
        };

        var html = await _razorRenderer.RenderAsync<EmailTemplate>(parameters);
        
        await _bus.Publish(new EmailMessage(
            To: user.Email,
            Subject: "Recovery Code Request",
            Html: html
        ));
        
        return SuccessResponses.Ok();
    }
}
