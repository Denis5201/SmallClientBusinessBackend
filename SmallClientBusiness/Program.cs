using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmallClientBusiness.BL;
using SmallClientBusiness.BL.Swagger;
using SmallClientBusiness.Common.System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Beautich service", 
        Version = "v1"
    });
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    
    options.OperationFilter<AuthOperationFilter>();
    
    var filePath = Path.Combine(System.AppContext.BaseDirectory, "SmallClientBusiness.xml");
    options.IncludeXmlComments(filePath);
});

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

Configurator.SeedRoles(app.Services).Wait();

app.Run();
