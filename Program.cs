using System.Security.Claims;
using YprojectUserService.Configurations;
using YprojectUserService.Database;
using y_nuget;
using y_nuget.Auth;
using y_nuget.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddConfiguration(builder);
builder.Services.AddYNugetConfiguration(builder);

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue(AuthService.HeaderParsedToken, out var claimsJsonValues))
    {
        var claimsJson = claimsJsonValues.ToString(); 

        var dictionary = System.Text.Json.JsonSerializer
            .Deserialize<Dictionary<string, string>>(claimsJson);

        if (dictionary != null)
        {
            var claims = dictionary.Select(kv => new Claim(kv.Key, kv.Value));
            var identity = new ClaimsIdentity(claims, "GatewayAuth");
            context.User = new ClaimsPrincipal(identity);
        }
    }

    await next();
});


app.MapEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.AddAutoMigration();
}

app.UseHttpsRedirection();

app.Run();