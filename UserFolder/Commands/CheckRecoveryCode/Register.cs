using y_nuget.Endpoints;

namespace YprojectUserService.UserFolder.Commands.CheckRecoveryCode;

public class Register: IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePost<CheckRecoveryCodeRequest, bool>("user/check-recovery-code");
    }
}