
using InterviewReportApp.Core.Domain.Interview;

namespace InterviewReportApp.Core.Application.Services.Interfaces;

public interface IIntentService
{
    Task<InterviewRouterResponse> RouteIntent(string input, CancellationToken cancellationToken);
}