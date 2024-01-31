public abstract class SoftDeletableEntity
{
    public DateTime? HasBeenDeleted { get; private set; }

    public void SoftDelete()
    {
        HasBeenDeleted = DateTime.UtcNow;
    }

    public bool IsSoftDeleted() => HasBeenDeleted.HasValue;
}
