using InterviewReportApp.Core.Application.Services;
using InterviewReportApp.Core.Application.Services.Interfaces;
using InterviewReportApp.Core.Domain.Interview;
using InterviewReportApp.Infrastructure;
using InterviewReportApp.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register services
        services.AddTransient<IInterviewReportService, InterviewReportService>();
        services.AddTransient<IIntentService, IntentService>();
        services.AddTransient<IQuestionRepository, QuestionRepository>();
        services.AddSingleton<IChatCompletionService>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var apiKey = configuration["SemanticKernel:ApiKey"];
            var apiUrl = configuration["SemanticKernel:ApiUrl"];
            var chatDeploymentName = configuration["SemanticKernel:ChatDeploymentName"];
            return new AzureOpenAIChatCompletionService(chatDeploymentName!, apiUrl!, apiKey!);
        });

        services.AddKeyedTransient<Kernel>("InterviewReportKernel", (sp, key) =>
        {
            // Create a collection of plugins that the kernel will use
            KernelPluginCollection pluginCollection = new();
            return new Kernel(sp, pluginCollection);
        });

        // Add enterprise components
        services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));
    })
    .Build();

// Get the service and use it
var intentService = host.Services.GetRequiredService<IIntentService>();

Console.WriteLine("I am an AI Interview Assistant. You can send notes to create a report or ask for questions.");

string? input = string.Empty;
InterviewRouterResponse result = new();

do
{
    input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    result = await intentService.RouteIntent(input, new CancellationToken());

    if (result.Questions.Count > 0)
    {
        Console.WriteLine("Questions:");
        foreach (var question in result.Questions)
        {
            Console.WriteLine(question);
        }
    }

    if (!string.IsNullOrEmpty(result.Report))
    {
        Console.WriteLine(result.Report);
    }
}
while (!string.IsNullOrWhiteSpace(input));