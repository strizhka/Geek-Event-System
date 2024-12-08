using CommunityService.Models;
using EventService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventManager _eventManager;

        public EventController(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        [HttpPost("create-event")]
        [Authorize]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest createEventRequest)
        {
            try
            {
                var eventEntity = await _eventManager.CreateEventAsync(createEventRequest);
                return Ok(new CreateEventResponse
                {
                    EventId = eventEntity.EventId,
                    Title = eventEntity.Title,
                    Description = eventEntity.Description,
                    Date = eventEntity.Date,
                    Address = eventEntity.Address,
                    CommunityId = eventEntity.CommunityId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("get-event/{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            try
            {
                var eventEntity = await _eventManager.GetEventByIdAsync(id);
                if (eventEntity == null)
                    return NotFound("Event not found.");

                return Ok(new EventResponse
                {
                    EventId = eventEntity.EventId,
                    Title = eventEntity.Title,
                    Description = eventEntity.Description,
                    Date = eventEntity.Date,
                    Address = eventEntity.Address,
                    CommunityId = eventEntity.CommunityId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("list-events")]
        public async Task<IActionResult> ListEvents([FromQuery] int? communityId)
        {
            try
            {
                var events = await _eventManager.GetAllEventsAsync();

                if (communityId.HasValue)
                {
                    events = events
                        .Where(e => e.CommunityId == communityId.Value)
                        .ToList();
                }

                return Ok(events.Select(e => new EventResponse
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Description = e.Description,
                    Date = e.Date,
                    Address = e.Address,
                    CommunityId = e.CommunityId
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("update-event/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventRequest updateEventRequest)
        {
            try
            {
                if (id != updateEventRequest.EventId)
                {
                    return BadRequest("Event ID mismatch.");
                }

                await _eventManager.UpdateEventAsync(updateEventRequest);
                return Ok(new { Message = "Event updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("delete-event/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                await _eventManager.DeleteEventAsync(id);
                return Ok(new { Message = "Event deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

}
