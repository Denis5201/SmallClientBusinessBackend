using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Interfaces;
using SmallClientBusiness.Common.System;
using System.Data;
using System.Security.Claims;

namespace SmallClientBusiness.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [Authorize(Roles = AppRoles.Worker)]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("")]
        public async Task<ActionResult<WorkerProfile>> GetWorkerProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }
            var profile = await _profileService.GetWorkerProfile(userId);
            return Ok(profile);
        }

        [HttpPatch("")]
        public async Task<IActionResult> ChangeProfile(ChangeUser changeUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            await _profileService.ChangeProfile(userId, changeUser);
            return Ok();
        }

        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            await _profileService.ChangePassword(userId, changePassword);
            return Ok();
        }

        [HttpPut("subscribe")]
        public async Task<IActionResult> ChangeSubscribingStatus(bool isSubscribing)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();
            }

            await _profileService.SetSubscribingStatus(userId, isSubscribing);

            return Ok();
        }
    }
}
