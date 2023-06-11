using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Interfaces;
using SmallClientBusiness.Common.System;

namespace SmallClientBusiness.Controllers
{
    /// <summary>
    /// Контроллер для работы с записями
    /// </summary>
    [Route("api/appointments")]
    [ApiController]
    [Authorize(Roles = AppRoles.Worker)]
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
        /// Получение всех записей
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<ActionResult<List<Appointment>>> GetAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            return Ok();
        }
        

        /// <summary>
        /// Получение записей в указанный промежуток времени
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("timezone")]
        public async Task<ActionResult<List<Appointment>>> GetAppointments(DateOnly startDate, DateOnly endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            return Ok();
        }
        
        /// <summary>
        /// Получение записей с фильтрацией
        /// </summary>
        /// <param name="selectedDay"></param>
        /// <param name="services"></param>
        /// <param name="sortingForPrice"></param>
        /// <param name="sortingForDate"></param>
        /// <returns></returns>
        [HttpGet("filters")]
        public async Task<ActionResult<List<Appointment>>> GetAppointmentsForSelectedDay(
            DateOnly? selectedDay,
            Double? startPrice,
            Double? endPrice,
            DateOnly? startDate,
            DateOnly? endDate,
            List<Service> services
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
        
            return Ok();
        }

        /// <summary>
        /// Создание новой записи
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> CreateAppointment(CreateAppointment model)
        {
            return Ok();
        }
        
        /// <summary>
        /// Редактирование информации о записи
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("")]
        public async Task<IActionResult> EditAppointment(EditAppointment model)
        {
            return Ok();
        }
    }
}