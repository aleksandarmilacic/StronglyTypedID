public record struct UserId : IComparable
{
    private readonly BaseId<Guid> _baseId;

    public UserId(Guid value)
    {
        _baseId = new BaseId<Guid>(value);
    }

    public Guid Value => _baseId.Value;

    public int CompareTo(object? obj)
    {
        return Value.CompareTo(obj);
    }
     
    public override int GetHashCode() => _baseId.GetHashCode();
    public override string ToString() => _baseId.ToString();
}
