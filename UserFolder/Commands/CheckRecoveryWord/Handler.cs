using MediatR;
using Microsoft.AspNetCore.Mvc;
using y_nuget.Auth;
using y_nuget.Endpoints;
using YprojectUserService.Database;
using YprojectUserService.Localization;

namespace YprojectUserService.UserFolder.Commands.CheckRecoveryWord;

public record CheckRecoveryWordRequest([FromBody] string Word) : IHttpRequest<bool>;


public class Handler: IRequestHandler<CheckRecoveryWordRequest, Response<bool>>
{
    private readonly AuthService _authService;
    private readonly ApplicationDbContext _dbContext;

    public Handler(AuthService authService, ApplicationDbContext dbContext)
    {
        _authService = authService;
        _dbContext = dbContext;
    }
    
    public async Task<Response<bool>> Handle(CheckRecoveryWordRequest request, CancellationToken cancellationToken)
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
                LocalizationKeys.Recovery.WordNotFound
                );
        } 
        
        var checkResult = BCrypt.Net.BCrypt.Verify(request.Word, user.CodeWord);
        
        if (!checkResult)
        {
            return FailureResponses.BadRequest<bool>(
                LocalizationKeys.Recovery.ProviderWordIncorrect
            );
        }
        
        return SuccessResponses.Ok(true);
    }
}