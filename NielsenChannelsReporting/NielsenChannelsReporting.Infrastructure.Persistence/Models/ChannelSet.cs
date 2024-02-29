namespace NielsenChannelsReporting.Infrastructure.Persistence.Models
{
    public partial class ChannelSet
    {
        public long Id { get; set; }
        public string Environment { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool Locked { get; set; }
        public string? Channels { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? Publisheddate { get; set; }
    }
}
