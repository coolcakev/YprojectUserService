using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Database;

namespace YprojectUserService.UserFolder.Queries.GetEmailUsed;

public record GetEmailUsedRequest([FromQuery] string Email) : IHttpRequest<GetEmailUsedResponse>;
public record GetEmailUsedResponse(
    bool IsUsed
);

public class Handler: IRequestHandler<GetEmailUsedRequest, Response<GetEmailUsedResponse>>
{
    private readonly ApplicationDbContext _dbContext;

    public Handler(ApplicationDbContext context)
    {
        _dbContext = context;
    }
    
    public async Task<Response<GetEmailUsedResponse>> Handle(GetEmailUsedRequest request, CancellationToken cancellationToken)
    {
        var isUsed = await _dbContext.Users.AnyAsync(
            u => u.Email == request.Email, 
            cancellationToken
        );
        
        return SuccessResponses.Ok(new GetEmailUsedResponse(isUsed));
    }
}