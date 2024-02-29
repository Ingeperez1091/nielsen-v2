namespace NielsenChannelsReporting.Application.Abstractions
{
    public interface IUniversalUnitOfWork
    {
        INielsenChannelsReportLogRepository NielsenChannelsReportLogRepository { get; }

        IChannelSetRepository ChannelSetRepository { get; }

        /// <summary>
        /// Commits changes in context
        /// </summary>
        /// <returns></returns>
        int Commit();

        /// <summary>
        /// Commits cahnges in context asynchronously
        /// </summary>
        /// <returns></returns>
        Task<int> CommitAsync();
    }
}
