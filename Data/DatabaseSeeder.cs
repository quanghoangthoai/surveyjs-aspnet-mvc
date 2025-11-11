using System;
using System.Linq;
using System.Threading.Tasks;
using surveyjs_aspnet_mvc.Models;
using surveyjs_aspnet_mvc;

namespace surveyjs_aspnet_mvc.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Surveys.Any())
            {
                var defaults = SurveyDefinition.GetDefaultSurveys();
                foreach (var surveyDefinition in defaults)
                {
                    context.Surveys.Add(new Survey
                    {
                        Name = surveyDefinition.name,
                        Json = surveyDefinition.json,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await context.SaveChangesAsync();
            }

            if (!context.SurveyResponses.Any())
            {
                var defaultResults = SurveyResultsDefinition.GetDefaultSurveyResults();
                foreach (var result in defaultResults)
                {
                    foreach (var data in result.data)
                    {
                        var response = new SurveyResponse
                        {
                            PostId = result.id,
                            ResultJson = data,
                            CreatedAt = DateTime.UtcNow
                        };

                        if (int.TryParse(result.id, out var surveyId))
                        {
                            response.SurveyId = surveyId;
                        }

                        context.SurveyResponses.Add(response);
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}

