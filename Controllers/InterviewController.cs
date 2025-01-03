using System.Text.Json;
using InterviewReportApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace InterviewReportApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        private readonly Kernel _kernel;
        private readonly InterviewQuestionsRoot _questions;

        public InterviewController([FromKeyedServices("InterviewReportKernel")] Kernel semanticKernel)
        {
            _kernel = semanticKernel;
            _questions = LoadQuestionsFromJson("questions.json");
        }

        // Endpoint to get all questions
        [HttpGet("questions")]
        public IActionResult GetQuestions()
        {
            return Ok(_questions);
        }

        // Endpoint to submit interview notes
        [HttpPost("submit-notes")]
        public async Task<IActionResult> SubmitNotes([FromBody] string notes, CancellationToken cancellationToken)
        {
            var report = await GenerateReportAsync(notes, cancellationToken);
            return Ok(report);
        }

        // Method to load questions from a JSON file
        private InterviewQuestionsRoot LoadQuestionsFromJson(string filePath)
        {
            var json = System.IO.File.ReadAllText(filePath);

            // Deserializa el JSON a un objeto
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<InterviewQuestionsRoot>(json, options);
        }

        // Method to generate the interview report
        private async Task<string> GenerateReportAsync(string notes, CancellationToken stoppingToken)
        {
            // Get chat completion service
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            // Enable auto function calling
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            var input = $"Summary this {_questions}";

            ChatMessageContent chatResult = await chatCompletionService.GetChatMessageContentAsync(input,
                    openAIPromptExecutionSettings, _kernel, stoppingToken);

            return chatResult.ToString();
        }
    }
}
