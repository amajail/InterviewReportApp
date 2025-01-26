// See https://aka.ms/new-console-template for more information
using InterviewReportApp.Infrastructure;
using InterviewReportApp.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;


var kernel = CreateKernelWithChatCompletion();

OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();

kernel.Plugins.AddFromType<QuestionQuery>();
kernel.Plugins.AddFromType<CategoryQuery>();

history.AddSystemMessage(
    @"You are a helpful interviewer assistant for a software engineer position. 
    I will ask you for questions to ask to the interviewee.
    You need to check if the category is on the list of possible categories.
    You can get questions for a specific category using a plugin.
    If you dont find questions from that category, you can create them.");

string input;

do
{
    Console.WriteLine("What would you like to do?");
    input = Console.ReadLine()!;

    AddUserMessage(input);

    await GetReply();
}
while (!string.IsNullOrWhiteSpace(input));

static Kernel CreateKernelWithChatCompletion()
{

    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    var apiKey = configuration["SemanticKernel:ApiKey"];
    var apiUrl = configuration["SemanticKernel:ApiUrl"];
    var chatDeploymentName = configuration["SemanticKernel:ChatDeploymentName"];

    var builder = Kernel.CreateBuilder();
    builder.Services.AddAzureOpenAIChatCompletion(
        chatDeploymentName,
        apiUrl,
        apiKey,
        "gpt-4o");

    builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));
    builder.Services.AddTransient<IQuestionRepository, QuestionRepository>();

    return builder.Build();
}

async Task GetReply()
{
    ChatMessageContent reply = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel
    );

    Console.WriteLine("Assistant: " + reply.ToString());
    history.AddAssistantMessage(reply.ToString());
}

void AddUserMessage(string msg)
{
    Console.WriteLine("User: " + msg);
    history.AddUserMessage(msg);
}