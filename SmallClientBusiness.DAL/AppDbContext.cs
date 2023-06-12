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

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserEntity>()
                .HasOne(x => x.WorkerEntity)
                .WithOne(x => x.User)
                .HasForeignKey<WorkerEntity>(x => x.Id).IsRequired();

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
        }
    }
}
