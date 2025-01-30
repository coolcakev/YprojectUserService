using Bogus;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using y_nuget.RabbitMq;
using YprojectUserService.Authorization.Services;
using YprojectUserService.Database;
using YprojectUserService.Localization;
using YprojectUserService.Razor;
using YprojectUserService.Razor.Models;
using YprojectUserService.Razor.Templates;
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
    private readonly RazorRenderer _razorRenderer;
    private readonly ApplicationDbContext _context;
    private readonly IBus _bus;

    private static readonly Faker Faker = new Faker();

    public Handler(
        ApplicationDbContext context, 
        RazorRenderer razorRenderer, 
        JWtService jWtService,
        IBus bus
        )
    {
        _context = context;
        _jWtService = jWtService;
        _razorRenderer = razorRenderer;
        _bus = bus;
    }
    
    public async Task<y_nuget.Endpoints.Response<string>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => 
            x.Email == request.Body.Email,
            cancellationToken);
        
        if (user != null) return FailureResponses.BadRequest<string>(LocalizationKeys.User.LoginEmailRegistered);
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Body.Password);
        var hashedCodeWord = BCrypt.Net.BCrypt.HashPassword(request.Body.CodeWord);
        
        var uniqueLogin = GenerateUniqueLogin(2, 8);
        
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
        
        // var emailModel = new EmailModel
        // {
        //     Title = "Verify your account",
        //     Subtitle = "Your account has been successfully created!  Verify your email so we can be sure it's you. This is your unique login that is available to all users",
        //     Code = uniqueLogin,
        //     EmailButton = new EmailButton()
        //     {
        //         Link = "http://example.com",
        //         Text = "CLICK TO VERIFY"
        //     }
        // };
        //
        // var parameters = new Dictionary<string, object?>
        // {
        //     { "Model", emailModel }
        // };
        //
        // var html = await _razorRenderer.RenderAsync<EmailTemplate>(parameters);
        //
        // await _bus.Publish(new EmailMessage(
        //     To: request.Body.Email,
        //     Subject: "Recovery Code Request",
        //     Html: html
        // ));
        
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        var token = _jWtService.GenerateToken(user.Id, user.Email, false, user.AgeGroup, user.Sex);
        
        return SuccessResponses.Ok(token);
    }

    private string GenerateUniqueLogin(int lettersCount, int digitsCount)
    {
        string letters = Faker.Random.String2(lettersCount, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        string digits = Faker.Random.String2(digitsCount, "0123456789");
        return letters + digits;
    }
}