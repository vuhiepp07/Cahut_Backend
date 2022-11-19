using Cahut_Backend.Models;
using Cahut_Backend.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Cahut"));
});
builder.Services.AddScoped(p => new SiteProvider(builder.Configuration, p.GetRequiredService<AppDbContext>()));

var app = builder.Build();

app.MapControllers();
app.Run();
