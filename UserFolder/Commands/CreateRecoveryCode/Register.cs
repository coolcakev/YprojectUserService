using y_nuget.Endpoints;

namespace YprojectUserService.UserFolder.Commands.CreateRecoveryCode;

public class Register: IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder app)
    {
        app.MediatePost<CreateRecoveryCodeRequest, EmptyValue>("email/create-recovery-code");
    }
}