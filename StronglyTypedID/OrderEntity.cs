 


public class OrderEntity : SoftDeletableEntity, IId<OrderId>
{
    public OrderId Id { get; set; }
    // Other properties...
}
