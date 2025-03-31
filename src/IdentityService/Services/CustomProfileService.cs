using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService;

public class CustomProfileService(UserManager<ApplicationUser> userManager) : IProfileService
{
    UserManager<ApplicationUser> _userManager = userManager;

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        var existingClaims = await userManager.GetClaimsAsync(user);
        var newClaims = new [] 
        {
            new Claim("username", user.UserName ?? "")
        };
        context.IssuedClaims.Add(existingClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name));
        context.IssuedClaims.AddRange(newClaims);
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        return Task.CompletedTask;
    }
}
