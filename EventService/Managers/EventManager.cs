using CommunityService.Models;
using EventService.Data;
using EventService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace EventService.Managers
{
    public class EventManager : IEventManager
    {
        private readonly EventDbContext _context;
        private readonly RabbitMqPublisher _rabbitMqPublisher;

        public EventManager(EventDbContext context, RabbitMqPublisher rabbitMqPublisher)
        {
            _context = context;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public async Task<EventResponse> CreateEventAsync(CreateEventRequest request)
        {
            var newEvent = new Event
            {
                Title = request.Title,
                Description = request.Description,
                Date = request.Date,
                Address = request.Address ?? "Don't know",
                CommunityId = request.CommunityId,
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return new EventResponse
            {
                EventId = newEvent.EventId,
                Title = newEvent.Title,
                Description = newEvent.Description,
                Date = newEvent.Date,
                Address = newEvent.Address,
                CommunityId = newEvent.CommunityId,
            };
        }

        public async Task<EventResponse> GetEventByIdAsync(int eventId)
        {
            var existingEvent = await _context.Events.FindAsync(eventId);

            if (existingEvent == null)
            {
                throw new KeyNotFoundException("Event not found.");
            }

            return new EventResponse
            {
                EventId = existingEvent.EventId,
                Title = existingEvent.Title!,
                Description = existingEvent.Description,
                Date = existingEvent.Date,
                Address = existingEvent.Address ?? "Don't know",
                CommunityId = existingEvent.CommunityId,
            };
        }

        public async Task<IEnumerable<EventResponse>> GetAllEventsAsync()
        {
            return await _context.Events
                .Select(e => new EventResponse
                {
                    EventId = e.EventId,
                    Title = e.Title!,
                    Description = e.Description,
                    Date = e.Date,
                    Address = e.Address ?? "Don't know",
                    CommunityId = e.CommunityId,
                }).ToListAsync();
        }

        public async Task UpdateEventAsync(UpdateEventRequest request)
        {
            var existingEvent = await _context.Events.FindAsync(request.EventId);

            if (existingEvent == null)
            {
                throw new KeyNotFoundException("Event not found.");
            }

            existingEvent.Title = request.Title;
            existingEvent.Description = request.Description;
            existingEvent.Date = request.Date ?? DateTime.MinValue;
            existingEvent.Address = request.Address;

            await _context.SaveChangesAsync();

            await _rabbitMqPublisher.SendEventChangeAsync(existingEvent.Title ?? "Event", " has been changed.", "");
        }

        public async Task DeleteEventAsync(int eventId)
        {
            var existingEvent = await _context.Events.FindAsync(eventId);

            if (existingEvent == null)
            {
                throw new KeyNotFoundException("Event not found.");
            }

            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();

            await _rabbitMqPublisher.SendEventChangeAsync(existingEvent.Title ?? "Event", " has been canseled.", "");
        }
    }
}
