using System.Text;

namespace NielsenChannelsReporting.Application.Utilities
{
    public static class HtmlTemplateFiller
    {
        public static string BuildEmailBodyFromHtmlTemplate(Dictionary<string, string> keyValues, string htmlTemplatePath)
        {
            if (string.IsNullOrEmpty(htmlTemplatePath))
            {
                throw new ArgumentException($"'{nameof(htmlTemplatePath)}' cannot be null or empty.", nameof(htmlTemplatePath));
            }

            StringBuilder result = new(GetTemplate(htmlTemplatePath));

            foreach (var keyPair in keyValues)
            {
                result.Replace(keyPair.Key, keyPair.Value);
            }

            return result.ToString();
        }

        private static string GetTemplate(string templatePath)
        {
            return File.ReadAllText(templatePath);
        }
    }
}
