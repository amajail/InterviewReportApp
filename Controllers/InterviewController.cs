using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;

namespace InterviewReportApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        private readonly Kernel _kernel;
        private readonly List<Question> _questions;

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
        public async Task<IActionResult> SubmitNotes([FromBody] List<InterviewNote> notes, CancellationToken cancellationToken)
        {
            var report = await GenerateReportAsync(notes, cancellationToken);
            return Ok(report);
        }

        // Method to load questions from a JSON file
        private List<Question> LoadQuestionsFromJson(string filePath)
        {
            var json = System.IO.File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Question>>(json);
        }

        // Method to generate the interview report
        private async Task<string> GenerateReportAsync(List<InterviewNote> notes, CancellationToken stoppingToken)
        {
            // Get chat completion service
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            // Enable auto function calling
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            ChatMessageContent chatResult = await chatCompletionService.GetChatMessageContentAsync(input,
                    openAIPromptExecutionSettings, _kernel, stoppingToken);

            return chatResult.ToString();
        }

        // Model for a question
        public class Question
        {
            public string Category { get; set; }
            public string Text { get; set; }
        }

        // Model for interview notes
        public class InterviewNote
        {
            public string Question { get; set; }
            public string Response { get; set; }
        }
    }
