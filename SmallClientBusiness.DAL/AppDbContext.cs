using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmallClientBusiness.DAL.Entities;

namespace SmallClientBusiness.DAL
{
    public class AppDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
    {
        public DbSet<Worker> Workers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
 
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserEntity>().HasOne(x => x.Worker).WithOne(x => x.User)
                .HasForeignKey<Worker>(x => x.Id).IsRequired();
        }
    }
}
