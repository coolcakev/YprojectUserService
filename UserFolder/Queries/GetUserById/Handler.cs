using MediatR;
using y_nuget.Auth;
using y_nuget.Endpoints;
using YprojectUserService.Database;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.UserFolder.Queries.GetUserById;

public record GetUserByIdRequest(string Id) : IHttpRequest<GetUserByIdResponse>;

public record GetUserByIdResponse(
    string Id, 
    string Email,
    string CodeWord,
    DateTime Birthday,
    SexType Sex
);

public class Handler: IRequestHandler<GetUserByIdRequest, Response<GetUserByIdResponse>>
{
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;
    
    public Handler(ApplicationDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }
    
    public async Task<Response<GetUserByIdResponse>> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        //TODO забрати цю змінну
        var currentUser = _authService.GetCurrentUser();

        var user = await _context.Users.FindAsync(request.Id);
        if (user == null) return FailureResponses.NotFound<GetUserByIdResponse>("User not found");

        var userDto = new GetUserByIdResponse(user.Id,user.Email, user.CodeWord, user.Birthday, user.Sex);
        return SuccessResponses.Ok(userDto);
    }
}