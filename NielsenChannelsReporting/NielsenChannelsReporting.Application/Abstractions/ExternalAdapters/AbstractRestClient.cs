using Microsoft.Extensions.Logging;
using NielsenChannelsReporting.Application.Exceptions;
using Polly;
using Polly.Retry;
using RestSharp;
using System.Net;

namespace NielsenChannelsReporting.Application.Abstractions.ExternalAdapters
{
    public abstract class AbstractRestClient : IBaseRestClient
    {
        #region properties
        protected readonly RestClient _restClient;
        private AsyncRetryPolicy<RestResponse>? _retryPolicyAsync;
        private RetryPolicy<RestResponse>? _retryPolicy;
        private readonly int _maxRetryCount;
        private readonly ILogger<AbstractRestClient> _logger;
        #endregion

        #region Constructors
        protected AbstractRestClient(string url, ILogger<AbstractRestClient> logger, int maxRetry = 3)
        {
            _restClient = new RestClient(url);
            _maxRetryCount = maxRetry;
            _logger = logger;
            ConfigureRetryPolicy();

        }

        protected AbstractRestClient(RestClientOptions options, ILogger<AbstractRestClient> logger, int maxRetry = 3)
        {
            _restClient = new RestClient(options);
            _maxRetryCount = maxRetry;
            _logger = logger;
            ConfigureRetryPolicy();
        }
        #endregion

        private void ConfigureRetryPolicy()
        {
            _retryPolicyAsync = Policy
               .HandleResult<RestResponse>(r =>
               {
                   var intStatusCode = (int)r.StatusCode;
                   return (intStatusCode >= 500 && intStatusCode <= 599);
               })
               .WaitAndRetryAsync(
                   _maxRetryCount,
                   attempt =>
                   {
                       return TimeSpan.FromSeconds(Math.Pow(2, attempt));
                   },
                   (response, time, retryCount, context) =>
                   {
                       _logger.LogWarning($"REST request failed with StatusCode {response.Result.StatusCode} on attempt {retryCount}." +
                          $"Trying again after {time}. ResponseUri: {response.Result.ResponseUri}");
                   }
               );


            _retryPolicy = Policy
                .HandleResult<RestResponse>(r =>
                {
                    var intStatusCode = (int)r.StatusCode;
                    return (intStatusCode >= 500 && intStatusCode <= 599);
                })
                .WaitAndRetry(
                    _maxRetryCount,
                    attempt =>
                    {
                        return TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    },
                    (response, time, retryCount, context) =>
                    {
                        _logger.LogWarning($"REST request failed with StatusCode {response.Result.StatusCode} on attempt {retryCount}." +
                          $"Trying again after {time}. ResponseUri: {response.Result.ResponseUri}");
                    }
                );
        }

        #region Get Methods
        public async Task<T> GetAsync<T>(string route, params KeyValuePair<string, object>[] parameters)
        {
            var request = new RestRequest(route, Method.Get);

            foreach (var parameter in parameters)
            {
                request.AddQueryParameter(parameter.Key, parameter.Value?.ToString());
            }

            var response = await _retryPolicyAsync.ExecuteAsync(async () =>
            {
                return await _restClient.ExecuteAsync<T>(request);
            });

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ClientException(response);
            }

            var castedResponse = (RestResponse<T>)response;

            return castedResponse.Data;
        }
        #endregion

        #region Post Methods
        public async Task<T> PostAsync<T>(string route, object body)
        {
            var request = new RestRequest(route, Method.Post);
            request.AddJsonBody(body);

            var response = await _retryPolicyAsync.ExecuteAsync(async () =>
            {
                return await _restClient.ExecuteAsync<T>(request);
            });

            if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
            {
                throw new ClientException(response);
            }

            var castedResponse = (RestResponse<T>)response;

            return castedResponse.Data;
        }

        #endregion

        #region Put methods

        public async Task<T> PutAsync<T>(string route, object body)
        {
            var request = new RestRequest(route, Method.Put);
            request.AddJsonBody(body);

            var response = await _retryPolicyAsync.ExecuteAsync(async () =>
            {
                return await _restClient.ExecuteAsync<T>(request);
            });

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ClientException(response);
            }

            var castedResponse = (RestResponse<T>)response;

            return castedResponse.Data;
        }

        #endregion

        #region Delete Methods
        public async Task DeleteAsync(string route)
        {
            var request = new RestRequest(route, Method.Delete);

            var response = await _retryPolicyAsync.ExecuteAsync(async () =>
            {
                return await _restClient.ExecuteAsync(request);
            });

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ClientException(response);
            }
        }
        #endregion



    }
}
