using InterviewReportApp.Core.Domain.Interview;
using InterviewReportApp.Core.Application.Services.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using InterviewReportApp.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace InterviewReportApp.Core.Application.Services
{
    public class InterviewReportService([FromKeyedServices("InterviewReportKernel")] Kernel semanticKernel,
    IQuestionRepository questionRepository,
    ILogger<InterviewReportService> logger) : IInterviewReportService
    {
        private readonly ILogger _logger = logger;
        private readonly Kernel _kernel = semanticKernel;
        private readonly InterviewQuestionsRoot _questions = questionRepository.GetAllQuestions();

        // Method to generate the interview report
        public async Task<string> GenerateReportAsync(string notes, CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"Generating from this notes: {notes}", notes);

                const string promptTemplate = @"
                You are an AI assistant tasked with generating a report for HR based on the following interview notes taken from the interviewer:
                {{ InterviewReportPlugins.GetAskedQuestions }}
                Provide a concise summary in markdown, suitable for HR, in spanish";

                _kernel.ImportPluginFromPromptDirectory("Prompts/InterviewReportPlugins");
                var summarizeFunction = _kernel.CreateFunctionFromPrompt(promptTemplate);

                var summary = await summarizeFunction.InvokeAsync(_kernel,
                new() {
                    {"questions", _questions},
                    {"notes", notes}
                    });

                return summary.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report");
                return string.Empty;
            }
        }

        public InterviewQuestionsRoot GetQuestions()
        {
            return _questions;
        }
    }
}
