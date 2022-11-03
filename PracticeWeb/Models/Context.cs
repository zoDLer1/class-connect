using Microsoft.EntityFrameworkCore;
using PracticeWeb.Services;

namespace PracticeWeb.Models;

public class Context : DbContext
{
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<ItemType> ItemTypes { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;

    public Context(DbContextOptions<Context> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ItemType>().HasData(
            new ItemType { Id = 0, Name = "Folder" },
            new ItemType { Id = 1, Name = "File" }
        );

        builder.Entity<Item>().HasData(
            new Item { Id = "25aba956-b6c8-473f-b114-8ed881adf6c5", TypeId = 0, Name = "ИСП-564" },
            new Item { Id = "7989dbf3-35a0-4efa-9a2f-5fe40e4b7c27", TypeId = 0, Name = "Группа 1" }
        );

        builder.Entity<Group>().HasData(
            new Group { Id = "25aba956-b6c8-473f-b114-8ed881adf6c5", Name = "ИСП-564" },
            new Group { Id = "7989dbf3-35a0-4efa-9a2f-5fe40e4b7c27", Name = "Группа 1" }
        );
    }
}