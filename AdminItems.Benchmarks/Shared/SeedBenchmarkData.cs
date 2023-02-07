using Bogus;
using Dapper;
using IdGen;
using Npgsql;

namespace AdminItems.Benchmarks.Shared;

public static class SeedBenchmarkData
{
    public static async Task SeedData(int count, PostgresDatabase postgresDatabase)
    {
        if (!await RequiresSeeding(count, postgresDatabase.ConnectionString))
        {
            return;
        }

        await postgresDatabase.CleanUp();
        
        var colorsToPick = new[] { "red", "blue", "green", "orange", "purple", "aqua", "black", "yellow", "white" };
        var f = new Faker("en");
        var codes = Enumerable.Range(1, count)
            .Select(_ => f.Random.String2(3, 12, "abcdefghijklmnopqrstuvwxyz0123456789-_$@+"))
            .ToHashSet()
            .ToList();

        var names = Enumerable.Range(1, count)
            .Select(_ => f.Random.String2(20, 200, "abcdefghijklmnopqrstuvwxyz0123456789-_$@+ "))
            .ToHashSet()
            .ToList();

        var idGen = new IdGenerator(5);

        Console.WriteLine("Coping data to table...");
        await using var connection = new NpgsqlConnection(postgresDatabase.ConnectionString);
        await connection.OpenAsync();
        await using var writer =
            connection.BeginTextImport("COPY admin_items (id, code, name, color, comments) FROM STDIN (Delimiter ',')");
        foreach (var i in Enumerable.Range(0, Math.Min(codes.Count, names.Count)))
        {
            var id = idGen.CreateId();
            var code = codes[i];
            var name = names[i];
            var color = f.PickRandom(colorsToPick);

            await writer.WriteLineAsync($"{id},{code},{name},{color}, ");
            
            if (i % 100000 == 0)
            {
                Console.WriteLine($"Processed already {i+1} records");
            }
        }
    }

    private static async Task<bool> RequiresSeeding(int size, string connectionString)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var total = await connection.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM admin_items");
        return (double)size / total < 0.5;
    }
}