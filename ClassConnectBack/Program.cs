using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ClassConnect;
using ClassConnect.Exceptions;
using ClassConnect.Helpers;
using ClassConnect.Models;
using ClassConnect.Services;
using ClassConnect.Services.AuthenticationServices;
using ClassConnect.Services.FileSystemServices;
using ClassConnect.Services.FileSystemServices.Helpers;
using ClassConnect.Services.MailServices;
using ClassConnect.Services.UserServices;

string Ask(string question)
{
    while (true)
    {
        Console.WriteLine(question);
        var answer = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(answer))
            return answer.Trim();
    }
}

async void CreateAdmin(Context context)
{
    var email = "";
    var attr = new EmailAddressAttribute();
    while (!attr.IsValid(email))
        email = Ask("Enter the admin email:");

    var password = Ask("Enter the admin password:");
    context.Users.Add(
        new User
        {
            Name = "Админ",
            Surname = "Админов",
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            RegTime = DateTime.Now,
            RoleId = UserRole.Administrator
        }
    );

    await context.SaveChangesAsync();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<Context>(
    options => options.UseMySql(connection, ServerVersion.AutoDetect(connection))
);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<Client>(builder.Configuration.GetSection("ClientSettings"));

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "test",
        policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        }
    );
});
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    });

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IFileSystemService, FileSystemService>();
builder.Services.AddSingleton<IMailService, MailService>();

builder.Services.AddScoped<FileHelperService>();
builder.Services.AddScoped<FolderHelperService>();
builder.Services.AddScoped<GroupHelperService>();
builder.Services.AddScoped<SubjectHelperService>();
builder.Services.AddScoped<TaskHelperService>();
builder.Services.AddScoped<WorkHelperService>();

builder.Services.AddScoped<ServiceResolver>(
    serviceProvider =>
        key =>
        {
            TService GetService<TService>()
            {
                var service = serviceProvider.GetService<TService>();
                if (service == null)
                    throw new ServiceNotFoundException();
                return service;
            }
            ;
            switch (key)
            {
                case ClassConnect.Type.File:
                    return GetService<FileHelperService>();
                case ClassConnect.Type.Folder:
                    return GetService<FolderHelperService>();
                case ClassConnect.Type.Group:
                    return GetService<GroupHelperService>();
                case ClassConnect.Type.Subject:
                    return GetService<SubjectHelperService>();
                case ClassConnect.Type.Task:
                    return GetService<TaskHelperService>();
                case ClassConnect.Type.Work:
                    return GetService<WorkHelperService>();
                default:
                    throw new ItemTypeException() { PropertyName = "Type" };
            }
        }
);

builder.Services.AddSingleton<ClientLinkGenerator>();
builder.Services.AddSingleton<GenerateLink>(
    s => s.GetRequiredService<ClientLinkGenerator>().GenerateLink
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Context>();
    var isCreated = await context.Database.EnsureCreatedAsync();
    if (isCreated)
    {
        CreateAdmin(context);
        await scope.ServiceProvider
            .GetRequiredService<IFileSystemService>()
            .RecreateFileSystemAsync();
    }
}

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
