using Application.Abstractions.Data;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Tests.L0.Infrastructure;

public sealed class TestDbContext(DbContextOptions<TestDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.Ignore(u => u.DomainEvents);
        });

        modelBuilder.Entity<TodoItem>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Ignore(t => t.DomainEvents);
        });
    }

    public static TestDbContext Create() =>
        new(new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
