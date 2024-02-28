using NielsenChannelsReporting.Application.Models;

namespace NielsenChannelsReporting.Application.Abstractions
{
    public interface INielsenChannelsReportLogRepository
    {
        Task<NielsenChannelsReportLog> GetByIdAsync(long Id);

        Task<NielsenChannelsReportLog> AddAsync(NielsenChannelsReportLog reportLog);

        Task<NielsenChannelsReportLog> GetLastSuccessReportAsync(string environment);
    }
}
