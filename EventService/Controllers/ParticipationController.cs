using EventService.Interfaces;
using EventService.Managers;
using EventService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParticipationController : ControllerBase
    {
        private readonly IParticipationManager _participationManager;

        public ParticipationController(IParticipationManager participationManager)
        {
            _participationManager = participationManager;
        }

        [HttpPost("join-event/{id}")]
        [Authorize]
        public async Task<IActionResult> JoinEvent(int id, [FromBody] JoinEventRequest request)
        {
            try
            {
                var membership = await _participationManager.JoinEventAsync(request.UserId, id);
                return Ok(new { Message = "Successfully joined the community." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("leave-community")]
        [Authorize]
        public async Task<IActionResult> LeaveEvent([FromBody] JoinEventRequest request)
        {
            await _participationManager.LeaveEventAsync(request.UserId, request.EventId);
            return NoContent();
        }

        [HttpGet("user-events/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserEvents(int userId)
        {
            try
            {
                var memberships = await _participationManager.GetUserEventsAsync(userId);
                return Ok(memberships);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
