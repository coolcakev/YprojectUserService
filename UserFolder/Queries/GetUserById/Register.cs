using y_nuget.Endpoints;

namespace YprojectUserService.UserFolder.Queries.GetUserById;

public class Register: IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediateGet<GetUserByIdRequest, GetUserByIdResponse>("user/{id}");
    }
}