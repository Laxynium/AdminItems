namespace AdminItems.Api.Identity;

public static class Extensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<PasswordHasher>();
        services.AddScoped<UsersStore>();
        if (configuration.GetSection("identity:seedUsers").Get<bool>())
        {
            services.AddHostedService<SeedUsersHostService>();
        }
        return services;
    }
}