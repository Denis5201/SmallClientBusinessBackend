using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.Common.Dto
{
    public class WorkerProfile
    {
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(1)]
        public string FullName { get; set; }

        [Required]
        public DateOnly BirthDate { get; set; }

        public string? Avatar { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        public bool IsSubscribing { get; set; }
    }
}
