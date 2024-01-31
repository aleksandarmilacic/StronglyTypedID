
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Reflection;

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

        // Apply value converters to all entities implementing IId<ID>
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var interfaceType = entityType.ClrType.GetInterface("IId`1");
            if (interfaceType != null)
            {
                var idType = interfaceType.GetGenericArguments()[0];
                var idPropertyInfo = entityType.ClrType.GetProperty("Id");

                if (idPropertyInfo != null && idType.IsValueType)
                {
                    var valuePropertyInfo = idType.GetProperty("Value");
                    var guidConstructorInfo = idType.GetConstructor(new[] { typeof(Guid) });

                    if (valuePropertyInfo != null && guidConstructorInfo != null)
                    {
                        var idParamExpr = Expression.Parameter(idType, "id");
                        var valuePropertyExpr = Expression.Property(idParamExpr, valuePropertyInfo);
                        var toGuidLambda = Expression.Lambda<Func<idType, Guid>>(valuePropertyExpr, idParamExpr);

                        var guidParamExpr = Expression.Parameter(typeof(Guid), "guid");
                        var newIdExpr = Expression.New(guidConstructorInfo, guidParamExpr);
                        var fromGuidLambda = Expression.Lambda<Func<Guid, idType>>(newIdExpr, guidParamExpr);

                        var converter = new ValueConverter<idType, Guid>(toGuidLambda.Compile(), fromGuidLambda.Compile());
                        modelBuilder.Entity(entityType.ClrType)
                            .Property(idPropertyInfo.Name)
                            .HasConversion(converter);
                    }
                }
            }
        }


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

