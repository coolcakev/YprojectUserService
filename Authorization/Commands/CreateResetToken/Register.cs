using y_nuget.Endpoints;

namespace YprojectUserService.Authorization.Commands.CreateResetToken;

public class Register: IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePost<CreateResetTokenRequest, string>("/user/create-reset-token");
    }
}