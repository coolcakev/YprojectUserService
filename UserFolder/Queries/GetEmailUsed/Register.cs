using y_nuget.Endpoints;

namespace YprojectUserService.UserFolder.Queries.GetEmailUsed;

public class Register: IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePost<GetEmailUsedRequest, GetEmailUsedResponse>("user/email-is-used");
    }
}