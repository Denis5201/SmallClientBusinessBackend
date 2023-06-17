using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.Common.Dto
{
    public class ServiceShort
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }
    }
}
