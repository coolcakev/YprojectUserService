using y_nuget.Endpoints;

namespace YprojectUserService.Authorization.Commands.RegisterUser;

public class Register: IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePost<RegisterUserRequest, string>("auth/registration");
    }
}