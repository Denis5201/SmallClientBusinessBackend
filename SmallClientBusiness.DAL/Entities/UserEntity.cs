using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.DAL.Entities
{
    public class UserEntity : IdentityUser<Guid>
    {
        public DateTime BirthDate { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpires { get; set; }

        public string? Avatar { get; set; }

        public Worker Worker { get; set; }
    }
}
