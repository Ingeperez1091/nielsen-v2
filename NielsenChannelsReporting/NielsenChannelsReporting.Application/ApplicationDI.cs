using Amazon.SimpleEmail;
using Microsoft.Extensions.DependencyInjection;
using NielsenChannelsReporting.Application.Abstractions;
using NielsenChannelsReporting.Application.Mail;
using NielsenChannelsReporting.Application.Report;
using NielsenChannelsReporting.Application.Services;

namespace NielsenChannelsReporting.Application
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<AmazonSimpleEmailServiceClient>();
            services.AddScoped<IChannelReportService, ChannelReportService>();
            services.AddScoped<IAwsMailService, AwsMailService>();
            services.AddScoped<INewChannelsReportBuilder, NewChannelsReportBuilder>();
            return services;
        }
    }
}
