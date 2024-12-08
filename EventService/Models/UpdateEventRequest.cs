using System.ComponentModel.DataAnnotations;

namespace CommunityService.Models
{
    public class UpdateEventRequest
    {
        public int EventId { get; set; }
        [StringLength(50)]
        public string? Title { get; set; }
        [StringLength(200)]
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public string? Address { get; set; }
    }
}
