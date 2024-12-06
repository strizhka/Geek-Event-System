namespace CommunityService.Models
{
    public class CreateCommunityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; } = "Community created successfully.";
    } 
}
