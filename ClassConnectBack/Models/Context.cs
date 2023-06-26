using Microsoft.EntityFrameworkCore;
using ClassConnect.Services.UserServices;

namespace ClassConnect.Models;

public class Context : DbContext
{
    public DbSet<ItemEntity> Items { get; set; } = null!;
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

    public Context(DbContextOptions<Context> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().HasIndex(g => g.RefreshTokenId).IsUnique();

        builder.Entity<FileEntity>().HasOne(f => f.Item).WithMany().HasForeignKey(f => f.Id);

        builder.Entity<Group>().HasOne(g => g.Item).WithMany().HasForeignKey(g => g.Id);

        builder.Entity<Subject>().HasOne(s => s.Item).WithMany().HasForeignKey(s => s.Id);

        builder.Entity<TaskEntity>().HasOne(t => t.Item).WithMany().HasForeignKey(t => t.Id);

        builder.Entity<Work>().HasOne(w => w.Item).WithMany().HasForeignKey(w => w.Id);

        builder.Entity<Connection>().HasKey(t => new { t.ParentId, t.ChildId });

        builder.Entity<RefreshToken>().HasKey(t => new { t.Token });

        builder.Entity<Access>().HasKey(t => new { t.ItemId, t.UserId });

        builder
            .Entity<Role>()
            .HasData(
                new Role
                {
                    Id = UserRole.Student,
                    Name = "Student",
                    Title = "Студент"
                },
                new Role
                {
                    Id = UserRole.Teacher,
                    Name = "Teacher",
                    Title = "Преподаватель"
                },
                new Role
                {
                    Id = UserRole.Administrator,
                    Name = "Administrator",
                    Title = "Администратор"
                }
            );

        builder
            .Entity<ItemType>()
            .HasData(
                new ItemType { Id = Item.Folder, Name = "Folder" },
                new ItemType { Id = Item.File, Name = "File" },
                new ItemType { Id = Item.Group, Name = "Group" },
                new ItemType { Id = Item.Subject, Name = "Subject" },
                new ItemType { Id = Item.Task, Name = "Task" },
                new ItemType { Id = Item.Work, Name = "Work" }
            );
    }
}
