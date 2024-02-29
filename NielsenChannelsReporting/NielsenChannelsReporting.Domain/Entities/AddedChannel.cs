namespace NielsenChannelsReporting.Domain.Entities
{
    public class AddedChannel
    {
        public AddedChannel()
        {

        }

        public string? ChannelKey { get; set; }
        public string? ChannelName { get; set; }
        public string? StreamStart { get; set; }
        public string? StreamEnd { get; set; }
        public string? StationId { get; set; }
        public string? SourceKey { get; set; }
    }
}
