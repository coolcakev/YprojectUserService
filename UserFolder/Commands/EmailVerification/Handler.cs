using MediatR;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Database;

namespace YprojectUserService.UserFolder.Commands.EmailVerification;

public record UpdateVerifyRequest(string Id) : IHttpRequest<EmptyValue>;

public class Handler: IRequestHandler<UpdateVerifyRequest, Response<EmptyValue>>
{
    private readonly ApplicationDbContext _context;
 
    public Handler(ApplicationDbContext context)
    {
        _context = context;
    }
     
    public async Task<Response<EmptyValue>> Handle(UpdateVerifyRequest request, CancellationToken cancellationToken)
    {
        //TODO краще використовувати по FindAsync, коли шукаєш по id переглянути по всьому проекті
        var user = await _context.Users.FirstOrDefaultAsync(u=>u.Id == request.Id, cancellationToken);

        if (user == null)
        {
            return FailureResponses.BadRequest("User not found");
        }

        user.IsEmailVerified = true;
        await _context.SaveChangesAsync(cancellationToken);
        return SuccessResponses.Ok();
    }
}