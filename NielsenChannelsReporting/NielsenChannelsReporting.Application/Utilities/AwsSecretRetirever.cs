using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;

namespace NielsenChannelsReporting.Application.Utilities
{
    public static class AwsSecretRetirever
    {

        public static async Task<string> GetSecretValueFromKeyAsync(string secretName, string key, string region = "us-west-2")
        {
            var secret = await GetSecretAsync(secretName, region);
            if (secret != null)
            {
                // Parse the secret value as a JSON object
                var secretObject = JsonSerializer.Deserialize<Dictionary<string, string>>(secret);

                // Retrieve the value of key
                return secretObject.TryGetValue(key, out var myDbConnectionValue)
                    ? myDbConnectionValue
                    : throw new KeyNotFoundException($"The key {key} was not found in the secret.");
            }
            else
            {
                throw new InvalidDataException("The secret value is stored as binary and cannot be retrieved as a string.");
            }
        }

        public static async Task<T> GetSecretValueFromKeyAsync<T>(string secretName, string key, string region = "us-west-2")
        {
            var serializedValue = await GetSecretValueFromKeyAsync(secretName, key, region);
            return JsonSerializer.Deserialize<T>(serializedValue);
        }

        public static async Task<string> GetSecretAsync(string secretName, string region = "us-west-2")
        {
            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new()
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified.
            };

            GetSecretValueResponse response;

            try
            {
                response = await client.GetSecretValueAsync(request);
            }
            catch (Exception e)
            {
                // For a list of the exceptions thrown, see
                // https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
                throw e;
            }

            string secret = response.SecretString;
            return secret;
            // Your code goes here
        }
    }
}
