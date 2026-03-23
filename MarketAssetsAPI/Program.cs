using MarketAssetsAPI.Data;
using MarketAssetsAPI.Models;
using MarketAssetsAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.Configure<FintachartsSettings>(
    builder.Configuration.GetSection("Fintacharts"));

builder.Services.AddHttpClient<FintachartsAuthService>();
builder.Services.AddSingleton<FintachartsAuthService>();

builder.Services.AddHttpClient<InstrumentService>();
builder.Services.AddScoped<InstrumentService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
