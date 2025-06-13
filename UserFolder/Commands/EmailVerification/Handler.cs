using MediatR;
using y_nuget.Endpoints;
using YprojectUserService.Database;
using YprojectUserService.Localization;

namespace YprojectUserService.UserFolder.Commands.EmailVerification;

public record UpdateVerifyRequest(string Id) : IHttpRequest<EmptyValue>;

public class Handler: IRequestHandler<UpdateVerifyRequest, Response<EmptyValue>>
{
    private readonly ApplicationDbContext _context;
 
    public Handler(ApplicationDbContext context)
    {
        _context = context;
    }
     
    public async Task<Response<EmptyValue>> Handle(UpdateVerifyRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(request.Id, cancellationToken);

        if (user == null)
        {
            return FailureResponses.NotFound(LocalizationKeys.User.NotFound);
        }

        user.IsEmailVerified = true;
        await _context.SaveChangesAsync(cancellationToken);
        return SuccessResponses.Ok();
    }
}