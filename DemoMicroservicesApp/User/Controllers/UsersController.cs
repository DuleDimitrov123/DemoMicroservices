using Microsoft.AspNetCore.Mvc;
using User.Controllers.Requests;
using User.Controllers.Responses;
using User.Repositories;

namespace User.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var id = await _userRepository.Create(
            new Entities.User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Password = request.Password
            },
            cancellationToken);

        return Ok(id);
    }

    [HttpGet]
    public async Task<ActionResult<IList<GetUserResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _userRepository.GetAll(cancellationToken);

        return Ok(
            products
                .Select(p =>
                    new GetUserResponse(p.Id, p.FirstName, p.LastName, p.Username, p.Password)));
    }
}
