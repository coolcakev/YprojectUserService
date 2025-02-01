using MassTransit;
using MediatR;
using y_nuget.Auth;
using y_nuget.Endpoints;
using y_nuget.RabbitMq;
using YprojectUserService.Localization;
using YprojectUserService.Razor;
using YprojectUserService.Razor.Models;
using YprojectUserService.Razor.Templates;

namespace YprojectUserService.UserFolder.Commands.SendVerificationEmail;

public record SendEmailRequest() : IHttpRequest<EmptyValue>;


public class Handler: IRequestHandler<SendEmailRequest, y_nuget.Endpoints.Response<EmptyValue>>
{
    private readonly IBus _bus;
    private readonly RazorRenderer _razorRenderer;
    private readonly AuthService _authService;
    
    public Handler(RazorRenderer razorRenderer, IBus bus, AuthService authService)
    {
        _bus = bus;
        _razorRenderer = razorRenderer;
        _authService = authService;
    }
    
    public async Task<y_nuget.Endpoints.Response<EmptyValue>> Handle(SendEmailRequest request, CancellationToken cancellationToken)
    {
        var user = _authService.GetCurrentUser();

        if (user is null)
        {
            return FailureResponses.NotFound(LocalizationKeys.User.NotFound);
        }
        
        var emailModel = new EmailModel
        {
            Title = "Thank you for joining the Y Project!",
            Subtitle = "Your account has been successfully created! Verify your email so we can be sure it's you. This is your unique login that is available to all users",
            Code = user.Id,
            EmailButton = new EmailButton()
            {
                Link = "http://example.com",
                Text = "CLICK TO VERIFY"
            }
        };
        
        var parameters = new Dictionary<string, object?>
        {
            { "Model", emailModel }
        };
        
        var html = await _razorRenderer.RenderAsync<EmailTemplate>(parameters);
        
        await _bus.Publish(new EmailMessage(
            To: user.UserEmail,
            Subject: "Hello, it's great that you are with us 🎉",
            Html: html
        ));
        
        return SuccessResponses.Ok();
    }
}