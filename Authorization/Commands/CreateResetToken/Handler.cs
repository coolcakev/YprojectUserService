using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Authorization.Services;
using YprojectUserService.Database;

namespace YprojectUserService.Authorization.Commands.CreateResetToken;

public record CreateResetTokenRequest([FromBody] CreateResetTokenBody Body) : IHttpRequest<string>;
public record CreateResetTokenBody(string Email);

public class Handler: IRequestHandler<CreateResetTokenRequest, Response<string>>
{
    private readonly ApplicationDbContext _context;
    private readonly JWtService _jWtService;
    
    public Handler(ApplicationDbContext context, JWtService jWtService)
    {
        _jWtService = jWtService;
        _context = context;
    }
    
    public async Task<Response<string>> Handle(CreateResetTokenRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Body.Email);

        if (user is null)
        {
            return FailureResponses.BadRequest<string>("User not found");
        }
        
        var token = _jWtService.GenerateToken(user.Id, user.Email, true);

        return SuccessResponses.Ok(token);
    }
}