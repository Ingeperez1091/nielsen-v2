using Microsoft.Extensions.Logging;
using NielsenChannelsReporting.Application.Abstractions.ExternalAdapters;
using NielsenChannelsReporting.Application.Models;
using RestSharp;

namespace NielsenChannelsReporting.ExternalAdapters.UniversalAPI.Client
{
    public class UniversalApiClient : AbstractRestClient, IUniversalApiClient
    {
        public UniversalApiClient(string url, ILogger<UniversalApiClient> logger, int maxRetry = 3) : base(url, logger, maxRetry)
        {
        }

        public UniversalApiClient(RestClientOptions options, ILogger<UniversalApiClient> logger, int maxRetry = 3) : base(options, logger, maxRetry)
        {
        }

        public async Task<IEnumerable<ChannelSet>> GetChannelSetsAsync(string environment)
        {
            return await GetAsync<List<ChannelSet>>($"api/ChannelSet/GetAllChannelSets?environment={environment}");
        }

        public async Task<ChannelSet> GetChannelSet(long id)
        {
            return await GetAsync<ChannelSet>($"api/ChannelSet/GetChannelSet?id={id}");
        }

        public async Task<IEnumerable<ChannelSetAlias>> GetChannelSetAliasesAsync()
        {
            return await GetAsync<IEnumerable<ChannelSetAlias>>("api/ChannelSetAlias/GetCurrent");
        }
    }
}
