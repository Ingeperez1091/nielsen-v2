using NielsenChannelsReporting.Application.Models;

namespace NielsenChannelsReporting.Application.Abstractions
{
    public interface IChannelSetRepository
    {
        Task<ChannelSet> GetChannelSetAsync(long channelId);

        Task<ChannelSet> GetCurrentChannelSet(Configuration.ChannelReportSettings _reportSettings);
    }
}
