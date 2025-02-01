using y_nuget.Endpoints;

namespace YprojectUserService.UserFolder.Commands.SendVerificationEmail;

public class Register : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePost<SendEmailRequest, EmptyValue>("user/send-verification");
    }
}