using y_nuget.Endpoints;

namespace YprojectUserService.UserFolder.Commands.CheckRecoveryWord;

public class Register: IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePost<CheckRecoveryWordRequest, bool>("user/check-recovery-word");
    }
}