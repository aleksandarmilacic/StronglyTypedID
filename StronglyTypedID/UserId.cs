public struct UserId
{
    private readonly BaseId<Guid> _baseId;

    public UserId(Guid value)
    {
        _baseId = new BaseId<Guid>(value);
    }

    public Guid Value => _baseId.Value;

    public override bool Equals(object obj) => _baseId.Equals(obj);
    public override int GetHashCode() => _baseId.GetHashCode();
    public override string ToString() => _baseId.ToString();
}
