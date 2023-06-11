using System.ComponentModel.DataAnnotations;
using SmallClientBusiness.Common.Enum;

namespace SmallClientBusiness.DAL.Entities;

public class AppointmentEntity
{
    [Required]
    public double Price { get; set; }
    
    [Required]
    public DateTime Created { get; set; }
    
    public StatusAppointment Status { get; set; } = StatusAppointment.New;
}