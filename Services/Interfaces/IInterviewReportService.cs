
using InterviewReportApp.Models;

namespace InterviewReportApp.Services
{
    public interface IInterviewReportService
    {

        Task<InterviewReportResponse> GenerateReportAsync(string notes, CancellationToken cancellationToken);
        InterviewQuestionsRoot GetQuestions();
    }
}