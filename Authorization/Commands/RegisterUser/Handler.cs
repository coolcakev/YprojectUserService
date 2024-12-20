using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Database;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.Authorization.Commands.RegisterUser;

public record RegisterUserRequest([FromBody] RegisterUserBody Body) : IHttpRequest<EmptyValue>;

public record RegisterUserBody(
    string Email, 
    string Login, 
    string Password,
    DateTime Birthday,
    string CodeWord,
    SexType Sex
);

public class Handler: IRequestHandler<RegisterUserRequest, Response<EmptyValue>>
{
    private readonly ApplicationDbContext _context;

    public Handler(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Response<EmptyValue>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => 
            x.Login == request.Body.Login 
            || x.Email == request.Body.Email,
            cancellationToken);

        if (user != null) return FailureResponses.BadRequest("Login or Email are already register");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Body.Password);

        user = new User()
        {
            Login = request.Body.Login,
            Email = request.Body.Email,
            Password = hashedPassword,
            Birthday = request.Body.Birthday,
            CodeWord = request.Body.CodeWord,
            Sex = request.Body.Sex
        };

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return SuccessResponses.Ok();
    }
}