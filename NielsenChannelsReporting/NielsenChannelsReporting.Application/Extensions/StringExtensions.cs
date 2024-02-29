using System.Text.RegularExpressions;

namespace NielsenChannelsReporting.Application.Extensions
{
    public static class StringExtensions
    {

        public static string HideSensitiveInfo(this string text)
        {
            return text.HideEmailAddresses();
        }

        public static string HideEmailAddresses(this string text)
        {
            // Define a regular expression pattern to match email addresses
            string pattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b";

            // Replace email addresses with hidden versions
            string hiddenText = Regex.Replace(text, pattern, m =>
            {
                string email = m.Value;
                int atIndex = email.IndexOf('@');
                string username = email[..atIndex];
                string domain = email[(atIndex + 1)..];
                string hiddenUsername = MaskInfo(username);
                string hiddenDomain = MaskInfo(domain);
                return hiddenUsername + "@" + hiddenDomain;
            });

            return hiddenText;
        }

        private static string MaskInfo(string input)
        {
            if (input.Length <= 2)
            {
                return input;
            }

            char firstChar = input[0];
            char lastChar = input[input.Length - 1];
            string hiddenInfo = new string('*', input.Length - 2);

            return firstChar + hiddenInfo + lastChar;
        }
    }
}
