using MediatR;
using Microsoft.AspNetCore.Mvc;
using y_nuget.Auth;
using y_nuget.Endpoints;
using YprojectUserService.Database;
using YprojectUserService.Localization;

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
            return FailureResponses.NotFound<bool>(LocalizationKeys.User.NotFound);
        }
        
        var user = await _dbContext.Users.FindAsync(currentUser.Id, cancellationToken);
        
        if (user == null)
        {
            return FailureResponses.BadRequest<bool>(
                LocalizationKeys.Recovery.CodeNotFound
            );
        } 
        
        var checkResult = BCrypt.Net.BCrypt.Verify(request.Code, user.RecoveryCode);
        
        if (!checkResult)
        {
            return FailureResponses.BadRequest<bool>(
                LocalizationKeys.Recovery.ProviderIncorrect
            );
        }
        
        return SuccessResponses.Ok(true);
    }
}