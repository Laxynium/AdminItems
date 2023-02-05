using AdminItems.Api.AdminItems;
using AdminItems.Api.Colors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IAdminItemsStore, NullAdminItemsStore>();
builder.Services.AddSingleton<IColorsStore, NullColorsStore>();
builder.Services.AddSingleton<IAdminItemIdGenerator, IdGenAdminItemIdGenerator>();

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