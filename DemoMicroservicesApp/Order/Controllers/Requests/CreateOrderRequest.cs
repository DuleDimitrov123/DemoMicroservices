namespace Order.Controllers.Requests;

public record CreateOrderRequest(IList<CreateProductRequest> Products, string Username, int UserServiceId);

public record CreateProductRequest(string Name, int ProductServiceId);
