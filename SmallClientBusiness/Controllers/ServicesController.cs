using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Interfaces;
using SmallClientBusiness.Common.System;

namespace SmallClientBusiness.Controllers
{
    /// <summary>
    /// Контроллер для работы с услугами
    /// </summary>
    [Route("api/services")]
    [ApiController]
    public class ServicesController: ControllerBase
    {
        private readonly IServiceService _serviceService;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="serviceService"></param>
        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        /// <summary>
        /// Получить список всех услуг (дефолтных и пользовательские)
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<ActionResult<List<Service>>> GetServices()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
            
            var services = await _serviceService.GetServices(new Guid(userId));
            
            return Ok(services);
        }
        
        /// <summary>
        /// Получить список дефолтных услуг
        /// </summary>
        /// <returns></returns>
        [HttpGet("default")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<ActionResult<List<Service>>> GetDefaultServices()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            var services = await _serviceService.GetDefaultServices();
            
            return Ok(services);
        }
        
        /// <summary>
        /// Получить список услуг, добавленных авторизованным пользователем
        /// </summary>
        /// <returns></returns>
        [HttpGet("custom")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<ActionResult<List<Service>>> GetCustomServices()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
            
            var services = await _serviceService.GetCustomServices(new Guid(userId));
            
            return Ok(services);
        }
        
        /// <summary>
        /// Получить конкретную услугу
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        [HttpGet("{serviceId:guid}")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<ActionResult<List<Service>>> GetService(Guid serviceId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
            
            var services = await _serviceService.GetService(new Guid(userId), serviceId);
            
            return Ok(services);
        }
        
        /// <summary>
        /// Добавить кастомную услугу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<IActionResult> CreateService(CreateService model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
            
            await _serviceService.CreateService(new Guid(userId), model);
            
            return Ok();
        }
        
        /// <summary>
        /// Изменить данные услуги
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{serviceId:guid}")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<IActionResult> EditService(Guid serviceId, EditService model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
            
            await _serviceService.EditService(new Guid(userId), serviceId, model);
            
            return Ok();
        }
        
        /// <summary>
        /// Удаление кастомной услуги
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        [HttpDelete("{serviceId:guid}")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<IActionResult> DeleteService(Guid serviceId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
            
            await _serviceService.DeleteService(new Guid(userId), serviceId);
            
            return Ok();
        }
    }
}