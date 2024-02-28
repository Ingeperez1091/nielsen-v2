using NielsenChannelsReporting.Application.Models;
using NielsenChannelsReporting.Application.Report;
using NUnit.Framework;

namespace NielsenChannelsReporting.Tests.Report
{
    [TestFixture]
    public class NewChannelsReportBuilderTests
    {
        [Test]
        public void GenerateReport_MustMatch_OneChannelAddedReport()
        {
            var objectUnderTest = new NewChannelsReportBuilder();
            DateTime start = new(2023, 10, 1);
            DateTime end = new(2023, 10, 8);
            var channels = new List<Channel>() { new Channel() {
                ChannelKey ="ChannelKey1",
                Name = "Channel1",
                StreamStartDate = new DateTime(2023, 10, 5),
                StreamEndDate = new DateTime(2030, 10, 5),
                StationId = 123456,
                SourceKey = "SourceKey1"},
            };
            var expectedResult = GetExpectedReportString("OneChannelAddedReport.html");
            var result = objectUnderTest.GetFormattedReport(start, end, channels);


            Assert.That(result.Equals(expectedResult, StringComparison.InvariantCultureIgnoreCase));

        }


        private static string GetExpectedReportString(string fileName)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "MockFiles", "Reports", fileName);
            var reportString = File.ReadAllText(filePath);
            return reportString;
        }
    }
}
