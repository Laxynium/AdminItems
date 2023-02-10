using AdminItems.Api.AdminItems;
using AdminItems.Api.Colors;
using AdminItems.Api.Framework;
using AdminItems.Api.Identity;
using AdminItems.Migrator;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedServiceMigrator(builder.Configuration, "adminItems");

builder.Services.AddScoped<NpgsqlConnection>(sp =>
{
    var cs = sp.GetRequiredService<IConfiguration>().GetConnectionString("postgres")!;
    return new NpgsqlConnection(cs);
});

builder.Services.AddSingleton<IAdminItemsStore>(sp =>
{
    var cs = sp.GetRequiredService<IConfiguration>().GetConnectionString("postgres")!;
    return new SqlAdminItemsStore(cs);
});
builder.Services.AddSingleton<IColorsStore>(sp =>
{
    var cs = sp.GetRequiredService<IConfiguration>().GetConnectionString("postgres")!;
    return new SqlColorStore(cs);
});
builder.Services.AddSingleton<IAdminItemIdGenerator, IdGenAdminItemIdGenerator>();

builder.Services.AddIdentity(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers(cfg =>
{
    cfg.Filters.Add<OptimisticConcurrencyExceptionFilter>();
});

builder.Services.AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

namespace AdminItems.Api
{
    public class Program
    {
    }
}