using FluentResults;
using InternetShop.Application.User;
using InternetShop.Application.User.CreateUser;
using InternetShop.Application.User.GetAllUsers;
using InternetShop.Application.User.Login;
using InternetShop.Application.User.Registration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace InternetShop.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        public UserController(IUserRepository userRepository, IMediator mediator)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync(LoginQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Register([FromBody] RegistrationCommand command)
        {
            return await _mediator.Send(command);
        }
        [HttpPost]
        public async Task<IActionResult> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _mediator.Send(command, cancellationToken);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errors = ex.ToString() });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var allUsers = await _mediator.Send(query, cancellationToken);
                return Ok(allUsers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errors = ex.ToString() });
            }
        }
    }
}
