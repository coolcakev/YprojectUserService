using Bogus;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using YprojectUserService.Authorization.Services;
using YprojectUserService.Database;
using YprojectUserService.Localization;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.Authorization.Commands.RegisterUser;

public record RegisterUserRequest([FromBody] RegisterUserBody Body) : IHttpRequest<string>;

public record RegisterUserBody(
    string Email, 
    string Password,
    DateTime Birthday,
    string CodeWord,
    SexType Sex,
    string CountryISO,
    string StateISO,
    int CityId
);

public class Handler: IRequestHandler<RegisterUserRequest, y_nuget.Endpoints.Response<string>>
{
    private readonly JWtService _jWtService;
    private readonly ApplicationDbContext _context;
    
    private static readonly Faker Faker = new Faker();

    public Handler(
        ApplicationDbContext context, 
        JWtService jWtService
        )
    {
        _context = context;
        _jWtService = jWtService;
    }
    
    public async Task<Response<string>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => 
            x.Email == request.Body.Email,
            cancellationToken);
        
        if (user != null) return FailureResponses.BadRequest<string>(LocalizationKeys.User.LoginEmailRegistered);
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Body.Password);
        var hashedCodeWord = BCrypt.Net.BCrypt.HashPassword(request.Body.CodeWord);
        
        var uniqueLogin = GenerateUniqueLogin(2, 8);
        
        //TODO винести всі обрахунки з віком і сетання вікової групи в метод в класі User
        var today = DateTime.Today;
        int age = today.Year - request.Body.Birthday.Year;
        if (request.Body.Birthday.Date > today.AddYears(-age))
        {
            age--;
        }
        
        var userAgeGroup = age switch
        {
            >= 14 and <= 17 => AgeGroup.Teenagers,
            >= 18 and <= 24 => AgeGroup.Young,
            >= 25 and <= 34 => AgeGroup.ActiveDevelopment,
            >= 35 and <= 44 => AgeGroup.AdultLife,
            >= 45 and <= 54 => AgeGroup.LateMaturity,
            _ => AgeGroup.ActiveLongevity
        };
        
        user = new User()
        {
            Id = uniqueLogin,
            Email = request.Body.Email,
            Password = hashedPassword,
            Birthday = request.Body.Birthday,
            AgeGroup = userAgeGroup,
            CodeWord = hashedCodeWord,
            Sex = request.Body.Sex,
            IsEmailVerified = false,
            CountryISO = request.Body.CountryISO,
            StateISO = request.Body.StateISO,
            CityId = request.Body.CityId,
        };
        
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        var token = _jWtService.GenerateToken(user.Id, user.Email, true, user.AgeGroup, user.Sex);
        
        return SuccessResponses.Ok(token);
    }

    private string GenerateUniqueLogin(int lettersCount, int digitsCount)
    {
        string letters = Faker.Random.String2(lettersCount, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        string digits = Faker.Random.String2(digitsCount, "0123456789");
        return letters + digits;
    }
}