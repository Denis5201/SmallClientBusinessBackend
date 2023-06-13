using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.DAL.Entities
{
    public class WorkerEntity
    {
        public Guid Id { get; set; }

        public UserEntity User { get; set; }

        public bool IsSubscribing { get; set; } = false;
        
        public List<AppointmentEntity> Appointments { get; set; }
        
        public List<ServiceEntity> Services { get; set; }

        public WorkerEntity()
        {
            Appointments = new List<AppointmentEntity>();
            Services = new List<ServiceEntity>();
        }
    }
}
