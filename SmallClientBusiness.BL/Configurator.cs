using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmallClientBusiness.Common;
using SmallClientBusiness.DAL;
using SmallClientBusiness.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.BL
{
    public static class Configurator
    {
        public static void ConfigureAppDb(this WebApplicationBuilder builder)
        {
            var connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection));
        }

        public static void ConfigureIdentity(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<UserEntity, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+" +
                "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ ";
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AppDbContext>();
        }

        public static void ConfigureAppServices(this WebApplicationBuilder builder)
        {
        }

        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                RoleManager<IdentityRole<Guid>> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                if (!await roleManager.RoleExistsAsync(AppRoles.Worker))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(AppRoles.Worker));
                }
            }
        }
    }
}
