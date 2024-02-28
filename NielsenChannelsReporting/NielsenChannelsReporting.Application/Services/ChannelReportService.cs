using Microsoft.Extensions.Logging;
using NielsenChannelsReporting.Application.Abstractions;
using NielsenChannelsReporting.Application.Configuration;
using NielsenChannelsReporting.Application.Extensions;
using NielsenChannelsReporting.Application.Models;

namespace NielsenChannelsReporting.Application.Services
{

    public class ChannelReportService : IChannelReportService
    {
        private readonly IAwsMailService _awsMailService;
        private readonly IUniversalUnitOfWork _universalUnitOfWork;
        private readonly INewChannelsReportBuilder _newChannelsReportBuilder;
        private readonly NotificationSettings _notificationSettings;
        private readonly ChannelReportSettings _reportSettings;
        private readonly ILogger<ChannelReportService> _logger;

        public ChannelReportService(IAwsMailService awsMailService,
            IUniversalUnitOfWork universalUnitOfWork,
            INewChannelsReportBuilder newChannelsReportBuilder,
            NotificationSettings notificationSettings,
            ChannelReportSettings reportSettings,
            ILogger<ChannelReportService> logger)
        {
            _awsMailService = awsMailService;
            _universalUnitOfWork = universalUnitOfWork;
            _newChannelsReportBuilder = newChannelsReportBuilder;
            _notificationSettings = notificationSettings;
            _reportSettings = reportSettings;
            _logger = logger;
        }

        static string ChannelIdSelector(Channel channel) => channel.ChannelKey;

        public async Task<ReportGenerationResult> GenerateReportAsync()
        {
            NielsenChannelsReportLog log = new()
            {
                IsSuccess = true,
                ProcessDate = DateTime.UtcNow,
            };

            try
            {
                _logger.LogInformation("Getting new channels");
                var lastSuccessReport = await _universalUnitOfWork.NielsenChannelsReportLogRepository.GetLastSuccessReportAsync(_reportSettings.Environment);
                var lastReportedChannelSet = await _universalUnitOfWork.ChannelSetRepository.GetChannelSetAsync(lastSuccessReport?.ChannelSetId ?? 0);
                lastReportedChannelSet ??= new ChannelSet();

                var currentChannelSet = await _universalUnitOfWork.ChannelSetRepository.GetCurrentChannelSet(_reportSettings);

                DateTime lastSuccesReportDate = lastSuccessReport?.ProcessDate ?? 
                                                currentChannelSet.PublishedDate ?? 
                                                DateTime.UtcNow.AddDays(-7);

                IEnumerable<Channel> newChannels = GetNewChannels(lastReportedChannelSet, currentChannelSet);
                if (newChannels.Any())
                {
                    _logger.LogInformation("Creating report");
                    DateTime generationDate = DateTime.UtcNow.Date;
                    string emailBody = _newChannelsReportBuilder.GetFormattedReport(lastSuccesReportDate, generationDate, newChannels);
                    _logger.LogInformation("Sending report");
                    await SendReport(emailBody);
                }
                else
                {
                    log.Detail = "Not new channels found.";
                }

                log.ChannelSetId = currentChannelSet.Id;
                log.Environment = _reportSettings.Environment;

            }
            catch (Exception ex)
            {
                log.Detail = ex.Message.HideSensitiveInfo();
                _logger.LogError(ex, $"Error {ex.Message.HideSensitiveInfo()}");
                log.IsSuccess = false;
            }
            finally
            {
                await _universalUnitOfWork.NielsenChannelsReportLogRepository.AddAsync(log);
                await _universalUnitOfWork.CommitAsync();
            }
            return new ReportGenerationResult() { IsSuccess = log.IsSuccess, Error = log.Detail ?? string.Empty };
        }

        private async Task SendReport(string emailBody)
        {
            AwsMailParameters mailParameters = new()
            {
                Sender = _notificationSettings.Sender,
                ToAddresses = _notificationSettings.DestinationEmails,
                BccAdressess = _notificationSettings.BCC_DestinationEmails,
                IsHtml = true,
                Body = emailBody,
                Subject = _notificationSettings.Subject
            };

            await _awsMailService.SendMailAsync(mailParameters);
        }

        private static IEnumerable<Channel> GetNewChannels(ChannelSet lastReportedChannelSet, ChannelSet currentChannelSet)
        {
            var lastReportedChannels = lastReportedChannelSet.Channels.ToList();
            var currentChannels = currentChannelSet.Channels.ToList();

            var newChannels = currentChannels.ExceptBy(lastReportedChannels.Select(ChannelIdSelector), ChannelIdSelector);
            return newChannels;
        }
    }
}
