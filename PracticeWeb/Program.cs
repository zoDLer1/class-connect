using Microsoft.EntityFrameworkCore;
using PracticeWeb.Models;
using PracticeWeb.Services.FileSystemServices;
using PracticeWeb.Services.GroupStorageServices;
using PracticeWeb.Services.ItemStorageServices;

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
builder.Services.AddTransient<IFileSystemService, FileSystemService>();
builder.Services.AddTransient<IGroupStorageService, GroupStorageService>();
builder.Services.AddTransient<IItemStorageService, ItemStorageService>();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
