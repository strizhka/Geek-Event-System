using CommunityService.Data;
using CommunityService.Interfaces;
using CommunityService.Managers;
using CommunityService.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace CommunityService.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipManager _membershipManager;

        public MembershipController(IMembershipManager membershipManager)
        {
            _membershipManager = membershipManager;
        }

        [HttpPost("join-community/{id}")]
        public async Task<IActionResult> JoinCommunity(int id, [FromBody] JoinCommunityRequest request)
        {
            try
            {
                var membership = await _membershipManager.JoinCommunityAsync(request.UserId, id);
                return Ok(new { Message = "Successfully joined the community." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("leave")] 
        public async Task<IActionResult> LeaveCommunity([FromBody] JoinCommunityRequest request)
        {
            await _membershipManager.LeaveCommunityAsync(request.UserId, request.CommunityId);
            return NoContent();
        }

        [HttpGet("user-communities/{userId}")]
        public async Task<IActionResult> GetUserCommunities(int userId)
        {
            try
            {
                var memberships = await _membershipManager.GetUserCommunitiesAsync(userId);
                return Ok(memberships);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
