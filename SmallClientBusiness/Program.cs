using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmallClientBusiness.BL;
using SmallClientBusiness.Common.System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Database
builder.ConfigureAppDb();
builder.ConfigureIdentity();

//Services
builder.ConfigureAppServices();

//Jwt Config
var tokenConfig = builder.Configuration.GetRequiredSection("TokenConfigs").Get<JwtConfigs>();
builder.Services.Configure<JwtConfigs>(builder.Configuration.GetRequiredSection("TokenConfigs"));
//Auth JwtBearer
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = tokenConfig.Issuer,
            ValidateAudience = true,
            ValidAudience = tokenConfig.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = tokenConfig.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
