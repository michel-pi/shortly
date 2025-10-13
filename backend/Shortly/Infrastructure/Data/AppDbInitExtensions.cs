using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Shortly.Configuration.Options;
using Shortly.Domain.Identity;

namespace Shortly.Infrastructure.Data;

public static class AppDbInitExtensions
{
    private const string c_AdminRoleName = "admin";

    public static async Task MigrateAndSeedDbAsync(this IServiceProvider serviceProvider)
    {
        using var serviceScope = serviceProvider.CreateScope();
        var scopedServiceProvider = serviceScope.ServiceProvider;

        // db migration

        var db = scopedServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        // initial setup

        var userManager = scopedServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scopedServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        // reset password
        //var a = scopedServiceProvider.GetRequiredService<IPasswordHasher<AppUser>>();
        //var b = a.HashPassword(await userManager.FindByIdAsync("1"), "1234");

        if (!await userManager.Users.AnyAsync())
        {
            var defaultAdminOptions = scopedServiceProvider.GetRequiredService<IOptions<DefaultAdminOptions>>().Value;
            var passwordHasher = scopedServiceProvider.GetRequiredService<IPasswordHasher<AppUser>>();

            var defaultAdminUser = new AppUser
            {
                Email = defaultAdminOptions.Email,
                UserName = defaultAdminOptions.Email,
                EmailConfirmed = true
            };

            defaultAdminUser.PasswordHash = passwordHasher.HashPassword(defaultAdminUser, defaultAdminOptions.Password);

            if (!await roleManager.RoleExistsAsync(c_AdminRoleName))
            {
                var roleResult = await roleManager.CreateAsync(new AppRole { Name = c_AdminRoleName });
                // TODO: error handling
            }

            var userResult = await userManager.CreateAsync(defaultAdminUser);

            _ = await userManager.AddToRoleAsync(defaultAdminUser, c_AdminRoleName);
        }
    }
}
