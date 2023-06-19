using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using SmallClientBusiness.Common.Enum;

namespace SmallClientBusiness.Common.Dto;

public class CreateAppointment
{
    [Required]
    public string ClientName { get; set; }
    
    [Phone]
    [MaybeNull]
    public string ClientPhone { get; set; }
    
    [Required]
    public DateTime StartDateTime { get; set; }
    
    [Required]
    public List<Guid> IdServices { get; set; }
}