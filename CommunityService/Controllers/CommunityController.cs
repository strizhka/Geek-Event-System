using CommunityService.Data;
using CommunityService.Interfaces;
using CommunityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace CommunityService.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityManager _communityManager;

        public CommunityController(ICommunityManager communityManager)
        {
            _communityManager = communityManager;
        }

        [HttpPost("create-community")]
        [Authorize]
        public async Task<IActionResult> CreateCommunity([FromBody] CreateCommunityRequest createCommunityRequest)
        {
            try
            {
                var community = await _communityManager.CreateCommunityAsync(createCommunityRequest);
                return Ok(new CreateCommunityResponse
                {
                    Id = community.Id,
                    Name = community.Name
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("get-community/{id}")]
        public async Task<IActionResult> GetCommunity(int id)
        {
            try
            {
                var community = await _communityManager.GetCommunityByIdAsync(id);
                if (community == null)
                    return NotFound("Community not found.");

                return Ok(new CommunityResponse
                {
                    Id = community.Id,
                    Name = community.Name,
                    Description = community.Description
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("list-communities")]
        public async Task<IActionResult> ListCommunities([FromQuery] string? filter)
        {
            try
            {
                var communities = await _communityManager.GetAllCommunitiesAsync();

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    communities = communities
                        .Where(c => c.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                return Ok(communities.Select(c => new CommunityResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPut("update-community/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCommunity(int id, [FromBody] UpdateCommunityRequest updateCommunityRequest)
        {
            try
            {
                await _communityManager.UpdateCommunityAsync(updateCommunityRequest);
                return Ok(new { Message = "Community updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("delete-community/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCommunity(int id)
        {
            try
            {
                await _communityManager.DeleteCommunityAsync(id);
                return Ok(new { Message = "Community deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

}
