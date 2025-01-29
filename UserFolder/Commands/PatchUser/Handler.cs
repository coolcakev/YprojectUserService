using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Database;
using YprojectUserService.Localization;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.UserFolder.Commands.PatchUser;

public class PatchUserRequest : GenericPatchRequest<User, string>, IHttpRequest<string>
{
}

public class Handler : IRequestHandler<PatchUserRequest, Response<string>>
{
    private readonly ApplicationDbContext _context;

    public Handler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
    }

    public async Task<Response<string>> Handle(
        PatchUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (user == null)
            return FailureResponses.NotFound<string>(LocalizationKeys.User.NotFound);

        request.Patches.ApplyTo(user);

        request.Patches.ApplyIfOperationExists("/password", () =>
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        });

        request.Patches.ApplyIfOperationExists("/codeword", () =>
        {
            user.CodeWord = BCrypt.Net.BCrypt.HashPassword(user.CodeWord);
        });


        await _context.SaveChangesAsync(cancellationToken);
        return SuccessResponses.Ok("Success");
    }
}

public static class JsonPatchExtensions
{
    public static void ApplyIfOperationExists(this JsonPatchDocument<User> patches, string path, Action action)
    {
        if (patches.Operations.Any(op => op.path.Equals(path, StringComparison.OrdinalIgnoreCase)))
        {
            action();
        }
    }
}