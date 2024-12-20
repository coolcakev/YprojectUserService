using y_nuget.Endpoints;

namespace YprojectUserService.Authorization.Commands.Login;

public class Register: IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePost<LoginRequest, string>("auth/login");
    }
}