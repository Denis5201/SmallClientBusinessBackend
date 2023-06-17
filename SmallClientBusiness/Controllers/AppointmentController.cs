using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Enum;
using SmallClientBusiness.Common.Interfaces;
using SmallClientBusiness.Common.System;

namespace SmallClientBusiness.Controllers
{
    /// <summary>
    /// Контроллер для работы с записями
    /// </summary>
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController: ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        /// <summary>
        /// Коструктор
        /// </summary>
        /// <param name="appointmentService"></param>
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Получение записей в указанный промежуток времени
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("timezone")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<ActionResult<List<Appointment>>> GetAppointments(DateTime startDate, DateTime endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            var appointments = await _appointmentService.GetAppointments(new Guid(userId), startDate, endDate);

            return Ok(appointments);
        }

        /// <summary>
        /// Получение всех записей с фильтрацией и пагинацией
        /// </summary>
        /// <param name="endDate"></param>
        /// <param name="startPrice"></param>
        /// <param name="endPrice"></param>
        /// <param name="startDate"></param>
        /// <param name="servicesId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("filters-pagination")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<ActionResult<List<Appointment>>> GetAppointmentsForSelectedDay(
            double? startPrice,
            double? endPrice,
            DateTime? startDate,
            DateTime? endDate,
            [FromQuery] List<Guid> servicesId,
            [DefaultValue(1)] int page
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
            
            var appointments = await _appointmentService.GetAppointments(new Guid(userId), startPrice, endPrice, startDate, endDate, servicesId, page);

            return Ok(appointments);
        }
        
        /// <summary>
        /// Получение всех записей с фильтрацией
        /// </summary>
        /// <param name="endDate"></param>
        /// <param name="startPrice"></param>
        /// <param name="endPrice"></param>
        /// <param name="startDate"></param>
        /// <param name="servicesId"></param>
        /// <returns></returns>
        [HttpGet("filters")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<ActionResult<List<Appointment>>> GetAppointmentsForSelectedDay(
            double? startPrice,
            double? endPrice,
            DateTime? startDate,
            DateTime? endDate,
            [FromQuery] List<Guid> servicesId
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
            
            var appointments = await _appointmentService.GetAppointments(new Guid(userId), startPrice, endPrice, startDate, endDate, servicesId);

            return Ok(appointments);
        }
        
        /// <summary>
        /// Получение конкретной записи
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <returns></returns>
        [HttpGet("{appointmentId:guid}")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<ActionResult<List<Appointment>>> GetAppointment(Guid appointmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            var appointment = await _appointmentService.GetAppointment(new Guid(userId), appointmentId);

            return Ok(appointment);
        }

        /// <summary>
        /// Создание новой записи
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<IActionResult> CreateAppointment(CreateAppointment model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            await _appointmentService.CreateAppointment(new Guid(userId), model);
            return Ok();
        }

        /// <summary>
        /// Редактирование информации о записи
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{appointmentId:guid}")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<IActionResult> EditAppointment(Guid appointmentId, EditAppointment model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            await _appointmentService.EditAppointment(new Guid(userId), appointmentId, model);
            return Ok();
        }
        
        /// <summary>
        /// Изменить статус записи
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("{appointmentId:guid}/status")]
        [Authorize(Roles = AppRoles.Worker)]
        public async Task<IActionResult> ChangeStatus(Guid appointmentId, StatusAppointment status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            await _appointmentService.ChangeStatus(new Guid(userId), appointmentId, status);
            return Ok();
        }
    }
}