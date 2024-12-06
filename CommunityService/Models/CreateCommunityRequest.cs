namespace CommunityService.Models
{
    public class CreateCommunityRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
