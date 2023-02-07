using DbUp;
using DbUp.Builder;
using DbUp.Engine;
using Microsoft.Extensions.Logging;

namespace AdminItems.Migrator;

public class Migrator
{
    private readonly UpgradeConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    private Migrator(UpgradeConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }

    public static Migrator Create(string connectionString, string scriptsPath, ILoggerFactory loggerFactory)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        var configuration =
            DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsFromFileSystem(scriptsPath)
                .LogToAutodetectedLog()
                .WithExecutionTimeout(TimeSpan.FromSeconds(180))
                .BuildConfiguration();

        return new Migrator(configuration, loggerFactory);
    }

    public void Migrate()
    {
        var engine = new UpgradeEngine(_configuration);
        var logger = _loggerFactory.CreateLogger<Migrator>();

        if (!engine.TryConnect(out var connectionError))
        {
            logger.LogError("There was error while establishing connection using provided cs: {connectionError}", connectionError);
            throw new Exception($"There was error while establishing connection using provided cs: {connectionError}");
        }

        var scripts = engine.GetScriptsToExecute();
        foreach (var sqlScript in scripts)
        {
            logger.LogInformation($"Going to run a script: {sqlScript.Name}");
        }
        
        logger.LogInformation("Performing database upgrade...");
        var result = engine.PerformUpgrade();
        if (!result.Successful)
        {
            logger.LogError("Failed to upgrade database: {error}", result.Error);
            throw new Exception($"Failed to upgrade database: {result.Error}");
        }
        
        logger.LogInformation("Successfully performed database upgrade!");
    }
}