using NielsenChannelsReporting.Application.Extensions;
using NUnit.Framework;

namespace NielsenChannelsReporting.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionTests
    {
        [Test]
        public void HideSensitiveInfo_StringWithEmail_MustMaskEmail()
        {
            string email = "name.lastname@domain.com";
            string input = $"The message could not be delivereds to address {email}";
            string expectedOutput = "The message could not be delivereds to address n***********e@d********m";
            var result = input.HideSensitiveInfo();

            Assert.Multiple(() =>
            {
                Assert.That(!result.Contains(email, StringComparison.InvariantCultureIgnoreCase));
                Assert.That(result.Equals(expectedOutput, StringComparison.InvariantCultureIgnoreCase));
            });
        }
    }
}
