using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallClientBusiness.DAL.Entities;

public class SubscribeEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    
    public UserEntity User { get; set; }
    
    public DateTime CreateDate { get; set; }
}