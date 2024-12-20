namespace YprojectUserService.Configurations;

public static class CorsConfig
{
    public static string CorsKey="CorsPolicy";
    public static IServiceCollection AddCorsConfig(this IServiceCollection services)
    {
        services.AddCors(x=>x.AddPolicy(CorsKey, policy =>
        {
            policy
                .WithOrigins(
                    "https://localhost:3000", 
                    "http://localhost:3000"
                    )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }));

        return services;
    }
}
