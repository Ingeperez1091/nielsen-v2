using NielsenChannelsReporting.Application.Models;

namespace NielsenChannelsReporting.Application.Abstractions
{
    public interface IAwsMailService
    {
        Task SendMailAsync(AwsMailParameters mailParameters);
    }
}
