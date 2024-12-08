namespace CommunityService.Models
{
    public class CreateEventResponse
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public int CommunityId { get; set; }
    } 
}
