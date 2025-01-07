using InterviewReportApp.Core.Domain.Interview;

namespace InterviewReportApp.Core.Application.Services.Interfaces
{
        public interface IInterviewReportService
    {
        Task<string> GenerateReportAsync(string notes, CancellationToken cancellationToken);
        InterviewQuestionsRoot GetQuestions();
    }
}