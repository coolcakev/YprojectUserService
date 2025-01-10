using y_nuget.Endpoints;

namespace YprojectUserService.UserFolder.Commands.CheckRecoveryCode;

public class Register: IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePost<CheckRecoveryCodeRequest, CheckRecoveryCodeResponse>("user/check-recovery-code");
    }
}