namespace Inventory.Controllers.Responses;

public record GetProductResponse(int Id, string Name, string Description, int Quantity);
