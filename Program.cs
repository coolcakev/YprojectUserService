using YprojectUserService.Configurations;
using YprojectUserService.Database;
using y_nuget;
using y_nuget.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddConfiguration(builder);
builder.Services.AddYNugetConfiguration(builder);

var app = builder.Build();

app.MapEndpoints();
app.UseCors(CorsConfig.CorsKey);

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