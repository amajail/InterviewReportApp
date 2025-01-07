using InterviewReportApp.Core.Application.Services;
using InterviewReportApp.Core.Application.Services.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Interview Questions API",
        Description = "API para manejar preguntas de entrevistas y generar informes"
    });
});

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddSingleton<IChatCompletionService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiKey = configuration["SemanticKernel:ApiKey"];
    var apiUrl = configuration["SemanticKernel:ApiUrl"];
    var chatDeploymentName = configuration["SemanticKernel:ChatDeploymentName"];
    return new AzureOpenAIChatCompletionService(chatDeploymentName!, apiUrl!, apiKey!);
});

builder.Services.AddKeyedTransient<Kernel>("InterviewReportKernel", (sp, key) =>
{
    // Create a collection of plugins that the kernel will use
    KernelPluginCollection pluginCollection = new();
    return new Kernel(sp, pluginCollection);
});

builder.Services.AddTransient<IInterviewReportService, InterviewReportService>();

// Register controllers
builder.Services.AddControllers();

var app = builder.Build();


// Habilitar Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Interview Questions API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz del proyecto
    });
}

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
