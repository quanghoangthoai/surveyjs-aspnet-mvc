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
                        CreatedAt = DateTime.UtcNow,
                        IsSupplierEvaluation = surveyDefinition.isSupplierEvaluation
                    });
                }

                await context.SaveChangesAsync();
            }

            if (!context.Suppliers.Any())
            {
                var surveys = context.Surveys
                    .Where(s => s.IsSupplierEvaluation)
                    .OrderBy(s => s.Id)
                    .ToList();

                if (surveys.Any())
                {
                    int order = 1;
                    foreach (var survey in surveys)
                    {
                        context.Suppliers.Add(new Supplier
                        {
                            Name = $"Supplier {order}",
                            Description = $"Default supplier #{order}",
                            DisplayOrder = order,
                            SurveyId = survey.Id
                        });
                        order++;
                    }

                    await context.SaveChangesAsync();
                }
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
                            var supplier = context.Suppliers.FirstOrDefault(s => s.SurveyId == surveyId);
                            if (supplier != null)
                            {
                                response.SupplierId = supplier.Id;
                            }
                        }

                        context.SurveyResponses.Add(response);
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}

