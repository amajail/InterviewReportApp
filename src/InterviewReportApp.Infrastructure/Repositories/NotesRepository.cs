using InterviewReportApp.Infrastructure.Interfaces;

namespace InterviewReportApp.Infrastructure;

public class NotesRepository : INotesRepository
{
    public async Task<string> GetNotesAsync(CancellationToken stoppingToken)
    {
        return await File.ReadAllTextAsync("Files/notes/notes.txt", stoppingToken);
    }
}

