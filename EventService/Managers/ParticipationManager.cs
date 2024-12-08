using EventService.Data;
using EventService.Interfaces;
using EventService.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace EventService.Managers
{
    public class ParticipationManager : IParticipationManager
    {
        private readonly EventDbContext _context;

        public ParticipationManager(EventDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ParticipationResponse>> GetUserEventsAsync(int userId)
        {
            var participations = await _context.Participations
                .Where(m => m.UserId == userId)
                .Select(m => new ParticipationResponse
                {
                    UserId = m.UserId,
                    EventId = m.EventId
                }).ToListAsync();

            return participations;
        }

        public async Task<ParticipationResponse> JoinEventAsync(int userId, int eventId)
        {
            var events = await _context.Events.FindAsync(eventId);

            if (events == null)
            {
                throw new KeyNotFoundException("Event not found.");
            }

            var participation = new Participation
            {
                UserId = userId,
                EventId = eventId
            };

            _context.Participations.Add(participation);
            await _context.SaveChangesAsync();

            return new ParticipationResponse
            {
                EventId = eventId,
                UserId = userId
            };
        }

        public async Task LeaveEventAsync(int userId, int eventId)
        {
            var participation = await _context.Participations
                .FirstOrDefaultAsync(m => m.UserId == userId && m.EventId == eventId);

            if (participation == null)
            {
                throw new KeyNotFoundException("Participation not found.");
            }

            _context.Participations.Remove(participation);
            await _context.SaveChangesAsync();
        }
    }
}
