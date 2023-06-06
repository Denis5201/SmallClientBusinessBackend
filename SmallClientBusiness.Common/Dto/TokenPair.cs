using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.Common.Dto
{
    public class TokenPair
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshaToken { get; set; }
    }
}
