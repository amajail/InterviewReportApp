using InterviewReportApp.Core.Domain.Interview;

namespace InterviewReportApp.Infrastructure.Interfaces;

public interface IQuestionRepository
{
    InterviewQuestionsRoot GetAllQuestions();
}