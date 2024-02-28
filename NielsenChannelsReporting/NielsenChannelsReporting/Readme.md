# Vizio Nielsen Channels Reporting Lambda

<!-- Start Document Outline -->

* [Overview](#overview)
* [Architecture](#architecture)
	* [NielsenChannelsReporting.Domain](#nielsenchannelsreportingdomain)
	* [NielsenChannelsReporting.Application](#nielsenchannelsreportingapplication)
	* [NielsenChannelsReporting.Infrastructure.Persistence](#nielsenchannelsreportinginfrastructurepersistence)
	* [NielsenChannelsReporting](#nielsenchannelsreporting)
	* [NielsenChannelsReporting.Tests](#nielsenchannelsreportingtests)
* [How to](#how-to)
	* [Update database models](#update-database-models)
	* [Test it locally](#test-it-locally)

<!-- End Document Outline -->



## Overview   

This is a simple AWS lambda solution, the main flow diagram is shown below:  

<figure>
    <img src="../../DocAssets/Flowchart%20diagram.png" alt="Select Mock Tool" style="background-color:white;">
    <figcaption>Flowchart diagram</figcaption>
</figure>
  
The stepts to generate the report are:

1. Search the last reported channel set data from NielsenChannelsReportLog and ChannelSets tables
2. Get the current channel set data calling the GetCurrentChannelSet stored procedure.
3. Get channels that where added comparing the current and the last reported channel sets.
4. Create the html formatted table with added channels data.
5. Get the html report template and replace the tokens with the respective values.
6. Send the email using the *NotificationSettings*
7. Add a new row into NielsenChannelsReportLog with the current channel set id. If there is any error when building or sending the report, the *IsSuccess* flag must be set to false and add the error information in *Detail* column.


## Architecture  
  
This solution was created following a <a href="https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#clean-architecture" target="_blank">Clean Architecture</a>. These are the projects that compose the solution:

### NielsenChannelsReporting.Domain  
  
Contains the domain or business entities that are relevant in the application, in this case it will be just the AddedChannel that will be sent in the report.

### NielsenChannelsReporting.Application

This project contains all the business rules and basically here is where the different services, entities and models interacts to process that information. It's important to follow the structure keeping in mind that the external references must not be implemented in here, this project will only have their abstractions (i.e. repositories, Units of work, external APIs, etc).    
  
Some of the most important classes in this project are:  
  
* ***AwsSecretRetirever:*** This class is responsible of getting the secrets values from the AWS secrets manager.
* ***HtmlTemplateFiller:*** This class replaces the tokens of the html template file with the values passed in the dictionary and returns the generated html string.
* ***NewChannelsReportBuilder:*** This class takes the list of channels to send in the report, creates the html table and generates the html body to be send in the email.
* ***ChannelReportService:*** This is the main class, it reads the last reported and current channel sets and gets the added channels, then, it builds and sends the report.

  
### NielsenChannelsReporting.Infrastructure.Persistence  
  
This project has the implementation of the logic that interacts with the database, here is were is defined the database context and the abstractions defined in [Application](#nielsenchannelsreportingapplication) project are implemented.  
  
The database context implements Entity Framework Core. If new models are added and the <a href="https://www.entityframeworktutorial.net/efcore/fluent-api-in-entity-framework-core.aspx#:~:text=Entity%20Framework%20Fluent%20API%20is,acts%20as%20a%20Fluent%20API." target="_blank">fluent</a> models definition increases the lines of code, then it's recommended to move the model definition code to a new file or implement <a href="https://www.entityframeworktutorial.net/code-first/dataannotation-in-code-first.aspx" target="_blank">Data Annotations</a>.
  
The connection string to database is stored in AWS Secrets Manager.  

### NielsenChannelsReporting
This is the main project and consists of:  

* serverless.template - An AWS CloudFormation Serverless Application Model template file for declaring your Serverless functions and other AWS resources
* Function.cs - Class file containing the C# method mapped to the single function declared in the template file
* Startup.cs - Class file that can be used to configure services that can be injected for either the Lambda container lifetime or a single function invocation
* aws-lambda-tools-defaults.json - Default argument settings for use with Visual Studio and command line deployment tools for AWS.


#### Lambda Annotations
This template uses [Lambda Annotations](https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.Annotations/README.md) to bridge the gap between the Lambda programming model and a more idiomatic .NET model.

This automatically handles reading parameters from an APIGatewayProxyRequest and returning an APIGatewayProxyResponse. 

It also generates the function resources in a JSON or YAML CloudFormation template based on your function definitions, and keeps them updated.

##### Using Annotations without API Gateway
You can still use Lambda Annotations without integrating with API Gateway. For example, this Lambda function processes messages from an Amazon Simple Queue Service (Amazon SQS) queue:
```
[LambdaFunction(Policies = "AWSLambdaSQSQueueExecutionRole", MemorySize = 256, Timeout = 30)]
public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context) 
{ 
    foreach(var message in evnt.Records) 
    { 
      await ProcessMessageAsync(message, context);
    }
}
```

##### Reverting to not using Annotations
If you wish to use the former style of function instead of annotations, replace the Lambda function with:
```
public APIGatewayProxyResponse Get(APIGatewayProxyRequest request, ILambdaContext context)
{
    context.Logger.LogInformation("Handling the 'Get' Request");

    var response = new APIGatewayProxyResponse
    {
        StatusCode = (int)HttpStatusCode.OK,
        Body = "Hello AWS Serverless",
        Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
    };

    return response;
}
```

You must also replace the function resource in `serverless.template` with:
```
    "Get": {
      "Type": "AWS::Serverless::Function",
      "Properties": {
        "Handler": "<ASSEMBLY>::<TYPE>.Functions::Get",
        "Runtime": "dotnet6",
        "CodeUri": "",
        "MemorySize": 256,
        "Timeout": 30,
        "Role": null,
        "Policies": [
          "AWSLambdaBasicExecutionRole"
        ],
        "Events": {
          "RootGet": {
            "Type": "Api",
            "Properties": {
              "Path": "/",
              "Method": "GET"
            }
          }
        }
      }
    }
  }
```

You may also want to:
1. Remove the package reference and `using` statements related to `Amazon.Lambda.Annotations`.


### NielsenChannelsReporting.Tests
This project contains the unit tests, please try to follow the naming convention used. For naming methods we recommend follow this practice:  
  
The name of your test should consist of three parts:

* The name of the method being tested.
* The scenario under which it's being tested.
* The expected behavior when the scenario is invoked.

You can check the code coverage running the *generate-code-coverage.bat* file. If you have any issues you can check the <a href="https://www.nuget.org/packages/ReportGenerator" target="_blank">Report Generator documentation</a>.

## How to  
  
### Update database models  

If you need to update the database model you can do it manually, using the fluent or Data Annotations model definition depending on what is implemented. As this is a model for a database that already exists and is used in other applications, the recommendation is to use the entity framework <a href="https://www.entityframeworktutorial.net/efcore/cli-commands-for-ef-core-migration.aspx#dbcontext-scaffold" target="_blank">scaffold command</a> that will map the database schema into entity type classes, the **-t** option is used to specify the tables you want to include in the model.  
  
Other recommended option is to use the **-o** to specify the folder that will contain the new model and then replace the existing model(s) and update the database context.
  
  
### Test it locally
  
You must have the **aws-smartcast-data-dev** profile configured in your aws credentials file.


The project already have the dependencies required to test the function, in Visual Studio select the NielsenChannelsReporting project and the Mock Lambda Test Tool:  

<figure>
    <img src="../../DocAssets/Select%20Mock%20Tool.png" alt="Select Mock Tool" style="background-color:white;">
    <figcaption>Select Mock Tool</figcaption>
</figure>
  
A new window of your default browser will be opened:  

<figure>
    <img src="../../DocAssets/Mock%20Lambda%20Test%20Tool.png" alt="Select Mock Tool" style="background-color:white;">
    <figcaption>Lambda Mock Tool Window</figcaption>
</figure>

  
Select the AWS Credential Profile corresponding to the **aws-smartcast-data-dev** account and click on Execute Function button, this will run the Lambda in debug mode.  
  
:warning:***Important:*** Make sure you are running the solution using the **dev** account, you can also update the Startup class in the *NielsenChannelsReporting* and change the code to read the different settings from the *appsettings.json* file instead of reading them from secrets manager.
 