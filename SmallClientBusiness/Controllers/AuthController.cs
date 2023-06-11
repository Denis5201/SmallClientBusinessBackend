using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Interfaces;
using System.Security.Claims;

namespace SmallClientBusiness.Controllers
{
    /// <summary>
    /// Контроллер для аутентификации/регистрации
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="authService"></param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Зарегистрировать пользователя
        /// </summary>
        /// <param name="workerUser"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<TokenPair>> Register(CreateWorkerUser workerUser)
        {
            var tokens = await _authService.CreateWorker(workerUser);
            return Ok(tokens);
        }

        /// <summary>
        /// Залогинить пользователя
        /// </summary>
        /// <param name="loginCredentials"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<TokenPair>> Login(LoginCredentials loginCredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tokens = await _authService.Login(loginCredentials);
            return Ok(tokens);
        }

        /// <summary>
        /// Обновить access и refresh токены
        /// </summary>
        /// <param name="oldTokens"></param>
        /// <returns></returns>
        [HttpPost("refresh")]
        public async Task<ActionResult<TokenPair>> Refresh(TokenPair oldTokens)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tokens = await _authService.Refresh(oldTokens);
            return Ok(tokens);
        }

        /// <summary>
        /// Выйти из системы
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
            await _authService.Logout(userId);
            return Ok();
        }
    }
}
