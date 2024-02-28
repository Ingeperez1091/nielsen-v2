
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using NielsenChannelsReporting.Application.Abstractions;
using NielsenChannelsReporting.Application.Exceptions;
using NielsenChannelsReporting.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace NielsenChannelsReporting.Application.Mail
{
    public class AwsMailService : IAwsMailService
    {
        private readonly AmazonSimpleEmailServiceClient _client;

        public AwsMailService(AmazonSimpleEmailServiceClient client)
        {
            _client = client;
        }

        public async Task SendMailAsync(AwsMailParameters mailParameters)
        {
            try
            {
                mailParameters.Validate();
                await SendAsync(mailParameters);
            }
            catch (ValidationException ex)
            {
                throw new EmailException($"{nameof(mailParameters)} is not valid. {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new EmailException(ex.Message);
            }
        }

        private async Task SendAsync(AwsMailParameters mailParameters)
        {
            var content = new Content
            {
                Charset = "UTF-8",
                Data = mailParameters.Body
            };
            var sendRequest = new SendEmailRequest
            {
                Source = mailParameters.Sender,
                Destination = new Destination
                {
                    ToAddresses = mailParameters.ToAddresses,
                    CcAddresses = mailParameters.CcAddresses ?? new List<string>(),
                    BccAddresses = mailParameters.BccAdressess ?? new List<string>()
                },

                Message = new Message
                {
                    Subject = new Content(mailParameters.Subject),
                    Body = new Body
                    {
                        Html = mailParameters.IsHtml ? content : null,
                        Text = !mailParameters.IsHtml ? content : null,
                    }
                },
                // If you are not using a configuration set, comment
                // or remove the following line 
                //ConfigurationSetName = configSet
            };

            await _client.SendEmailAsync(sendRequest);
        }
    }
}
