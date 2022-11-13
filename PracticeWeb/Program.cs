using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PracticeWeb;
using PracticeWeb.Models;
using PracticeWeb.Exceptions;
using PracticeWeb.Services;
using PracticeWeb.Services.AuthenticationServices;
using PracticeWeb.Services.FileSystemServices;
using PracticeWeb.Services.FileSystemServices.Helpers;
using PracticeWeb.Services.UserServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<Context>(options => options
    .UseMySql(connection, ServerVersion.AutoDetect(connection)));
builder.Services.AddCors(options => 
{
    options.AddPolicy(name: "test", policy => {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    }); 
});
builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,

            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        };
    });
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IFileSystemService, FileSystemService>();

builder.Services.AddTransient<FileHelperService>();
builder.Services.AddTransient<FolderHelperService>();
builder.Services.AddTransient<GroupHelperService>();
builder.Services.AddTransient<SubjectHelperService>();

builder.Services.AddTransient<ServiceResolver>(serviceProvider => key =>
{
    IFileSystemHelper? service;
    switch (key)
    {
        case "File":
            service = serviceProvider.GetService<FileHelperService>();
            if (service == null)
                throw new ServiceNotFoundException();
            return service;
        case "Folder":
            service = serviceProvider.GetService<FolderHelperService>();
            if (service == null)
                throw new ServiceNotFoundException();
            return service;
        case "Group":
            service = serviceProvider.GetService<GroupHelperService>();
            if (service == null)
                throw new ServiceNotFoundException();
            return service;
        case "Subject":
            service = serviceProvider.GetService<SubjectHelperService>();
            if (service == null)
                throw new ServiceNotFoundException();
            return service;
        default:
            throw new KeyNotFoundException();
    }
});

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("test");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAuthorization();

app.MapControllers();

app.Run();
