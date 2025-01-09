using InterviewReportApp.Core.Application.Services.Interfaces;
using InterviewReportApp.Core.Domain.Interview;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace InterviewReportApp.Core.Application.Services
{
    public class IntentService([FromKeyedServices("InterviewReportKernel")] Kernel semanticKernel,
                               IInterviewReportService interviewReportService,
                               ILogger<IntentService> logger)
                               : IIntentService
    {
        private readonly Kernel _kernel = semanticKernel;
        private readonly KernelPlugin _intentPlugins = semanticKernel.ImportPluginFromPromptDirectory("Prompts/IntentPlugins");
        private readonly IInterviewReportService _interviewReportService = interviewReportService;
        private List<string> _notes = [];
        private readonly ILogger<IntentService> _logger = logger;

        public async Task<InterviewRouterResponse> RouteIntent(string input, CancellationToken cancellationToken)
        {
            var intent = await _kernel.InvokeAsync<string>(
                _intentPlugins["GetIntent"],
                new() { { "input", input } }
            );

            var response = new InterviewRouterResponse();

            _logger.LogInformation($"{intent} intent detected.",intent);

            switch (intent)
            {
                case "MakeReport":
                    string notes = TextPreprocessor.PreProcessNotesList(_notes);
                    response.Report = await _interviewReportService.GenerateReportAsync(notes, cancellationToken);
                    break;
                case "TakeNote":
                    _notes.Add(input);
                    response.Notes = _notes;
                    break;
                case "SuggestQuestions":
                    response.Questions = ["Que dia es hoy?"];
                    break;
                default:
                    var otherIntentResult = await _kernel.InvokePromptAsync(input);
                    response.Response = otherIntentResult.ToString();
                    break;
            }

            return response;
        }
    }
}