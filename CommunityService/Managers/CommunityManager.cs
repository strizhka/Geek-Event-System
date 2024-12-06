using CommunityService.Data;
using CommunityService.Interfaces;
using CommunityService.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace CommunityService.Managers
{
    public class CommunityManager : ICommunityManager
    {
        private readonly CommunityDbContext _context;

        public CommunityManager(CommunityDbContext context)
        {
            _context = context;
        }

        public async Task<CommunityResponse> CreateCommunityAsync(CreateCommunityRequest request)
        {
            var community = new Community
            {
                Name = request.Name,
                Description = request.Description,
            };

            _context.Communities.Add(community);
            await _context.SaveChangesAsync();

            return new CommunityResponse
            {
                Id = community.CommunityId,
                Name = community.Name,
                Description = community.Description,
            };
        }

        public async Task<CommunityResponse> GetCommunityByIdAsync(int communityId)
        {
            var community = await _context.Communities.FindAsync(communityId);

            if (community == null)
            {
                throw new KeyNotFoundException("Community not found.");
            }

            return new CommunityResponse
            {
                Id = community.CommunityId,
                Name = community.Name,
                Description = community.Description,
            };
        }

        public async Task<IEnumerable<CommunityResponse>> GetAllCommunitiesAsync()
        {
            return await _context.Communities
                .Select(c => new CommunityResponse
                {
                    Id = c.CommunityId,
                    Name = c.Name,
                    Description = c.Description,
                }).ToListAsync();
        }

        public async Task UpdateCommunityAsync(UpdateCommunityRequest request)
        {
            var community = await _context.Communities.FindAsync(request.Id);

            if (community == null)
            {
                throw new KeyNotFoundException("Community not found.");
            }

            community.Name = request.Name;
            community.Description = request.Description;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommunityAsync(int communityId)
        {
            var community = await _context.Communities.FindAsync(communityId);

            if (community == null)
            {
                throw new KeyNotFoundException("Community not found.");
            }

            _context.Communities.Remove(community);
            await _context.SaveChangesAsync();
        }
    }

}
