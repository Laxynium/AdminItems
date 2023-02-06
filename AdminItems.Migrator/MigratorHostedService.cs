using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdminItems.Migrator;

public class MigratorHostedService : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;
    private readonly string _sectionName;

    public MigratorHostedService(IConfiguration configuration, ILoggerFactory loggerFactory, string sectionName)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
        _sectionName = sectionName;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var migratorSection = _configuration.GetRequiredSection("migrator")
            .GetRequiredSection(_sectionName);

        var cs = migratorSection.GetRequiredSection("connectionString").Value!;
        var scriptsPath = migratorSection.GetRequiredSection("scriptsPath").Value!;
        
        var migrator = Migrator.Create(cs, scriptsPath, _loggerFactory);
        
        migrator.Migrate();
        
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}