namespace InterviewReportApp.Core.Domain.Interview;
public class InterviewRouterResponse
{
    public List<string> Notes = [];
    public string Report;
    public List<string> Questions { get; set; } = [];
    public string Response { get; set; }
}