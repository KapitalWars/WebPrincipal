using Microsoft.AspNetCore.Identity;
using Conqueco.Models;

namespace Conqueco.Data.Seed;

public static class IdentitySeed
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // =========================
        // 1. ROLES
        // =========================
        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // =========================
        // 2. ADMIN USER
        // =========================
        var config = serviceProvider.GetRequiredService<IConfiguration>();

        var adminEmail = config["Admin:Email"] ?? throw new Exception("Admin email missing");
        var adminPassword = config["Admin:Password"] ?? throw new Exception("Admin password missing");

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}