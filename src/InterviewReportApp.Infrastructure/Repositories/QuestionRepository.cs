using System.Text.Json;
using InterviewReportApp.Core.Domain.Interview;
using InterviewReportApp.Infrastructure.Interfaces;

namespace InterviewReportApp.Infrastructure;

public class QuestionRepository : IQuestionRepository
{

    public InterviewQuestionsRoot GetAllQuestions()
    {
        return LoadQuestionsFromJson("questions-net.json");
    }
    // Method to load questions from a JSON file
    private InterviewQuestionsRoot LoadQuestionsFromJson(string filePath)
    {
        var json = File.ReadAllText(filePath);

        // Deserializa el JSON a un objeto
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<InterviewQuestionsRoot>(json, options)!;
    }

}

