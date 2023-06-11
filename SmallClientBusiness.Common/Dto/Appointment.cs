using System.ComponentModel.DataAnnotations;
using SmallClientBusiness.Common.Enum;

namespace SmallClientBusiness.Common.Dto;

public class Appointment
{
    [Required]
    public string ClientName { get; set; }
    
    [Required]
    public double Price { get; set; }
    
    [Required]
    public DateTime DateTime { get; set; }
    
    public StatusAppointment Status { get; set; } = StatusAppointment.New;
}