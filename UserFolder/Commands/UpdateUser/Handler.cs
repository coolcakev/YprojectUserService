using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Auth;
using y_nuget.Endpoints;
using YprojectUserService.Database;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.UserFolder.Commands.UpdateUser;

public class UpdateUserRequest : GenericPatchRequest<User, string>, IHttpRequest<string>
{
}

public class UpdateUserBody
{
    public UpdateUserBody(string password, DateTime birthday, string codeWord, SexType sex, string countryISO, string stateISO, int cityId)
    {
    }
}

public class Handler : IRequestHandler<UpdateUserRequest, Response<string>>
{
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;
    public Handler(ApplicationDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<Response<string>> Handle(
        UpdateUserRequest request,
        CancellationToken cancellationToken
    )
    {
        //var userEmailFromToken = _httpContextAccessor
        //    .HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "userEmail")
        //    ?.Value;
        //var user = await _context.Users.FirstOrDefaultAsync(
        //    x => x.Email == userEmailFromToken,
        //    cancellationToken
        //);
        var userClaims = _authService.GetCurrentUser();
        if (userClaims?.Id == null)
            return FailureResponses.NotFound<string>("Claims not found");
        
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userClaims.Id, cancellationToken);
        if (user == null)
            return FailureResponses.NotFound<string>("User not found");


        request.Patches.ApplyTo(user);

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.CodeWord = BCrypt.Net.BCrypt.HashPassword(user.CodeWord);

        await _context.SaveChangesAsync(cancellationToken);
        return SuccessResponses.Ok(user.Id);
    }
}
