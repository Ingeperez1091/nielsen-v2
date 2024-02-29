using NielsenChannelsReporting.Application.Abstractions;
using NielsenChannelsReporting.Application.Exceptions;
using NielsenChannelsReporting.Application.Models;
using NielsenChannelsReporting.Application.Utilities;
using System.Text;

namespace NielsenChannelsReporting.Application.Report
{
    public class NewChannelsReportBuilder : INewChannelsReportBuilder
    {
        private const string NEW_CHANNELS_HTML_TEMPLATE_PATH = "Templates/NielsenNewChannelsTemplate.html";
        public NewChannelsReportBuilder()
        {

        }

        public string GetFormattedReport(DateTime startDate, DateTime endDate, IEnumerable<Channel> newChannels)
        {
            try
            {
                string formattedChannels = GetFormattedChannelsList(newChannels.ToList());

                Dictionary<string, string> values = new()
                {
                    {"[ChannelsRows]", formattedChannels},
                    {"[StartDate]", $"{startDate:yyyy-MM-dd}"},
                    {"[EndDate]", $"{endDate.Date:yyyy-MM-dd}"},
                };

                string templatePath = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}/{NEW_CHANNELS_HTML_TEMPLATE_PATH}";
                string emailBody = HtmlTemplateFiller.BuildEmailBodyFromHtmlTemplate(values, templatePath);
                return emailBody;

            }
            catch (Exception ex)
            {
                throw new ReportBuilderException(ex.Message, nameof(NewChannelsReportBuilder));
            }

        }

        private static string GetFormattedChannelsList(List<Channel> channels)
        {
            StringBuilder sb = new();

            for (int i = 0; i < channels.Count; i++)
            {
                var channel = channels[i];
                string rowStyle = i % 2 == 0 ? "tg-evenRow" : "tg-oddRow";
                string row = GetChannelAsTableRow(channel, rowStyle);
                sb.AppendLine(row);
            }

            return sb.ToString();
        }

        private static string GetChannelAsTableRow(Channel channel, string rowStyle)
        {
            string[] columns = new string[] {
                    channel.ChannelKey,
                    channel.Name,
                    $"{channel.StreamStartDate:yyyy-MM-dd HH:mm:ss}",
                    $"{channel.StreamEndDate:yyyy-MM-dd HH:mm:ss}",
                    channel.StationId.ToString(),
                    channel.SourceKey };

            StringBuilder rowSb = new();
            foreach (var column in columns)
            {
                rowSb.Append($"<td class=\"{rowStyle}\">{column}</td> ");
            }
            string row = $"<tr>{rowSb} </tr>";
            return row;
        }
    }
}
