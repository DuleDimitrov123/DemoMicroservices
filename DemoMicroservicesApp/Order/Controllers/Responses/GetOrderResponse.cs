namespace Order.Controllers.Responses;

public record GetOrderResponse(string Id, IList<GetProductResponse> Products, string Username);

public record GetProductResponse(string Name);
