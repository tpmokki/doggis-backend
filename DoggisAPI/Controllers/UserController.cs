using DoggisAPI.Data;
using DoggisAPI.Models;
using DoggisAPI.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoggisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DoggisDBContext _db;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<AppUser> userManager,
            DoggisDBContext db,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "AppUser")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return BadRequest(new
                {
                    error = "Tietoja ei löydy!"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserDTO
            {
                Name = user.Name,
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            });
        }

        [HttpPut]
        [Route("ChangeUserStatus/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> ChangeUserStatus(string id, [FromBody] AppUser body)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return BadRequest(new
                {
                    error = "User not found."
                });
            }

            if (user.Status == body.Status)
            {
                return NoContent();
            }

            user.Status = body.Status;
            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = $"User status has changed to {body.Status}"
            });
        }
    }
}
