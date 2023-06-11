using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Enum;

namespace SmallClientBusiness.Common.Interfaces;

public interface IAppointmentService
{
    Task<List<Appointment>> GetAppointments(Guid workerId, DateTime startDate, DateTime endDate);

    Task<AppointmentPagedList> GetAppointments(
        Guid workerId,
        double? startPrice,
        double? endPrice,
        DateTime? startDate,
        DateTime? endDate,
        List<Guid> servicesId,
        int page
    );

    Task<Appointment> GetAppointment(Guid workerId, Guid appointmentId);
    
    Task CreateAppointment(Guid workerId, CreateAppointment model);
    Task EditAppointment(Guid workerId, Guid appointmentId, EditAppointment model);
    
    Task ChangeStatus(Guid workerId, Guid appointmentId, StatusAppointment status);
}