using BurajIdentity.Infrastructure.Persistence;
using BurajIdentity.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BurajIdentity.Server.Quickstart.Account
{
    [Route("[controller]")]
    [ApiController]
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RegisterModel _registerModel;

        public RegisterController(UserManager<AppUser> userManager, RegisterModel registerModel)
        {
            _userManager = userManager;
            _registerModel = registerModel;
        }


        [HttpPost]
        public async Task<ActionResult> Register([FromBody] RegisterModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _registerModel.Name = request.Name;
            _registerModel.Email = request.Email;
            _registerModel.Password = request.Password;
            _registerModel.UserName = request.UserName;

            var user = new AppUser { UserName = _registerModel.Email, Name = _registerModel.Name, Email = _registerModel.Email };
            var result = await _userManager.CreateAsync(user, _registerModel.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddClaimAsync(user, new Claim("userName", user.UserName));
            await _userManager.AddClaimAsync(user, new Claim("name", user.Name));
            await _userManager.AddClaimAsync(user, new Claim("email", user.Email));
            await _userManager.AddClaimAsync(user, new Claim("role", "user"));
            return Ok(result);
        }
    }
}
