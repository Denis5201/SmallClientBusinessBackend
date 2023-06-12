using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallClientBusiness.DAL.Entities;

public class ServiceEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("WorkerEntity")]
    public Guid? WorkerId { get; set; }
    
    [Required]
    [MinLength(1)]
    public string Name { get; set; }
    
    [Required]
    public double Price { get; set; }

    [Required] 
    public TimeOnly Duration { get; set; }
    
    public List<AppointmentServiceEntity> AppointmentServices { get; set; }
    
    public WorkerEntity? Worker { get; set; }

    public ServiceEntity()
    {
        AppointmentServices = new List<AppointmentServiceEntity>();
    }
}