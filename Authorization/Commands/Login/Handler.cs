using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Authorization.Services;
using YprojectUserService.Database;

namespace YprojectUserService.Authorization.Commands.Login;

public record LoginRequest([FromBody] LoginBody Body) : IHttpRequest<string>;

public record LoginBody(string Login, string Password);

public class Handler: IRequestHandler<LoginRequest, Response<string>>
{
    private readonly ApplicationDbContext _context;
    private readonly JWtService _jWtService;

    public Handler(ApplicationDbContext context, JWtService jWtService)
    {
        _context = context;
        _jWtService = jWtService;
    }
    
    public async Task<Response<string>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == request.Body.Login || x.Email == request.Body.Login, cancellationToken);
        //TODO винести локалізацію в окремий клас переедивитися по всьому проекті
        if (user == null) return FailureResponses.NotFound<string>("userNotFound");

        var checkPass = BCrypt.Net.BCrypt.Verify(request.Body.Password, user.Password);

        if (!checkPass) return FailureResponses.NotFound<string>("invalidPassword");
        
        var token = _jWtService.GenerateToken(user.Id, user.Email, false);

        return SuccessResponses.Ok(token);
    }
}