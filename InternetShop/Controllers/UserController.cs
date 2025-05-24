using FluentValidation;
using InternetShop.Application.BusinessLogic.User;
using InternetShop.Application.BusinessLogic.User.DeleteUser;
using InternetShop.Application.BusinessLogic.User.GetAllUsers;
using InternetShop.Application.BusinessLogic.User.GetClients;
using InternetShop.Application.BusinessLogic.User.GetCurrentUser;
using InternetShop.Application.BusinessLogic.User.Interfaces;
using InternetShop.Application.BusinessLogic.User.Login;
using InternetShop.Application.BusinessLogic.User.Registration;
using InternetShop.Application.BusinessLogic.User.UpdateUser;
using InternetShop.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;

namespace InternetShop.Controllers
{
    [ApiController]
    [Route("api/users")]
    public partial class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<LoginQuery> _loginValidator;
        private readonly IConfiguration _configuration;
        //private readonly IIdentityRepository _identityRepository;

        public UserController(IMediator mediator, IValidator<LoginQuery> loginValidator,
                              IConfiguration configuration /*IIdentityRepository identityRepository *//*, IIdentityService identityService*/)
        {
            _mediator = mediator;
            _loginValidator = loginValidator;
            _configuration = configuration;
            //_identityRepository = identityRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginQuery query, CancellationToken cancellationToken)
        {
            var validation = await _loginValidator.ValidateAsync(query, cancellationToken);
            if (!validation.IsValid)
                return BadRequest(new { Errors = validation.Errors.Select(e => e.ErrorMessage) });

            try
            {
                var tokenResult = await _mediator.Send(query, cancellationToken);

                // Возвращаем токен в стандартном формате
                return Ok(new
                {
                    token = tokenResult.Value,
                    expires = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:ExpiryDays")),
                    email = query.Email
                });
            }
            catch (AuthenticationException)
            {
                return Unauthorized(new { Error = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("registration")]
        //[Authorize(Roles = "Admin,Manager")] // Только админы и руководители
        public async Task<IActionResult> RegisterAsync(
            [FromBody] RegistrationCommand command,
            CancellationToken cancellationToken)
        {

            return await _mediator.Send(command, cancellationToken);
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] GetAllUsersQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                // Фильтрация по ролям в зависимости от прав текущего пользователя
                if (User.IsInRole("Employee"))
                {
                    query.RoleFilters = new List<RoleEnum> { RoleEnum.Client }; // Сотрудники видят только клиентов
                }
                else if (User.IsInRole("Manager"))
                {
                    query.RoleFilters = new List<RoleEnum> { RoleEnum.Client, RoleEnum.Employee }; // Руководители видят сотрудников и клиентов
                }

                var users = await _mediator.Send(query, cancellationToken);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Ошибка",
                    ex.Message,
                    ExceptionType = ex.GetType().Name
                });
            }
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpGet("clients")]
        public async Task<IActionResult> GetClients(
            [FromQuery] GetClientsQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                var users = await _mediator.Send(new GetAllUsersQuery(), cancellationToken);
                var clients = users.Where(u => u.Role == RoleEnum.Client).ToList();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Internal server error",
                    ex.Message,
                    ExceptionType = ex.GetType().Name
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(
            [FromRoute] Guid userId,
            [FromBody] UpdateUserCommand request,
            CancellationToken cancellationToken)
                {
            try
            {
                var command = new UpdateUserCommand(
                    userId,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.Role,
                    request.Password);

                var result = await _mediator.Send(command, cancellationToken);

                return result.IsSuccess
                    ? NoContent()
                    : BadRequest(new { Error = result.Errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Internal server error",
                    ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new DeleteUserCommand(userId), cancellationToken);

                return result.IsSuccess
                    ? NoContent()
                    : BadRequest(new { Error = result.Errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Internal server error",
                    ex.Message
                });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var command = new GetCurrentUserQuery { UserId = userId };

            return await mediator.Send(command, cancellationToken);
        }


        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(
            [FromRoute] Guid userId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(userId), cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(result.Errors);
        }
    }
}