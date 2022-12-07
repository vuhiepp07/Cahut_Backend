using Cahut_Backend;
using Cahut_Backend.Models;
using Cahut_Backend.Repository;
using Cahut_Backend.SignalR.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

//Khoa bi mat
byte[] key = Encoding.ASCII.GetBytes("asdqwuida42381dasdasd");
builder.Services.AddAuthentication(p =>
{
    p.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    p.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(p =>
{
    p.RequireHttpsMetadata = false;
    p.SaveToken = true;
    p.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    //build.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    //build.WithOrigins("https://cahut2.netlify.app/").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    //build.WithOrigins($"{Helper.TestingLink}").AllowAnyHeader().AllowAnyMethod();

    build.SetIsOriginAllowed(isOriginAllowed: _ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
}));


builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors("corspolicy");
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<SlideHub>("/slideHub");
app.Run();