using System.Runtime.Serialization;

namespace NielsenChannelsReporting.Application.Exceptions
{
    [Serializable]
    public class ReportBuilderException : Exception
    {
        const string MESSAGE_BASE = "Error generating report.";

        public ReportBuilderException(): base(MESSAGE_BASE)
        {
        }

        public ReportBuilderException(string? message, string reportName) : base($"{MESSAGE_BASE} {reportName}. {message ?? string.Empty}")
        {
        }

        public ReportBuilderException(string? message, Exception? innerException) : base($"{MESSAGE_BASE} {message ?? string.Empty}", innerException)
        {
        }

        protected ReportBuilderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
