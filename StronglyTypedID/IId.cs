
public interface IId<ID>
{
    ID Id { get; set; }
}
 
public abstract class BaseIdSD<T> : SoftDeletableEntity, IId<T>, IEquatable<BaseIdSD<T>>, IComparable<BaseIdSD<T>>
{
    public T Id { get; set; }
 
    public override bool Equals(object obj)
    {
        return obj is BaseIdSD<T> other && Equals(other);
    }
 
    public bool Equals(BaseIdSD<T> other)
    {
        return EqualityComparer<T>.Default.Equals(Id, other.Id);
    }
 
    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Id);
    }
 
    public override string ToString()
    {
        return Id.ToString();
    }
 
    public static bool operator ==(BaseIdSD<T> left, BaseIdSD<T> right)
    {
        return left.Equals(right);
    }
 
    public static bool operator !=(BaseIdSD<T> left, BaseIdSD<T> right)
    {
        return !(left == right);
    }
 
    public int CompareTo(BaseIdSD<T> other)
    {
        // Check if T implements IComparable and if Id is not null
        if (Id is IComparable comparable && other.Id != null)
        {
            return comparable.CompareTo(other.Id);
        }
 
        throw new InvalidOperationException(
                       $"The type {typeof(T).Name} does not implement IComparable.");
    }
}

public abstract class BaseIdWithoutSD<T> : SoftDeletableEntity, IId<T>, IEquatable<BaseIdWithoutSD<T>>, IComparable<BaseIdWithoutSD<T>>
{
    public T Id { get; set; }

    public override bool Equals(object obj)
    {
        return obj is BaseIdWithoutSD<T> other && Equals(other);
    }

    public bool Equals(BaseIdWithoutSD<T> other)
    {
        return EqualityComparer<T>.Default.Equals(Id, other.Id);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Id);
    }

    public override string ToString()
    {
        return Id.ToString();
    }

    public static bool operator ==(BaseIdWithoutSD<T> left, BaseIdWithoutSD<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BaseIdWithoutSD<T> left, BaseIdWithoutSD<T> right)
    {
        return !(left == right);
    }

    public int CompareTo(BaseIdWithoutSD<T> other)
    {
        // Check if T implements IComparable and if Id is not null
        if (Id is IComparable comparable && other.Id != null)
        {
            return comparable.CompareTo(other.Id);
        }

        throw new InvalidOperationException(
                       $"The type {typeof(T).Name} does not implement IComparable.");
    }
}
