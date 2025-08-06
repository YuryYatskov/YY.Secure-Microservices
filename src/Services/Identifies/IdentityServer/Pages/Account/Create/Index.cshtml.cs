using Duende.IdentityModel;
using Duende.IdentityServer.Services;
using IdentityServer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace IdentityServer.Pages.Create;

[SecurityHeaders]
[AllowAnonymous]
public class Index(
    IIdentityServerInteractionService interaction,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<ApplicationRole> roleManager) : PageModel
{
    private readonly IIdentityServerInteractionService _interaction = interaction;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public IActionResult OnGet(string? returnUrl)
    {
        Input = new InputModel { ReturnUrl = returnUrl };
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser()
            {
                UserName = Input.Username,
                Email = Input.Email,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(user, Input.Password!);
            if (result.Succeeded)
            {
                if (!roleManager.RoleExistsAsync(Input.RoleName!).GetAwaiter().GetResult())
                {
                    var userRole = new ApplicationRole
                    {
                        Name = Input.RoleName,
                        NormalizedName = Input.RoleName
                    };
                    var role = await roleManager.CreateAsync(userRole);
                }
                var ToRole = await userManager.AddToRoleAsync(user, Input.RoleName!);

                var claims = await userManager.AddClaimsAsync(user,
                  [
                      new Claim(JwtClaimTypes.Name, Input.Name!),
                      new Claim(JwtClaimTypes.Email, Input.Email!),
                      new Claim(JwtClaimTypes.Role, Input.RoleName!)
                  ]);

                var loginresult = await signInManager.PasswordSignInAsync(
                    Input.Email!,
                    Input.Password!,
                    false,
                    lockoutOnFailure: true);

                if (loginresult.Succeeded)
                {
                    if (Url.IsLocalUrl(Input.ReturnUrl))
                    {
                        return Redirect(Input.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(Input.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        throw new Exception("Invalid return URL");
                    }
                }
            }
        }

        return Page();
    }
}
