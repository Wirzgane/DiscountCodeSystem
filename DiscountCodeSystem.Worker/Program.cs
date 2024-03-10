using DiscountCodeSystem.Worker;
using DiscountCodeSystem.Worker.Infrastructure;
using DiscountCodeSystem.Worker.Services;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Register the DiscountCodeDbContext service
builder.Services.AddDbContext<DiscountCodeDbContext>(options =>
{
    string dbPath = "Data Source=DiscountCodeDatabase.db"; // Relative path to application folder
    options.UseSqlite(dbPath);
});

builder.Services.AddSingleton<DiscountCodeGenerator>();
builder.Services.AddSingleton<DiscountCodeManager>();
builder.Services.AddSingleton<TCPServer>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
