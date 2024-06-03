namespace Messages;

public class CreatedOrderEvent
{
    public List<ProductEvent> Products { get; set; }

    public string Username { get; set; }

    public int UserServiceId { get; set; }
}

public class ProductEvent
{
    public string Name { get; set; }

    public int ProductServiceId { get; set; }
}