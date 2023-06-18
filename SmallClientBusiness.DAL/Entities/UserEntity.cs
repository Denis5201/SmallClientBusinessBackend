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
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpires { get; set; }

        public bool Avatar { get; set; } = false;

        public WorkerEntity WorkerEntity { get; set; }
        
        public SubscribeEntity Subscribe { get; set; }
    }
}
