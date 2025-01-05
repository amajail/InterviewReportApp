namespace InterviewReportApp.Models
{
    public class InterviewReportResponse
    {
        public string Report { get; set; }
        public string ChatHistory { get; internal set; }
    }
}