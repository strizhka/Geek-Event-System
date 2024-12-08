using CommunityService.Models;

namespace EventService.Interfaces
{
    public interface IEventManager
    {
        Task<EventResponse> CreateEventAsync(CreateEventRequest request);
        Task<EventResponse> GetEventByIdAsync(int communityId);
        Task<IEnumerable<EventResponse>> GetAllEventsAsync();
        Task UpdateEventAsync(UpdateEventRequest request);
        Task DeleteEventAsync(int communityId);
    }
}
