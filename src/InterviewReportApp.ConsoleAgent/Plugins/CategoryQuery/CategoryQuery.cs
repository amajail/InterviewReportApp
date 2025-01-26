using System.ComponentModel;
using Microsoft.SemanticKernel;
using InterviewReportApp.Infrastructure;
using System.Linq;

public class CategoryQuery
{
    [KernelFunction("GetCategories")]
    [Description("Get avaiable categories for do a query.")]
    static string GetCategories()
    {
        var questionRepository  = new QuestionRepository();

        var response = questionRepository.GetAllQuestions().InterviewCategories;

        var categories = response.Select(q => q.CategoryName).Distinct().ToList();

        var jsonCategories = System.Text.Json.JsonSerializer.Serialize(categories);
        return jsonCategories;
    }
}