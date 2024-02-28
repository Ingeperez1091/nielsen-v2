using System.Runtime.Serialization;

namespace NielsenChannelsReporting.Application.Exceptions
{
    [Serializable]
    public class EmailException : Exception
    {
        const string MESSAGE_BASE = "Error sending email.";
        public EmailException() : base(MESSAGE_BASE)
        {
        }

        public EmailException(string? message) : base($"{MESSAGE_BASE} {message ?? string.Empty}")
        {
        }

        public EmailException(string? message, Exception? innerException) : base($"{MESSAGE_BASE} {message ?? string.Empty}", innerException)
        {
        }

        protected EmailException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
