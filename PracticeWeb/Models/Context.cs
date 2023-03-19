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
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<TaskEntity> Tasks { get; set; } = null!;
    public DbSet<Work> Works { get; set; } = null!;
    public DbSet<WorkItem> WorkItems { get; set; } = null!;

    public DbSet<Access> Accesses { get; set; } = null!;

    public Context(DbContextOptions<Context> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Group>()
            .HasIndex(g => g.Name)
            .IsUnique();

        builder.Entity<User>()
            .HasIndex(g => g.RefreshTokenId)
            .IsUnique();

        builder.Entity<Connection>().HasKey(t => new {
            t.ParentId, t.ChildId
        });

        builder.Entity<RefreshToken>().HasKey(t => new {
            t.Token
        });


        builder.Entity<Access>().HasKey(t => new {
            t.ItemId, t.UserId
        });

        builder.Entity<Role>().HasData(
            new Role { Id = UserRole.Student, Name = "Student", Title = "Студент" },
            new Role { Id = UserRole.Teacher, Name = "Teacher", Title = "Преподаватель" },
            new Role { Id = UserRole.Administrator, Name = "Administrator", Title = "Администратор" }
        );

        builder.Entity<User>().HasData(
            new User { Id = 1, Name = "Админ", Surname = "Админов", Email = "admin@admin.admin", RoleId = UserRole.Administrator, Password = "$2a$11$vZJfXw2NUiLp43m/lkoc6.uW5W6ibxwKHFHlKlcoJmHrFvRwk.yWG", RegTime = DateTime.Now },
            new User { Id = 2, Name = "Валенок", Surname = "Купцов", Patronymic = "Анатольевич", Email = "test@test.test", RoleId = UserRole.Teacher, Password = "$2a$11$/DpkLbtTr9oZEJPZpLpyieT67Cd/T5liNN/fm3kf81vJ6L0EhWgHe", RegTime = DateTime.Now },
            new User { Id = 3, Name = "Валентин", Surname = "Купцов", Patronymic = "Анатольевич", Email = "teacher@test.test", RoleId = UserRole.Teacher, Password = "$2a$11$/DpkLbtTr9oZEJPZpLpyieT67Cd/T5liNN/fm3kf81vJ6L0EhWgHe", RegTime = DateTime.Now },
            new User { Id = 4, Name = "Другой", Surname = "Препод", Email = "anotherTeacher@test.test", RoleId = UserRole.Teacher, Password = "$2a$11$/DpkLbtTr9oZEJPZpLpyieT67Cd/T5liNN/fm3kf81vJ6L0EhWgHe", RegTime = DateTime.Now }
        );

        builder.Entity<ItemType>().HasData(
            new ItemType { Id = Type.Folder, Name = "Folder" },
            new ItemType { Id = Type.File, Name = "File" },
            new ItemType { Id = Type.Group, Name = "Group" },
            new ItemType { Id = Type.Subject, Name = "Subject" },
            new ItemType { Id = Type.Task, Name = "Task" },
            new ItemType { Id = Type.Work, Name = "Work" }
        );

        builder.Entity<Item>().HasData(
            new Item { Id = "25aba956-b6c8-473f-b114-8ed881adf6c5", TypeId = Type.Group, Name = "ИСП-564", CreationTime = DateTime.Now, CreatorId = 1 },
            new Item { Id = "7989dbf3-35a0-4efa-9a2f-5fe40e4b7c27", TypeId = Type.Group, Name = "Группа 1", CreationTime = DateTime.Now, CreatorId = 1 }
        );

        builder.Entity<Group>().HasData(
            new Group { Id = "25aba956-b6c8-473f-b114-8ed881adf6c5", Name = "ИСП-564", TeacherId = 3 },
            new Group { Id = "7989dbf3-35a0-4efa-9a2f-5fe40e4b7c27", Name = "Группа 1", TeacherId = 2 }
        );
    }
}
