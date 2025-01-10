using y_nuget.Endpoints;

namespace YprojectUserService.UserFolder.Commands.EmailVerification;

public class Register : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePut<UpdateVerifyRequest, EmptyValue>("user/verify");
    }
}