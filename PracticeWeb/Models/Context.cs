using Microsoft.EntityFrameworkCore;
using PracticeWeb.Services;

namespace PracticeWeb.Models;

public class Context : DbContext
{
    public DbSet<Group> Groups { get; set; } = null!;

    public Context(DbContextOptions<Context> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Group>().HasData(
            new Group{ Id = Guid.NewGuid().ToString(), Name = "ИСП-564" },
            new Group{ Id = Guid.NewGuid().ToString(), Name = "Группа 1" }
        );
    }
}