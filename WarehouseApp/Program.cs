using Microsoft.AspNetCore.Builder;                    
using Microsoft.AspNetCore.Hosting;                                           
using Microsoft.EntityFrameworkCore;                   
using Microsoft.Extensions.Configuration;              
using Microsoft.Extensions.DependencyInjection;        
using Microsoft.Extensions.Hosting;                    
using Microsoft.Extensions.Logging;                               
using WarehouseApp.Data;
using WarehouseApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();                     // MVC контроллеры (опционально)
builder.Services.AddRazorPages();                      // Для Razor Pages
builder.Services.AddDbContext<WarehouseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=warehouse.db"));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<ICsvLoaderService, CsvLoaderService>();
builder.Services.AddHostedService<CsvLoaderService>(); // Автозагрузка CSV при старте

builder.Logging.AddConsole();  // Логи в консоль для отладки

var app = builder.Build();

// пайплайн
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();                                  // Для API
app.MapRazorPages();                                   // Для Razor Pages

app.Run();