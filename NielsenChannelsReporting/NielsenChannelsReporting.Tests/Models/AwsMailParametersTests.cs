using NielsenChannelsReporting.Application.Models;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace NielsenChannelsReporting.Tests.Models
{
    [TestFixture]
    public class AwsMailParametersTests
    {

        [Test]
        public void Validate_EmptySender_MustThrowException()
        {
            AwsMailParameters objectUnderTest = new()
            {
                ToAddresses = new List<string> { "email@email.com" },
                Subject = "New email"
            };

            Assert.That(() => objectUnderTest.Validate(),
                Throws.Exception.TypeOf<ValidationException>()
                .With.Message.EqualTo("Validation errors: Sender must be a valid email address.")
                .IgnoreCase);
        }

        [Test]
        public void Validate_EmptySubject_MustThrowException()
        {
            AwsMailParameters objectUnderTest = new()
            {
                ToAddresses = new List<string> { "email@email.com" },
                Sender = "sender@email.com"
            };

            Assert.That(() => objectUnderTest.Validate(),
                Throws.Exception.TypeOf<ValidationException>()
                .With.Message.EqualTo("Validation errors: Subject must not be null or empty.")
                .IgnoreCase);
        }

        [Test]
        public void Validate_EmptyToAddresses_MustThrowException()
        {
            AwsMailParameters objectUnderTest = new()
            {
                ToAddresses = new List<string> { },
                Sender = "sender@email.com",
                Subject = "New email"
            };

            Assert.That(() => objectUnderTest.Validate(),
                Throws.Exception.TypeOf<ValidationException>()
                .With.Message.EqualTo("Validation errors: ToAddresses list must contain at least 1 element.")
                .IgnoreCase);
        }


        [Test]
        public void Validate_MustBeSucces()
        {
            AwsMailParameters objectUnderTest = new()
            {
                ToAddresses = new List<string> { "email@email.com" },
                Sender = "sender@email.com",
                Subject = "New email"
            };

            ValidationContext context = new(objectUnderTest, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(objectUnderTest, context, validationResults, validateAllProperties: true);
            Assert.That(isValid, Is.True);
        }
    }
}
