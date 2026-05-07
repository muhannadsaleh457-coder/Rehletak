using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rehletak.Abstractions;
using Rehletak.Shared.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Rehletak.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IServiceManager serviceManager) : ControllerBase
    {
        
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> getCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await serviceManager.usersServices.getCurrentUserAsync(userId);

            return Ok(result);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> updateCurrentUser(UpdateUserRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await serviceManager.usersServices.updateUserAsync(userId, request);
            return Ok(result);
        }
        }
}
