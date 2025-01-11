using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Database;

namespace YprojectUserService.UserFolder.Commands.CheckRecoveryCode;

//TODO не потрібно цілого обєкта, а тільки змінну string з боді тягнути
public record CheckRecoveryCodeRequest([FromBody] CheckRecoveryCodeBody Body) : IHttpRequest<CheckRecoveryCodeResponse>;

public record CheckRecoveryCodeBody(
    string Code
);
//TODO тут має вертатися тільки бул, а не обєкт
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
        //TODO це має тягнутися з AuthService
        var userEmailFromToken = _httpContextAccessor.HttpContext?
            .User
            .Claims
            .FirstOrDefault(c => c.Type == "userEmail")?
            .Value;
        
            //TODO тут краще шукати по id користувача, а не по email
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
        
        //TODO це повідомлення безполезне
        return SuccessResponses.Ok( new CheckRecoveryCodeResponse(true, "The entered code matches"));
    }
}