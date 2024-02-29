namespace NielsenChannelsReporting.Application.Configuration
{
    public class NotificationSettings
    {
        public List<string> DestinationEmails { get; set; }
        public string Subject { get; set; }
        public List<string> BCC_DestinationEmails { get; set; }
        public string Sender { get; set; }
    }
}
