using System.ComponentModel.DataAnnotations;

namespace SmallClientBusiness.Common.Dto;

public class Service
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; }
    
    [Required]
    public double Price { get; set; }

    [Required] 
    public TimeOnly Duration { get; set; }
}