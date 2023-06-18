using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.DAL.Entities;

namespace SmallClientBusiness.DAL
{
    public class AppDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
    {
        public DbSet<WorkerEntity> Workers { get; set; }
        public DbSet<AppointmentEntity> Appointments { get; set; }
        public DbSet<ServiceEntity> Services { get; set; }
        public DbSet<AppointmentServiceEntity> AppointmentService { get; set; }
        public DbSet<SubscribeEntity> SubscribeEntities { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserEntity>()
                .HasOne(x => x.WorkerEntity)
                .WithOne(x => x.User)
                .HasForeignKey<WorkerEntity>(x => x.Id).IsRequired();
            
            builder.Entity<UserEntity>()
                .HasOne(e => e.Subscribe)
                .WithOne(e => e.User)
                .HasForeignKey<SubscribeEntity>(e => e.UserId)
                .IsRequired();

            builder.Entity<AppointmentServiceEntity>()
                .HasKey(e => new {e.AppointmentId, e.ServiceId});
        
            builder.Entity<AppointmentServiceEntity>()
                .HasOne(e => e.Appointment)
                .WithMany(e => e.AppointmentServices)
                .HasForeignKey(e => e.AppointmentId)
                .IsRequired();
        
            builder.Entity<AppointmentServiceEntity>()
                .HasOne(e => e.Service)
                .WithMany(e => e.AppointmentServices)
                .HasForeignKey(e => e.ServiceId)
                .IsRequired();

            builder.Entity<AppointmentEntity>()
                .HasOne(e => e.Worker)
                .WithMany(e => e.Appointments)
                .HasForeignKey(e => e.WorkerId)
                .IsRequired();

            builder.Entity<ServiceEntity>()
                .HasOne(e => e.Worker)
                .WithMany(e => e.Services)
                .HasForeignKey(e => e.WorkerId);

            builder.Entity<ServiceEntity>().HasData(
                new ServiceEntity[]
                {
                    new ServiceEntity { Id = Guid.Parse("3f044dec-0643-43b8-a3da-3e8b7749d665"), 
                        Name = "Маникюр", Price = 1200, Duration = new TimeOnly(1, 0) },
                    new ServiceEntity { Id = Guid.Parse("0cdb988c-4b9f-4713-805f-864a9c7563ac"), 
                        Name = "Наращивание ногтей", Price = 1000, Duration = new TimeOnly(2, 0) },
                    new ServiceEntity { Id = Guid.Parse("d6450d0d-c6df-450a-9534-8f843ec73eb7"), 
                        Name = "Укладка волос", Price = 400, Duration = new TimeOnly(0, 45) },
                    new ServiceEntity { Id = Guid.Parse("b1942411-2a3f-416b-bc3d-9f22090b02f5"), 
                        Name = "Окрашивание", Price = 2500, Duration = new TimeOnly(1, 15) },
                    new ServiceEntity { Id = Guid.Parse("20a90b4c-ceaa-4ae5-8502-c20114425150"), 
                        Name = "Массаж", Price = 800, Duration = new TimeOnly(0, 40) },
                    new ServiceEntity { Id = Guid.Parse("146ff8c8-b942-44ae-8e1e-cc658ec47bad"), 
                        Name = "Ламинирование бровей", Price = 600, Duration = new TimeOnly(0, 50) },
                    new ServiceEntity { Id = Guid.Parse("fb767e25-05c7-4a10-bec8-08f669eb16d3"), 
                        Name = "Наращивание ресниц", Price = 1400, Duration = new TimeOnly(1, 10) }
                });
        }
    }
}
