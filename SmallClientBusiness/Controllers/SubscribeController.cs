using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Interfaces;
using SmallClientBusiness.Common.System;

namespace SmallClientBusiness.Controllers
{
    /// <summary>
    /// Контроллер для работы с подпиской
    /// </summary>
    [Route("api/subscribe")]
    [ApiController]
    public class SubscribeController : ControllerBase
    {
        private readonly ISubscribeService _subscribeService;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="subscribeService"></param>
        public SubscribeController(ISubscribeService subscribeService)
        {
            _subscribeService = subscribeService;
        }
        
        /// <summary>
        /// Проверить наличие подписки
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<ActionResult<bool>> IsSubscribing()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            var isSubscribing = await _subscribeService.IsSubscribing(new Guid(userId));

            return Ok(isSubscribing);
        }

        /// <summary>
        /// Изменить статус подписки
        /// </summary>
        /// <param name="isSubscribing"></param>
        /// <returns></returns>
        [HttpPut("")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<IActionResult> ChangeSubscribingStatus(bool isSubscribing)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            await _subscribeService.SetSubscribingStatus(userId, isSubscribing);

            return Ok();
        }
        
        /// <summary>
        /// Получить информацию о подписке
        /// </summary>
        /// <returns></returns>
        [HttpGet("details")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<ActionResult<Subscribe>> GetSubscribeInformation()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            var subscribe = await _subscribeService.GetSubscribeInformation(new Guid(userId));

            return Ok(subscribe);
        }
    }
}