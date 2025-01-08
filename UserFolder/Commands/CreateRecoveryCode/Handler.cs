using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Database;
using YprojectUserService.Razor;
using YprojectUserService.Razor.Models;

namespace YprojectUserService.UserFolder.Commands.CreateRecoveryCode;

public record CreateRecoveryCodeRequest([FromBody] CreateRecoveryCodeBody Body) : IHttpRequest<EmptyValue>;

public record CreateRecoveryCodeBody(
    string Email
);

public record RecoveryCodeMessage(string To, string Subject, string Html);

public class Handler : IRequestHandler<CreateRecoveryCodeRequest, y_nuget.Endpoints.Response<EmptyValue>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly RazorRenderer _razorRenderer; // Додано RazorRenderer

    public Handler(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint, RazorRenderer razorRenderer)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _razorRenderer = razorRenderer; // Ініціалізація RazorRenderer
    }
     
    public async Task<y_nuget.Endpoints.Response<EmptyValue>> Handle(CreateRecoveryCodeRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(e => e.Email == request.Body.Email, cancellationToken);
        
        if (user == null)
        {
            return FailureResponses.BadRequest<EmptyValue>(
                "User not found."
            );
        } 
        
        var random = new Random();
        var code = random.Next(100000, 999999).ToString();

        var hashCode = BCrypt.Net.BCrypt.HashPassword(code);

        user.CodeWord = hashCode;
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        // Створюємо модель для шаблону
        var emailModel = new EmailModel
        {
            Title = "Reset your secret",
            Subtitle = "You have requested to reset your personal data. Please use the code below to proceed with the reset process.",
            Code = code
        };

        // Генеруємо HTML з шаблону
        var html = await _razorRenderer.RenderAsync("EmailTemplate.cshtml", emailModel);

        // Відправляємо повідомлення
        await _publishEndpoint.Publish(new RecoveryCodeMessage(
            To: request.Body.Email,
            Subject: "Recovery Code Request",
            Html: html
        ));
        
        return SuccessResponses.Ok();
    }
}
