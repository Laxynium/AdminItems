using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AdminItems.Migrator;

public static class Extensions
{
    public static IServiceCollection AddHostedServiceMigrator(this IServiceCollection services, IConfiguration configuration, string sectionName)
    {
        var section = configuration.GetRequiredSection("migrator");
        if (!section.GetRequiredSection("enabled").Get<bool>())
        {
            return services;
        }
        
        services.AddHostedService<MigratorHostedService>(sp =>
            new MigratorHostedService(
                sp.GetRequiredService<IConfiguration>(), 
                sp.GetRequiredService<ILoggerFactory>(),
                sectionName));
        return services;
    }
}