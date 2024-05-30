namespace User.Controllers.Requests;

public record CreateUserRequest(string FirstName, string LastName, string Username, string Password);
