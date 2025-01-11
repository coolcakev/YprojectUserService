using Bogus;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using y_nuget.Endpoints;
using y_nuget.RabbitMq;
using YprojectUserService.Database;
using YprojectUserService.Razor;
using YprojectUserService.Razor.Models;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.Authorization.Commands.RegisterUser;

public record RegisterUserRequest([FromBody] RegisterUserBody Body) : IHttpRequest<EmptyValue>;

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

public class Handler: IRequestHandler<RegisterUserRequest, y_nuget.Endpoints.Response<EmptyValue>>
{
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly RazorRenderer _razorRenderer;

    private static readonly Faker Faker = new Faker();

    public Handler(ApplicationDbContext context, IPublishEndpoint publishEndpoint, RazorRenderer razorRenderer)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _razorRenderer = razorRenderer;
    }
    
    public async Task<y_nuget.Endpoints.Response<EmptyValue>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => 
            x.Email == request.Body.Email,
            cancellationToken);
        
        //TODO тут треба подумати, бо у нас додаток локалізований переглянути по всьому проекті
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
            CountryISO = request.Body.CountryISO,
            StateISO = request.Body.StateISO,
            CityId = request.Body.CityId
        };
        
        var emailModel = new EmailModel
        {
            Title = "Verify your account",
            Subtitle = "Your account has been successfully created!  Verify your email so we can be sure it's you. This is your unique login that is available to all users",
            Code = uniqueLogin,
            EmailButton = new EmailButton()
            {
                Link = "http://example.com",
                Text = "CLICK TO VERIFY"
            }
        };
        
        var html = await _razorRenderer.RenderAsync("EmailTemplate.cshtml", emailModel);
        
        await _publishEndpoint.Publish(new EmailMessage(
            To: request.Body.Email,
            Subject: "Recovery Code Request",
            Html: html
        ));
        
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        //TODO тут потрібно буде вертати токен, бо користувач вже авторизувався після успішної реєстрації
        return SuccessResponses.Ok();
    }

    private string GenerateUniqueLogin(int lettersCount, int digitsCount)
    {
        string letters = Faker.Random.String2(lettersCount, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        string digits = Faker.Random.String2(digitsCount, "0123456789");
        return letters + digits;
    }
}