using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallClientBusiness.DAL.Entities;

public class AppointmentServiceEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("AppointmentEntity")]
    public Guid AppointmentId { get; set; }
    
    [Required]
    [ForeignKey("ServiceEnitity")]
    public Guid ServiceId { get; set; }
    
    public AppointmentEntity Appointment { get; set; }
    
    [Required]
    public ServiceEntity Service { get; set; }
}