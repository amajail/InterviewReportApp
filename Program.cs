using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddSingleton<IChatCompletionService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiKey = configuration["SemanticKernel:ApiKey"];
    var apiUrl = configuration["SemanticKernel:ApiUrl"];
    var chatDeploymentName = configuration["SemanticKernel:ChatDeploymentName"];
    return new AzureOpenAIChatCompletionService(chatDeploymentName, apiUrl, apiKey);
});

builder.Services.AddKeyedTransient<Kernel>("InterviewReportKernel", (sp, key) =>
{
    // Create a collection of plugins that the kernel will use
    KernelPluginCollection pluginCollection = new();
    return new Kernel(sp, pluginCollection);
});

// Register controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
