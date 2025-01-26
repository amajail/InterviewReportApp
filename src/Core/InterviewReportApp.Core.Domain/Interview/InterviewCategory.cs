namespace InterviewReportApp.Core.Domain.Interview;

public class InterviewCategory
{
    public string CategoryName { get; set; }
    public List<InterviewQuestion> Questions { get; set; }
}