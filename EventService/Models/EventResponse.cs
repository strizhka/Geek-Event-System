namespace CommunityService.Models
{
    public class EventResponse
    {
        public int EventId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public required string Address { get; set; }
        public int CommunityId { get; set; }
    }

}
