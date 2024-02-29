using Amazon.Lambda.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NielsenChannelsReporting.Application;
using NielsenChannelsReporting.Application.Configuration;
using NielsenChannelsReporting.Application.Utilities;
using NielsenChannelsReporting.Infrastructure.Persistence;
using System.Diagnostics.CodeAnalysis;

namespace NielsenChannelsReporting;

#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
[LambdaStartup]
[ExcludeFromCodeCoverage]
public class Startup
{
    private readonly IConfigurationRoot Configuration;



    public Startup()
    {
        //// Example of creating the IConfiguration object and
        //// adding it to the dependency injection container.
        var builder = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
        Configuration = builder.Build();

    }

    /// <summary>
    /// Services for Lambda functions can be registered in the services dependency injection container in this method. 
    ///
    /// The services can be injected into the Lambda function through the containing type's constructor or as a
    /// parameter in the Lambda function using the FromService attribute. Services injected for the constructor have
    /// the lifetime of the Lambda compute container. Services injected as parameters are created within the scope
    /// of the function invocation.
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
        var universalConnectionString = AwsSecretRetirever.GetSecretValueFromKeyAsync("NielsenReport/Configuration", "UniversalDbConnectionString")
                                        .GetAwaiter()
                                        .GetResult();

        // Register the notification settings as a singleton
        var notificationSettings = AwsSecretRetirever.GetSecretValueFromKeyAsync<NotificationSettings>("NielsenReport/Configuration", "NotificationSettings")
                                   .GetAwaiter()
                                   .GetResult();

        // Register the report settings as a singleton
        var channelReportSettings = AwsSecretRetirever.GetSecretValueFromKeyAsync<ChannelReportSettings>("NielsenReport/Configuration", "ChannelReportSettings")
                                   .GetAwaiter()
                                   .GetResult();

        services.AddSingleton(notificationSettings);
        services.AddSingleton(channelReportSettings);

        services.AddPersistenceServices(universalConnectionString);

        services.AddApplicationServices();

        // Configure the logging services
        services.AddLogging();
        services.AddLogging(loggingBuilder =>
        {
            // Add any desired logging providers
            loggingBuilder.AddLambdaLogger();
        });

    }
}
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.