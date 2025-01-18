using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Database;

namespace YprojectUserService.UserFolder.Queries.GetEmailUsed;

public record GetEmailUsedRequest([FromBody] string Email) : IHttpRequest<bool>;
public class Handler: IRequestHandler<GetEmailUsedRequest, Response<bool>>
{
    private readonly ApplicationDbContext _dbContext;

    public Handler(ApplicationDbContext context)
    {
        _dbContext = context;
    }
    
    public async Task<Response<bool>> Handle(GetEmailUsedRequest request, CancellationToken cancellationToken)
    {
        var isUsed = await _dbContext.Users.AnyAsync(
            u => u.Email == request.Email, 
            cancellationToken
        );
        
        return SuccessResponses.Ok(isUsed);
    }
}