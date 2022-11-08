using Microsoft.EntityFrameworkCore;
using PracticeWeb.Services;

namespace PracticeWeb.Models;

public class Context : DbContext
{
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<ItemType> ItemTypes { get; set; } = null!;
    public DbSet<FileEntity> Files { get; set; } = null!;
    public DbSet<Connection> Connections { get; set; } = null!;

    public Context(DbContextOptions<Context> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Group>()
            .HasIndex(g => g.Name)
            .IsUnique();

        builder.Entity<Connection>().HasKey(t => new {
            t.ParentId, t.ChildId
        });

        builder.Entity<ItemType>().HasData(
            new ItemType { Id = 1, Name = "Folder" },
            new ItemType { Id = 2, Name = "File" },
            new ItemType { Id = 3, Name = "Group" },
            new ItemType { Id = 4, Name = "Subject" }
        );

        builder.Entity<Item>().HasData(
            new Item { Id = "25aba956-b6c8-473f-b114-8ed881adf6c5", TypeId = 3, Name = "ИСП-564", CreationTime = DateTime.Now },
            new Item { Id = "7989dbf3-35a0-4efa-9a2f-5fe40e4b7c27", TypeId = 3, Name = "Группа 1", CreationTime = DateTime.Now }
        );

        builder.Entity<Group>().HasData(
            new Group { Id = "25aba956-b6c8-473f-b114-8ed881adf6c5", Name = "ИСП-564" },
            new Group { Id = "7989dbf3-35a0-4efa-9a2f-5fe40e4b7c27", Name = "Группа 1" }
        );
    }
}