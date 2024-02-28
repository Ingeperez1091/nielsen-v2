using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Moq;
using NielsenChannelsReporting.Application.Exceptions;
using NielsenChannelsReporting.Application.Mail;
using NielsenChannelsReporting.Application.Models;
using NUnit.Framework;

namespace NielsenChannelsReporting.Tests.Mail
{
    [TestFixture]
    public class AwsMailServiceTests
    {
        [Test]
        public async Task SendEmailAsync_MustBeSuccess()
        {
            var mockEmailServiceClient = new Mock<AmazonSimpleEmailServiceClient>(RegionEndpoint.USWest2);
            var mailParameters = new AwsMailParameters()
            {
                Subject = "New email",
                ToAddresses = new List<string>() { "email@email.com" },
                Sender = "sender@email.com",
                Body = "Test",
                IsHtml = false
            };

            // Configure the mock behavior for the desired method
            mockEmailServiceClient.Setup(client => client.SendEmailAsync(
                    It.IsAny<SendEmailRequest>(),
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(new SendEmailResponse() { HttpStatusCode = System.Net.HttpStatusCode.OK });

            var objectUndertest = new AwsMailService(mockEmailServiceClient.Object);

            await objectUndertest.SendMailAsync(mailParameters);

            mockEmailServiceClient.Verify(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(),
                    It.IsAny<CancellationToken>()), Times.Once, "SendEmailAsync() in AmazonSimpleEmailServiceClient was not executed");
        }

        [Test]
        public async Task SendEmailAsync_InvalidParameters_MustTrowEmailException()
        {
            var mockEmailServiceClient = new Mock<AmazonSimpleEmailServiceClient>(RegionEndpoint.USWest2);
            var mailParameters = new AwsMailParameters()
            {
                Subject = "New email",
                Sender = "sender@email.com",
                Body = "Test",
                IsHtml = false
            };

            // Configure the mock behavior for the desired method
            mockEmailServiceClient.Setup(client => client.SendEmailAsync(
                    It.IsAny<SendEmailRequest>(),
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(new SendEmailResponse() { HttpStatusCode = System.Net.HttpStatusCode.OK });

            var objectUnderTest = new AwsMailService(mockEmailServiceClient.Object);

            await Assert.ThatAsync(async () => await objectUnderTest.SendMailAsync(mailParameters), 
                Throws.Exception.TypeOf<EmailException>()
                .With.Message.EqualTo("Error sending email. mailParameters is not valid. Validation errors: ToAddresses list must contain at least 1 element.")
                .IgnoreCase);

        }

        [Test]
        public async Task SendEmailAsync_ClientThrowsException_MustTrowEmailException()
        {
            var mockEmailServiceClient = new Mock<AmazonSimpleEmailServiceClient>(RegionEndpoint.USWest2);
            var mailParameters = new AwsMailParameters()
            {
                Subject = "New email",
                ToAddresses = new List<string>() { "email@email.com" },
                Sender = "sender@email.com",
                Body = "Test",
                IsHtml = false
            };

            // Configure the mock behavior for the desired method
            mockEmailServiceClient.Setup(client => client.SendEmailAsync(
                It.IsAny<SendEmailRequest>(),
                It.IsAny<CancellationToken>()
            ))
            .ThrowsAsync(new MailFromDomainNotVerifiedException("Message cannot be sent to a not verified address"));

            var objectUnderTest = new AwsMailService(mockEmailServiceClient.Object);

            await Assert.ThatAsync(async () => await objectUnderTest.SendMailAsync(mailParameters),
                Throws.Exception.TypeOf<EmailException>()
                .With.Message.EqualTo("Error sending email. Message cannot be sent to a not verified address")
                .IgnoreCase);

        }
    }
}
