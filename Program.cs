using KonkursBot.Configurations;
using KonkursBot.Controllers;
using KonkursBot.Db;
using KonkursBot.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();
app.UseCors(options =>
{
    options.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobHubBot API v1");
    c.RoutePrefix = "swagger";
});

app.MapControllerRoute(
    name: "konkurs",
    pattern: "{controller=Bot}/{action=Post}");
app.MapControllers();

try
{
    // Apply migrations on startup
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    Console.WriteLine($"Migrations applying succesfully completed");
}
catch (Exception ex)
{
    Console.WriteLine($"Error applying migrations: {ex.Message}");
}

app.Run();