using MediatR;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Database;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.UserFolder.Commands.PatchUser;

public class PatchUserRequest : GenericPatchRequest<User, string>, IHttpRequest<string>
{
}

public class PatchUserBody
{
    public PatchUserBody(string password, DateTime birthday, string codeWord, SexType sex, string countryISO, string stateISO, int cityId)
    {
    }
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
            return FailureResponses.NotFound<string>("userNotFound");

        var testOperations = request.Patches.Operations
            .Where(op => op.OperationType == OperationType.Test)
            .ToList();

        foreach (var testOp in testOperations)
        {
            if (testOp.path.Equals("/password", StringComparison.OrdinalIgnoreCase))
            {
                var plaintextPassword = testOp.value?.ToString();
                if (!BCrypt.Net.BCrypt.Verify(plaintextPassword, user.Password))
                    return FailureResponses.BadRequest<string>("incorrectData");
            }
            else if (testOp.path.Equals("/codeword", StringComparison.OrdinalIgnoreCase))
            {
                var plaintextCodeWord = testOp.value?.ToString();
                if (!BCrypt.Net.BCrypt.Verify(plaintextCodeWord, user.CodeWord))
                    return FailureResponses.BadRequest<string>("incorrectData");
            }

            request.Patches.Operations.Remove(testOp);
        }

        request.Patches.ApplyTo(user);

        if (request.Patches.Operations.Any(op => op.path.Equals("/password", StringComparison.OrdinalIgnoreCase)))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        }
        if (request.Patches.Operations.Any(op => op.path.Equals("/codeword", StringComparison.OrdinalIgnoreCase)))
        {
            user.CodeWord = BCrypt.Net.BCrypt.HashPassword(user.CodeWord);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return SuccessResponses.Ok("Success");
    }
}
