namespace NielsenChannelsReporting.Application.Models
{
    public class ChannelSetAlias
    {
        public long Id { get; set; }
        public long? ChannelSetId { get; set; }
        public string ScConfigName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
