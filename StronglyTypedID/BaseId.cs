﻿public record struct BaseId<T> : IEquatable<BaseId<T>>, IComparable<BaseId<T>>
{
    public T Value { get; }

    public BaseId(T value)
    {
        Value = value;
    }

  

    public bool Equals(BaseId<T> other)
    {
        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

   
    public int CompareTo(BaseId<T> other)
    {
        // Check if T implements IComparable and if Value is not null
        if (Value is IComparable comparable && other.Value != null)
        {
            return comparable.CompareTo(other.Value);
        }

        throw new InvalidOperationException(
            $"The type {typeof(T).Name} does not implement IComparable.");
    }
}
