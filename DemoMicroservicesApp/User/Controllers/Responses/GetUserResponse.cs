namespace User.Controllers.Responses;

public record GetUserResponse(int Id, string FirstName, string LastName, string Username, string Password);
