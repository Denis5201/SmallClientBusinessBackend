using SmallClientBusiness.Common.Dto;

namespace SmallClientBusiness.Common.Interfaces;

public interface IAppointmentService
{
    Task CreateAppointment(Guid workerId, CreateAppointment model);
    Task EditAppointment(Guid workerId, Guid appointmentId, EditAppointment model);
}