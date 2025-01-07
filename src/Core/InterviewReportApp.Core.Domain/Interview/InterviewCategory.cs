namespace InterviewReportApp.Core.Domain.Interview;

public class InterviewCategory
{
    public string Category { get; set; }
    public List<InterviewQuestion> Questions { get; set; }
}