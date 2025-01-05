using System.Text.Json;
using InterviewReportApp.Models;
using InterviewReportApp.Services;
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
        private IInterviewReportService _interviewReportService;

        public InterviewController(IInterviewReportService interviewReportService)
        {
            _interviewReportService = interviewReportService;
        }

        // Endpoint to get all questions
        [HttpGet("questions")]
        public IActionResult GetQuestions()
        {
            return Ok(_interviewReportService.GetQuestions());
        }

        // Endpoint to submit interview notes
        [HttpPost("submit-notes")]
        public async Task<IActionResult> SubmitNotes([FromBody] string notes, CancellationToken cancellationToken)
        {
            var report = await _interviewReportService.GenerateReportAsync(notes, cancellationToken);
            return Ok(report);
        }
    }
}
