using System.ComponentModel.DataAnnotations;
using SmallClientBusiness.Common.Enum;

namespace SmallClientBusiness.Common.Dto;

public class Appointment
{
    public Guid Id { get; set; }
    
    [Required]
    public string ClientName { get; set; }
    
    [Phone]
    public string? ClientPhone { get; set; }

    [Required]
    public List<ServiceShort> Services { get; set; }
    
    [Required]
    public double Price { get; set; }
    
    [Required]
    public DateTime StartDateTime { get; set; }
    
    [Required]
    public DateTime EndDateTime { get; set; }
    
    public StatusAppointment Status { get; set; } = StatusAppointment.New;
}