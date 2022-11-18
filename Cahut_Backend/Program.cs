using Cahut_Backend.Models;
using Cahut_Backend.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();
