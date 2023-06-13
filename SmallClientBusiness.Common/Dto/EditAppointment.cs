using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SmallClientBusiness.Common.Dto;

public class EditAppointment
{
    [Required]
    public string ClientName { get; set; }
    
    [Required]
    [Phone]
    [MaybeNull]
    public string ClientPhone { get; set; }
    
    [Required]
    public DateTime StartDateTime { get; set; }
    
    [Required]
    public List<Guid> ServicesId { get; set; }
}