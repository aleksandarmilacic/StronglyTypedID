using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
 



public class MyDbContext : DbContext
{
    // DbSet properties for your entities
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }

    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public override int SaveChanges()
    {
        ApplySoftDeleteLogic();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplySoftDeleteLogic();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplySoftDeleteLogic()
    {
        // prevent soft-deleted entities from being hard-deleted
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is SoftDeletableEntity && e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            var softDeletableEntity = (SoftDeletableEntity)entry.Entity;
            softDeletableEntity.SoftDelete();
            entry.State = EntityState.Modified;
        }

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // Apply global query filters to all entities inheriting from SoftDeletableEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(SoftDeletableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = SetGlobalQueryForSoftDeletableEntityMethod.MakeGenericMethod(entityType.ClrType);
                method.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    private static readonly MethodInfo SetGlobalQueryForSoftDeletableEntityMethod = typeof(MyDbContext)
        .GetMethod(nameof(SetGlobalQueryForSoftDeletableEntity), BindingFlags.NonPublic | BindingFlags.Static);

    static void SetGlobalQueryForSoftDeletableEntity<T>(ModelBuilder modelBuilder) where T : SoftDeletableEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.HasBeenDeleted.HasValue);
    }

}

