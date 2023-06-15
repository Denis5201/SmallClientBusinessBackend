using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using SmallClientBusiness.Common.Enum;

namespace SmallClientBusiness.DAL.Entities;

public class AppointmentEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string ClientName { get; set; }
    
    [ForeignKey("WorkerEntity")]
    public Guid WorkerId { get; set; }
    
    [Required]
    public double Price { get; set; }
    
    [Required]
    [Phone]
    [MaybeNull]
    public string ClientPhone { get; set; }
    
    [Required]
    public DateTime StartDateTime { get; set; }
    
    [Required]
    public DateTime EndDateTime { get; set; }
    
    public StatusAppointment Status { get; set; } = StatusAppointment.New;
    
    public List<AppointmentServiceEntity> AppointmentServices { get; set; }
    
    public WorkerEntity Worker { get; set; }

    public AppointmentEntity()
    {
        AppointmentServices = new List<AppointmentServiceEntity>();
    }
}