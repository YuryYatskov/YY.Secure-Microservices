using Duende.IdentityModel;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using IdentityServer.Configurations;
using IdentityServer.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityServer.Data;

public class DatabaseInitialize(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IServiceProvider serviceProvider) : IDatabaseInitialize
{

    public void Initialize()
    {
        // Roles.
        if (roleManager.FindByNameAsync(Config.Admin).Result == null)
        {
            roleManager.CreateAsync(new ApplicationRole(Config.Admin)).GetAwaiter().GetResult();
            roleManager.CreateAsync(new ApplicationRole(Config.Customer)).GetAwaiter().GetResult();
        }

        // Users.
        ApplicationUser adminUser = new()
        {
            UserName = "Bob",
        };

        if (userManager.FindByNameAsync(adminUser.UserName).Result == null)
        {
            userManager.CreateAsync(adminUser, "Qwe123!@#$").GetAwaiter().GetResult();
            userManager.AddToRoleAsync(adminUser, Config.Admin).GetAwaiter().GetResult();
            _ = userManager.AddClaimsAsync(adminUser,
                [
                    new Claim(JwtClaimTypes.Name, adminUser.UserName),
                new Claim(JwtClaimTypes.Role, Config.Admin)
                ]).Result;
        }

        ApplicationUser customerUser = new()
        {
            UserName = "Alice",
        };

        if (userManager.FindByNameAsync(customerUser.UserName).Result == null)
        { 
            userManager.CreateAsync(customerUser, "Qwer1234!@#$").GetAwaiter().GetResult();
            userManager.AddToRoleAsync(customerUser, Config.Customer).GetAwaiter().GetResult();
            _ = userManager.AddClaimsAsync(customerUser,
                [
                    new Claim(JwtClaimTypes.Name, customerUser.UserName),
                    new Claim(JwtClaimTypes.Role, Config.Customer)
                ]).Result;
        }
    }
}
