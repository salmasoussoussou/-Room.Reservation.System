using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using projet1.models;

public class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        // Créer les rôles si ils n'existent pas
        string[] roleNames = { "Admin", "User" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                // Créer le rôle
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Créer un utilisateur admin si il n'existe pas
        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser == null)
        {
            adminUser = new AppUser()
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
            };
            await userManager.CreateAsync(adminUser, "Admin@123");
        }

        // Assigner le rôle Admin à l'utilisateur admin
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Créer un utilisateur normal si il n'existe pas
        var normalUser = await userManager.FindByEmailAsync("user@example.com");
        if (normalUser == null)
        {
            normalUser = new AppUser()
            {
                UserName = "user@example.com",
                Email = "user@example.com",
            };
            await userManager.CreateAsync(normalUser, "User@123");
        }

        // Assigner le rôle User à l'utilisateur normal
        if (!await userManager.IsInRoleAsync(normalUser, "User"))
        {
            await userManager.AddToRoleAsync(normalUser, "User");
        }
    }
}
