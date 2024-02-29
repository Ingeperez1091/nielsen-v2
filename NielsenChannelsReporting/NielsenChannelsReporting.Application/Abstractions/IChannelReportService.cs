using NielsenChannelsReporting.Application.Models;

namespace NielsenChannelsReporting.Application.Abstractions
{
    public interface IChannelReportService
    {
        Task<ReportGenerationResult> GenerateReportAsync();
    }
}
