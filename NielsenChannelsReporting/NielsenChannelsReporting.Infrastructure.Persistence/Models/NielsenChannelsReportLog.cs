namespace NielsenChannelsReporting.Infrastructure.Persistence.Models
{
    public partial class NielsenChannelsReportLog
    {
        public long Id { get; set; }
        public long? ChannelSetId { get; set; }
        public DateTime ProcessDate { get; set; }
        public bool IsSuccess { get; set; }
        public string? Detail { get; set; }
        public string Environment { get; set; }
    }
}
