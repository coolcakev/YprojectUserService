using Bogus;
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
    string Password,
    DateTime Birthday,
    string CodeWord,
    SexType Sex
);

public class Handler: IRequestHandler<RegisterUserRequest, Response<EmptyValue>>
{
    private readonly ApplicationDbContext _context;
    private static readonly Faker _faker = new Faker();

    public Handler(ApplicationDbContext context, Faker faker)
    {
        _context = context;
    }
    
    public async Task<Response<EmptyValue>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => 
            x.Email == request.Body.Email,
            cancellationToken);

        if (user != null) return FailureResponses.BadRequest("Login or Email are already register");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Body.Password);
        var hashedCodeWord = BCrypt.Net.BCrypt.HashPassword(request.Body.CodeWord);
        
        var uniqueLogin = GenerateUniqueLogin(2, 8);
        
        user = new User()
        {
            Id = uniqueLogin,
            Email = request.Body.Email,
            Password = hashedPassword,
            Birthday = request.Body.Birthday,
            CodeWord = hashedCodeWord,
            Sex = request.Body.Sex,
            IsEmailVerified = false,
        };

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return SuccessResponses.Ok();
    }

    private string GenerateUniqueLogin(int lettersCount, int digitsCount)
    {
        string letters = _faker.Random.String2(lettersCount, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        string digits = _faker.Random.String2(digitsCount, "0123456789");
        return letters + digits;
    }
}