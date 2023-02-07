using AdminItems.Api.AdminItems;
using AdminItems.Api.Colors;
using AdminItems.Migrator;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSingleton<IColorsStore, NullColorsStore>();
builder.Services.AddSingleton<IAdminItemIdGenerator, IdGenAdminItemIdGenerator>();

builder.Services.AddHostedServiceMigrator(builder.Configuration, "adminItems");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

namespace AdminItems.Api
{
    public class Program
    {
    }
}