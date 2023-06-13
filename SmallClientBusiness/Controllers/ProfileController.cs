using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Interfaces;
using SmallClientBusiness.Common.System;
using System.Data;
using System.Security.Claims;

namespace SmallClientBusiness.Controllers
{
    /// <summary>
    /// Контроллер для взаимодействия с аккаунтом пользователя
    /// </summary>
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="profileService"></param>
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Получение профиля работника
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<ActionResult<WorkerProfile>> GetWorkerProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
            var profile = await _profileService.GetWorkerProfile(userId);
            return Ok(profile);
        }

        /// <summary>
        /// Изменить данные профиля
        /// </summary>
        /// <param name="changeUser"></param>
        /// <returns></returns>
        [HttpPatch("")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<IActionResult> ChangeProfile(ChangeUser changeUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            await _profileService.ChangeProfile(userId, changeUser);
            return Ok();
        }

        /// <summary>
        /// Изменить пароль
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        [HttpPut("password")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            await _profileService.ChangePassword(userId, changePassword);
            return Ok();
        }

        /// <summary>
        /// Изменить статус подписки
        /// </summary>
        /// <param name="isSubscribing"></param>
        /// <returns></returns>
        [HttpPut("subscribe")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<IActionResult> ChangeSubscribingStatus(bool isSubscribing)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            await _profileService.SetSubscribingStatus(userId, isSubscribing);

            return Ok();
        }
    }
}
