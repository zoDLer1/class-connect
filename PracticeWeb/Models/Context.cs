using Microsoft.EntityFrameworkCore;
using PracticeWeb.Services.FileSystemServices;

namespace PracticeWeb.Models;

public class Context : DbContext
{
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<ItemType> ItemTypes { get; set; } = null!;
    public DbSet<FileEntity> Files { get; set; } = null!;
    public DbSet<Connection> Connections { get; set; } = null!;

    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<GroupStudent> GroupStudents { get; set; } = null!;

    public Context(DbContextOptions<Context> options) : base(options)
    {
        
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

        builder.Entity<GroupStudent>().HasKey(t => new {
            t.GroupId, t.StudentId
        });

        builder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Student", Title = "Студент" },
            new Role { Id = 2, Name = "Teacher", Title = "Преподаватель" },
            new Role { Id = 3, Name = "Administrator", Title = "Администратор" }
        );

        builder.Entity<User>().HasData(
            new User { Id = 1, FirstName = "Админ", LastName = "Админов", Email = "admin@admin.admin", RoleId = 3, Password = "$2a$11$vZJfXw2NUiLp43m/lkoc6.uW5W6ibxwKHFHlKlcoJmHrFvRwk.yWG", RegTime = DateTimeOffset.Now.ToUnixTimeSeconds()},
            new User { Id = 2, FirstName = "Валенок", LastName = "Купцов", Patronymic = "Анатольевич", Email = "test@test.test", RoleId = 2, Password = "$2a$11$/DpkLbtTr9oZEJPZpLpyieT67Cd/T5liNN/fm3kf81vJ6L0EhWgHe", RegTime = DateTimeOffset.Now.ToUnixTimeSeconds()},
            new User { Id = 3, FirstName = "Валентин", LastName = "Купцов", Patronymic = "Анатольевич", Email = "teacher", RoleId = 2, Password = "$2a$11$/DpkLbtTr9oZEJPZpLpyieT67Cd/T5liNN/fm3kf81vJ6L0EhWgHe", RegTime = DateTimeOffset.Now.ToUnixTimeSeconds()}
        );

        builder.Entity<ItemType>().HasData(
            new ItemType { Id = 1, Name = "Folder" },
            new ItemType { Id = 2, Name = "File" },
            new ItemType { Id = 3, Name = "Group" },
            new ItemType { Id = 4, Name = "Subject" },
            new ItemType { Id = 5, Name = "Task" }
        );

        builder.Entity<Item>().HasData(
            new Item { Id = "25aba956-b6c8-473f-b114-8ed881adf6c5", TypeId = 3, Name = "ИСП-564", CreationTime = DateTime.Now, CreatorId = 1 },
            new Item { Id = "7989dbf3-35a0-4efa-9a2f-5fe40e4b7c27", TypeId = 3, Name = "Группа 1", CreationTime = DateTime.Now, CreatorId = 1 }
        );

        builder.Entity<Group>().HasData(
            new Group { Id = "25aba956-b6c8-473f-b114-8ed881adf6c5", Name = "ИСП-564", TeacherId = 2 },
            new Group { Id = "7989dbf3-35a0-4efa-9a2f-5fe40e4b7c27", Name = "Группа 1", TeacherId = 2 }
        );

        
    }
}