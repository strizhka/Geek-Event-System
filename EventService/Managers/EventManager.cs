using CommunityService.Models;
using EventService.Interfaces;

namespace EventService.Managers
{
    public class EventManager : IEventManager
    {
        public async Task<EventResponse> CreateEventAsync(CreateEventRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteEventAsync(int communityId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EventResponse>> GetAllEventsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<EventResponse> GetEventByIdAsync(int communityId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateEventAsync(UpdateEventRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
