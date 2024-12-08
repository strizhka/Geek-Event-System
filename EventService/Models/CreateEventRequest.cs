using System.ComponentModel.DataAnnotations;

namespace CommunityService.Models
{
    public class CreateEventRequest
    {
        [StringLength(50)]
        public required string Title { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }
        public required DateTime Date { get; set; }

        public required string Address { get; set; }

        public required int CommunityId { get; set; }
    }

}
