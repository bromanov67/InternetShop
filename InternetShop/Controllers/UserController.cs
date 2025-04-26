using FluentValidation;
using InternetShop.Application.BusinessLogic.User;
using InternetShop.Application.BusinessLogic.User.GetAllUsers;
using InternetShop.Application.BusinessLogic.User.Login;
using InternetShop.Application.BusinessLogic.User.Registration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly IValidator<LoginQuery> _loginValidator;
        public UserController(IUserRepository userRepository, IMediator mediator, IValidator<LoginQuery> loginValidator)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _loginValidator = loginValidator;
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync(LoginQuery query, CancellationToken cancellationToken)
        {
            var validateResult = await _loginValidator.ValidateAsync(query, cancellationToken);
            if (!validateResult.IsValid)
                return BadRequest(new { errors = validateResult.Errors.Select(x => x.ErrorMessage) });

            try
            {
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errors = ex.ToString() });
            }

        }

        [HttpPost("registration")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegistrationCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetAllUsersQuery query, CancellationToken cancellationToken)
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
