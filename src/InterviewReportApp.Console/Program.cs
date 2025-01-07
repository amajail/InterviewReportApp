using InterviewReportApp.Core.Application.Services;
using InterviewReportApp.Core.Application.Services.Interfaces;
using InterviewReportApp.Infrastructure;
using InterviewReportApp.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    })
    .Build();

// Get the service and use it
var reportGenerator = host.Services.GetRequiredService<IInterviewReportService>();
var result = await reportGenerator.GenerateReportAsync(string.Empty, new CancellationToken());

Console.WriteLine(result);


