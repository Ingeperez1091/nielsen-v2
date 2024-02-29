using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using NielsenChannelsReporting.Application.Abstractions;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NielsenChannelsReporting.Application.Extensions;
using System.Diagnostics.CodeAnalysis;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NielsenChannelsReporting;
[ExcludeFromCodeCoverage]
public class Functions
{


    private readonly IChannelReportService _channelReportService;

    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>
    public Functions()
    {
        var serviceCollection = new ServiceCollection();
        var startUp = new Startup();
        startUp.ConfigureServices(serviceCollection);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        _channelReportService = serviceProvider.GetService<IChannelReportService>();
    }

    /// <summary>
    /// A Lambda function to respond to HTTP Get methods from API Gateway
    /// </summary>
    /// <remarks>
    /// This uses the <see href="https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.Annotations/README.md">Lambda Annotations</see> 
    /// programming model to bridge the gap between the Lambda programming model and a more idiomatic .NET model.
    /// 
    /// This automatically handles reading parameters from an APIGatewayProxyRequest
    /// as well as syncing the function definitions to serverless.template each time you build.
    /// 
    /// If you do not wish to use this model and need to manipulate the API Gateway 
    /// objects directly, see the accompanying Readme.md for instructions.
    /// </remarks>
    /// <param name="context">Information about the invocation, function, and execution environment</param>
    /// <returns>The response as an implicit <see cref="APIGatewayProxyResponse"/></returns>
    [LambdaFunction(Policies = "AWSLambdaBasicExecutionRole", MemorySize = 256, Timeout = 30)]
    [RestApi(LambdaHttpMethod.Post, "/")]
    public async Task<APIGatewayProxyResponse> GenerateReportAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            int statusCode = 200;
            context.Logger.LogInformation("Handling the 'Post' Request");

            var result = await _channelReportService.GenerateReportAsync();

            context.Logger.LogInformation("Report Generated");


            if (!result.IsSuccess)
            {
                statusCode = 500;
                context.Logger.LogError($"Error generating Report: {result.Error}");
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = JsonSerializer.Serialize(result)
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error handling the 'POST/GenerateReport' Request {ex.Message.HideSensitiveInfo()}");
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = $"Error executing function. {ex.Message.HideSensitiveInfo()}"
            };
        }

    }
}
