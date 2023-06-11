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
        /// Получение всех записей
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<ActionResult<List<Appointment>>> GetAppointments()
        {
            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // if (userId == null)
            // {
            //     return Forbid();
            // }

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
        /// <param name="endDate"></param>
        /// <param name="services"></param>
        /// <param name="startPrice"></param>
        /// <param name="endPrice"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        [HttpGet("filters")]
        public async Task<ActionResult<List<Appointment>>> GetAppointmentsForSelectedDay(
            DateOnly? selectedDay,
            double? startPrice,
            double? endPrice,
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
        /// Получение конкретной записи
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <returns></returns>
        [HttpGet("{appointmentId:guid}")]
        public async Task<ActionResult<List<Appointment>>> GetAppointment(Guid appointmentId)
        {
            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // if (userId == null)
            // {
            //     return Forbid();
            // }

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
            
            return Ok();
        }

        /// <summary>
        /// Редактирование информации о записи
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{appointmentId:guid}")]
        public async Task<IActionResult> EditAppointment(Guid appointmentId, EditAppointment model)
        {
            return Ok();
        }
        
        /// <summary>
        /// Изменить статус записи
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("{appointmentId:guid}")]
        public async Task<IActionResult> EditAppointment(Guid appointmentId, StatusAppointment status)
        {
            return Ok();
        }
    }
}