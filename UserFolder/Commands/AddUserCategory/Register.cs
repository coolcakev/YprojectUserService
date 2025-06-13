using y_nuget.Endpoints;

namespace YprojectUserService.UserFolder.Commands.AddUserCategory;

public class Register: IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePost<AddUserCategoryRequest, EmptyValue>("user/{id}/category");
    }
}