using y_nuget.Endpoints;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.UserFolder.Commands.UpdateUser;

public class Register : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePatch<UpdateUserRequest, User, string>("user/update");
    }
}

