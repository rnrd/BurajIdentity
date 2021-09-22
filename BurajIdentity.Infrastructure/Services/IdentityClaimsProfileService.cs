using BurajIdentity.Infrastructure.Persistence;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BurajIdentity.Infrastructure.Services
{
    public class IdentityClaimsProfileService : IProfileService
    {
        //IUSerClaimsPrincipalFactory provides an abstraction for a factory to create a ClaimsPrincipal from a user
        private readonly IUserClaimsPrincipalFactory<AppUser> _claimsFactory;
        //UserManager from identity server deals with user issues and management
        private readonly UserManager<AppUser> _userManager;

        public IdentityClaimsProfileService(IUserClaimsPrincipalFactory<AppUser> claimsFactory, UserManager<AppUser> userManager)
        {
            _claimsFactory = claimsFactory;
            _userManager = userManager;
        }

        //When this interface is implemented we need to process two methods below

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //In order to include our custom claims, we need to implement our own GetProfileDataAsync() method using the IProfileService. 	
            //This method is being called everytime a user claim is requested.
            //GetProfileDataAsync is for loading claims for a user. It is passed an instance of ProfileDataRequestContext.
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            if (user == null) return;

            var principal = await _claimsFactory.CreateAsync(user);
            var claims = principal.Claims.ToList();
            Console.WriteLine("claims " + claims);
            //RequestedClaimTypes is the collection of claim types being requested.
            claims = claims.Where(f => context.RequestedClaimTypes.Contains(f.Type)).ToList();
            Console.WriteLine(" requested claims " + claims);
            claims.Add(new Claim(JwtClaimTypes.GivenName, user.Name));
            claims.Add(new Claim(JwtClaimTypes.Id, user.Id.ToString()));
            claims.Add(new Claim("userEmailAddress", user.Email));
            claims.Add(new Claim(JwtClaimTypes.Role, "user"));
            //Console.WriteLine("final claims " + claims);
            //IssuedClaims is the list of Claim s that will be returned. This is expected to be populated by the custom IProfileService implementation.
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            //IsActiveAsync indicates if a user is currently allowed to obtain tokens. It is passed an instance of IsActiveContext.
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
