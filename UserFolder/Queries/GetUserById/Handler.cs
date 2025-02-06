using MediatR;
using y_nuget.Endpoints;
using YprojectUserService.Database;
using YprojectUserService.Localization;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.UserFolder.Queries.GetUserById;

public record GetUserByIdRequest(string Id) : IHttpRequest<GetUserByIdResponse>;

public record GetUserByIdResponse(
    string Id,
    string Email, 
    bool IsEmailVerified,
    DateTime Birthday,
    AgeGroup AgeGroup,
    SexType Sex,
    string CountryISO,
    string StateISO,
    int CityId
);

public class Handler: IRequestHandler<GetUserByIdRequest, Response<GetUserByIdResponse>>
{
    private readonly ApplicationDbContext _context;
    
    public Handler(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Response<GetUserByIdResponse>> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(request.Id);
        if (user == null) return FailureResponses.NotFound<GetUserByIdResponse>(LocalizationKeys.User.NotFound);

        var userDto = new GetUserByIdResponse(
            user.Id,
            user.Email,
            user.IsEmailVerified,
            user.Birthday,
            user.AgeGroup,
            user.Sex,
            user.CountryISO,
            user.StateISO,
            user.CityId
        );
        
        return SuccessResponses.Ok(userDto);
    }
}