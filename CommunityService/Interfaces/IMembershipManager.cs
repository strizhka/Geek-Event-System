using CommunityService.Models;

namespace CommunityService.Interfaces
{
    public interface IMembershipManager
    {
        Task<MembershipResponse> JoinCommunityAsync(int userId, int communityId);
        Task LeaveCommunityAsync(int userId, int communityId);
        Task<IEnumerable<MembershipResponse>> GetUserCommunitiesAsync(int userId);
    }
}
