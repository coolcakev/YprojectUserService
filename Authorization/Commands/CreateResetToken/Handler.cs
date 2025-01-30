using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Authorization.Services;
using YprojectUserService.Database;
using YprojectUserService.Localization;

namespace YprojectUserService.Authorization.Commands.CreateResetToken;

public record CreateResetTokenRequest([FromBody] string Email) : IHttpRequest<string>;

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
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null)
        {
            return FailureResponses.NotFound<string>(LocalizationKeys.User.NotFound);
        }
        
        var token = _jWtService.GenerateToken(user.Id, user.Email, true, user.AgeGroup, user.Sex);

        return SuccessResponses.Ok(token);
    }
}