using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Database;

namespace YprojectUserService.UserFolder.Commands.CheckRecoveryCode;

public record CheckRecoveryCodeRequest([FromBody] CheckRecoveryCodeBody Body) : IHttpRequest<CheckRecoveryCodeResponse>;

public record CheckRecoveryCodeBody(
    string Code
);

public record CheckRecoveryCodeResponse(bool Success, string Message);

public class Handler : IRequestHandler<CheckRecoveryCodeRequest, y_nuget.Endpoints.Response<CheckRecoveryCodeResponse>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public Handler(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }
     
    public async Task<y_nuget.Endpoints.Response<CheckRecoveryCodeResponse>> Handle(CheckRecoveryCodeRequest request, CancellationToken cancellationToken)
    {
        var userEmailFromToken = _httpContextAccessor.HttpContext?
            .User
            .Claims
            .FirstOrDefault(c => c.Type == "userEmail")?
            .Value;
        
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(e => e.Email == userEmailFromToken, cancellationToken);
        
        if (user == null)
        {
            return FailureResponses.BadRequest<CheckRecoveryCodeResponse>(
                "No recovery code found for this email."
            );
        } 
        
        var checkResult = BCrypt.Net.BCrypt.Verify(request.Body.Code, user.CodeWord);
        
        if (!checkResult)
        {
            return FailureResponses.BadRequest<CheckRecoveryCodeResponse>(
                "The provided recovery code is incorrect."
            );
        }
        
        return SuccessResponses.Ok( new CheckRecoveryCodeResponse(true, "The entered code matches"));
    }
}