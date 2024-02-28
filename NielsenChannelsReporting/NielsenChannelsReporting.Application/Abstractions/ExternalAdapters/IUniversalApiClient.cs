using NielsenChannelsReporting.Application.Models;

namespace NielsenChannelsReporting.Application.Abstractions.ExternalAdapters
{
    public interface IUniversalApiClient
    {
        Task<ChannelSet> GetChannelSet(long id);
        Task<IEnumerable<ChannelSetAlias>> GetChannelSetAliasesAsync();
        Task<IEnumerable<ChannelSet>> GetChannelSetsAsync(string environment);
    }
}
