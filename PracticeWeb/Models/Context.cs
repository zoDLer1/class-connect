using Microsoft.EntityFrameworkCore;

namespace PracticeWeb.Models;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
        Database.EnsureCreated();
    }
}