using EventService.Models;

namespace EventService.Interfaces
{
    public interface IParticipationManager
    {
        Task<ParticipationResponse> JoinEventAsync(int userId, int eventId);
        Task LeaveEventAsync(int userId, int eventId);
        Task<IEnumerable<ParticipationResponse>> GetUserEventsAsync(int userId);
    }
}
