using System.Runtime.Serialization;

namespace NielsenChannelsReporting.Application.Exceptions
{
    [Serializable]
    public class PersistenceException : Exception
    {

        const string MESSAGE_BASE = "Error in persistence layer.";

        public PersistenceException() : base(MESSAGE_BASE)
        {
        }

        public PersistenceException(string? message) : base($"{MESSAGE_BASE} {message ?? string.Empty}")
        {
        }

        public PersistenceException(string repositoryName, string methodName, string? message) : base($"{MESSAGE_BASE} {message ?? string.Empty}")
        {
        }

        public PersistenceException(string? message, Exception? innerException) : base($"{MESSAGE_BASE} {message ?? string.Empty}", innerException)
        {
        }

        protected PersistenceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
