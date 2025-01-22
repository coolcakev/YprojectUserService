using y_nuget.Endpoints;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.UserFolder.Commands.PatchUser;

public class Register : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePatch<PatchUserRequest, User, string>("user/patch");
    }
}

