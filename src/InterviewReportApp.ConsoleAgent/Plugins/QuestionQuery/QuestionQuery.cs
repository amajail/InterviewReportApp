

using System.ComponentModel;
using InterviewReportApp.Infrastructure;
using Microsoft.SemanticKernel;

public class QuestionQuery
{
    [KernelFunction("GetQuestions")]
    [Description("Get the questions for the specified category.")]
    static string GetQuestions(string category)
    {
        var questionRepository  = new QuestionRepository();

        var response = questionRepository.GetAllQuestions().InterviewCategories;

        var questions = response
            .Where(c => c.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase))
            .SelectMany(c => c.Questions)
            .ToList();

        return System.Text.Json.JsonSerializer.Serialize(questions);
    }
}