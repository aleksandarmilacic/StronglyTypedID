
public class UserEntity : SoftDeletableEntity, IId<UserId>
{
    public UserId Id { get; set; }
    // Other properties...
}
