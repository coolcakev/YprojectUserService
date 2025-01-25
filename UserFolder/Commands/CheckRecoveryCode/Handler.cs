using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Auth;
using y_nuget.Endpoints;
using YprojectUserService.Database;

namespace YprojectUserService.UserFolder.Commands.CheckRecoveryCode;

public record CheckRecoveryCodeRequest([FromBody] string Code) : IHttpRequest<bool>;

public class Handler : IRequestHandler<CheckRecoveryCodeRequest, Response<bool>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly AuthService _authService;
    public Handler(ApplicationDbContext dbContext,  AuthService authService)
    {
        _authService = authService;
        _dbContext = dbContext;
    }
     
    public async Task<Response<bool>> Handle(CheckRecoveryCodeRequest request, CancellationToken cancellationToken)
    {
        var currentUser = _authService.GetCurrentUser();

        if (currentUser is null)
        {
            return FailureResponses.NotFound<bool>("userNotFound");
        }
        
        //TODO можна вокристати FindASync
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(e => e.Id == currentUser.Id, cancellationToken);
        
        if (user == null)
        {
            return FailureResponses.BadRequest<bool>(
                "recoveryCodeNotFound"
            );
        } 
        
        var checkResult = BCrypt.Net.BCrypt.Verify(request.Code, user.RecoveryCode);
        
        if (!checkResult)
        {
            return FailureResponses.BadRequest<bool>(
                "recoveryCodeProviderIncorrect"
            );
        }
        
        return SuccessResponses.Ok(true);
    }
}