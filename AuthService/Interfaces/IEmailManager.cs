namespace AuthService.Interfaces
{
    public interface IEmailManager
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
