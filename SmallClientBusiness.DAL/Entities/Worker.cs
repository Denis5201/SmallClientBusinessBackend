using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.DAL.Entities
{
    public class Worker
    {
        public Guid Id { get; set; }

        public UserEntity User { get; set; }

        public bool IsSubscribing { get; set; } = false;
    }
}
