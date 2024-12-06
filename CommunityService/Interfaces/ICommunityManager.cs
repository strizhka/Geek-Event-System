using CommunityService.Models;
using Shared.Models;

namespace CommunityService.Interfaces
{
    public interface ICommunityManager
    {
        Task<CommunityResponse> CreateCommunityAsync(CreateCommunityRequest request);
        Task<CommunityResponse> GetCommunityByIdAsync(int communityId);
        Task<IEnumerable<CommunityResponse>> GetAllCommunitiesAsync();
        Task UpdateCommunityAsync(UpdateCommunityRequest request);
        Task DeleteCommunityAsync(int communityId);
    }
}
