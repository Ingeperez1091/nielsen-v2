using NielsenChannelsReporting.Application.Models;

namespace NielsenChannelsReporting.Application.Abstractions
{
    public interface INewChannelsReportBuilder
    {
        string GetFormattedReport(DateTime startDate, DateTime endDate, IEnumerable<Channel> newChannels);
    }
}