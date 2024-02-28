namespace NielsenChannelsReporting.Application.Models
{
    public class Channel
    {
        public Guid ChannelConfigurationId { get; set; }
        public long StationId { get; set; }
        public string ChannelKey { get; set; }
        public bool IsEpgEnabled { get; set; }
        public int? SourceId { get; set; }
        public string SourceKey { get; set; }
        public DateTime StreamStartDate { get; set; }
        public DateTime StreamEndDate { get; set; }
        public string Name { get; set; }
        public int ChannelNumber { get; set; }
        public int ChannelPriority { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public long? ChannelIcon { get; set; }
        public long? WhiteLogo { get; set; }
        public string ChannelUrl { get; set; }
        public long? GracenoteChannelId { get; set; }
        public string LicenseUrl { get; set; }
        public string WurlDrmTokenUrl { get; set; }
        public List<string> AlternateTitles { get; set; } = new List<string>();
        public bool IsEnabled { get; set; }
        public string TokenUrl { get; set; }
        public long StreamingChannelId { get; set; }
        public int ParentalControlLevel { get; set; } = -1;
        public long? HeroImageId { get; set; }
        public long? TitleArtImageId { get; set; }
        public long? DiscoverImageId { get; set; }
        public bool UseLocalization { get; set; }
    }
}
