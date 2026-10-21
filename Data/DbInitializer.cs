using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace municipal_election_web.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "SuperAdmin", "MunicipalityAdmin" };
            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var superAdminUsername = "super";
            if (await userManager.FindByNameAsync(superAdminUsername) == null)
            {
                var superAdmin = new IdentityUser { UserName = superAdminUsername };
                var result = await userManager.CreateAsync(superAdmin, "P@$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                }
            }

            var surreyAdminUsername = "surrey";
            if (await userManager.FindByNameAsync(surreyAdminUsername) == null)
            {
                var surreyAdmin = new IdentityUser { UserName = surreyAdminUsername };
                var result = await userManager.CreateAsync(surreyAdmin, "P@$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(surreyAdmin, "MunicipalityAdmin");
                    await userManager.AddClaimAsync(surreyAdmin, new Claim("MunicipalityId", "1"));
                }
            }

            var langleyAdminUsername = "langley";
            if (await userManager.FindByNameAsync(langleyAdminUsername) == null)
            {
                var langleyAdmin = new IdentityUser { UserName = langleyAdminUsername };
                var result = await userManager.CreateAsync(langleyAdmin, "P@$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(langleyAdmin, "MunicipalityAdmin");
                    await userManager.AddClaimAsync(langleyAdmin, new Claim("MunicipalityId", "2"));
                }
            }
        }
    }
}