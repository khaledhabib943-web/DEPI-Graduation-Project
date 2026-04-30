using FinalProject.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace FinalProject.Infrastructure.Services
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole<int>>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            
            // Add custom role claim from UserRole enum
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));
            
            // Add FullName claim
            identity.AddClaim(new Claim("FullName", user.FullName));
            
            return identity;
        }
    }
}
