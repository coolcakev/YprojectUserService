using YprojectUserService.Authorization.Services;

namespace YprojectUserService.Authorization;

public static class AuthConfig
{
    public static IServiceCollection AddAuthConfig(this IServiceCollection services)
    {
        services.AddSingleton<JWtService>();
        return services;
    }
}