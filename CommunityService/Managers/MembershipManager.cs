using CommunityService.Data;
using CommunityService.Interfaces;
using CommunityService.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace CommunityService.Managers
{
    public class MembershipManager : IMembershipManager
    {
        private readonly CommunityDbContext _context;

        public MembershipManager(CommunityDbContext context)
        {
            _context = context;
        }

        public async Task<MembershipResponse> JoinCommunityAsync(int userId, int communityId)
        {
            var community = await _context.Communities.FindAsync(communityId);

            if (community == null)
            {
                throw new KeyNotFoundException("Community not found.");
            }

            var membership = new Membership
            {
                UserId = userId,
                CommunityId = communityId
            };

            _context.Memberships.Add(membership);
            await _context.SaveChangesAsync();

            return new MembershipResponse
            {
                CommunityId = communityId,
                UserId = userId
            };
        }

        public async Task LeaveCommunityAsync(int userId, int communityId)
        {
            var membership = await _context.Memberships
                .FirstOrDefaultAsync(m => m.UserId == userId && m.CommunityId == communityId);

            if (membership == null)
            {
                throw new KeyNotFoundException("Membership not found.");
            }

            _context.Memberships.Remove(membership);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MembershipResponse>> GetUserCommunitiesAsync(int userId)
        {
            var memberships = await _context.Memberships
                .Where(m => m.UserId == userId)
                .Select(m => new MembershipResponse
                {
                    UserId = m.UserId,
                    CommunityId = m.CommunityId
                }).ToListAsync();

            return memberships;
        }
    }

}
