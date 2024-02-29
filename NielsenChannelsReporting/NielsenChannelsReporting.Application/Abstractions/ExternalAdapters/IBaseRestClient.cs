namespace NielsenChannelsReporting.Application.Abstractions.ExternalAdapters
{
    internal interface IBaseRestClient
    {
        Task DeleteAsync(string route);
        Task<T> GetAsync<T>(string route, params KeyValuePair<string, object>[] parameters);
        Task<T> PostAsync<T>(string route, object body);
        Task<T> PutAsync<T>(string route, object body);
    }
}
