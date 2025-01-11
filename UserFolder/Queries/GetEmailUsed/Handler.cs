using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Database;

namespace YprojectUserService.UserFolder.Queries.GetEmailUsed;

//TODO тут нема сенсу приймати цілий обєкт і відправляти обєкт
public record GetEmailUsedRequest([FromBody] GetEmailUsedBody Body) : IHttpRequest<GetEmailUsedResponse>;
public record GetEmailUsedResponse(
    bool IsUsed
);

public record GetEmailUsedBody(string Email);
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
            u => u.Email == request.Body.Email, 
            cancellationToken
        );
        
        return SuccessResponses.Ok(new GetEmailUsedResponse(isUsed));
    }
}