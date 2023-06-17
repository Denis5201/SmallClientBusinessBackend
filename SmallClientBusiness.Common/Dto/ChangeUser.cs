using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.Common.Dto
{
    public class ChangeUser
    {
        [Required]
        [MinLength(1)]
        public string FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
