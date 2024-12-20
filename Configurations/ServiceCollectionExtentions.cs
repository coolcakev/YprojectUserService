namespace YprojectUserService.Configurations;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AutoAddScopeByBaseType<T>(
        this IServiceCollection services,
        Action<Type,Type> action
        )
    {
        var baseJobTypes = typeof(T);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseJobTypes.IsAssignableFrom(p)&&baseJobTypes!=p);

        foreach (var type in types)
        {
            action(baseJobTypes, type);
        }

        return services;
    }
    
    public static void AddScopeWithOneParam(this IServiceCollection services,Type baseType, Type type)
    {
        services.AddScoped(type);
    }
    
    public static void AddScopeWithTwoParams(this IServiceCollection services,Type baseType, Type type)
    {
        services.AddScoped(baseType, type);
    }
    
    //registration single and with dependent class or interface
    public static void AddScopeForStrategy(this IServiceCollection services, Type baseType, Type type)
    {
        services.AddScopeWithTwoParams(baseType, type);
        services.AddScopeWithOneParam(baseType, type);
    }
}