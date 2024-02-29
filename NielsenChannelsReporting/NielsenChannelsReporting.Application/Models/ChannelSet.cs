namespace NielsenChannelsReporting.Application.Models
{
    public class ChannelSet
    {
        public long Id { get; set; } = -1;
        public string? Environment { get; set; }
        public string? Name { get; set; }
        public bool Locked { get; set; }
        public List<Channel> Channels { get; set; } = new List<Channel>();
        public DateTime CreatedDate { get; set; }
        public DateTime? PublishedDate { get; set; }
    }
}
