using RestSharp;

namespace NielsenChannelsReporting.Application.Exceptions
{
    [Serializable]
    public class ClientException : Exception
    {
        public ClientException() { }

        public ClientException(string message) : base(message) { }

        public ClientException(RestResponse response) : base(GetResponseString(response)) { }

        
        public ClientException(string message, Exception inner) : base(message, inner) { }

        protected ClientException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        private static string GetResponseString
            (RestResponse response)
        {
            return $"Failed while attempting {response.Request.Method} {response.ResponseUri}. StatusCode: {(int)response.StatusCode} {response.StatusCode}. {response.ErrorMessage}";
        }

    }
}
