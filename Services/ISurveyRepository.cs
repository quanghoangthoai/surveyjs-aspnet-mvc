using System.Collections.Generic;
using System.Threading.Tasks;
using surveyjs_aspnet_mvc;

namespace surveyjs_aspnet_mvc.Services
{
    public interface ISurveyRepository
    {
        Task<IEnumerable<SurveyDefinition>> GetActiveAsync();
        Task<SurveyDefinition?> GetSurveyAsync(string surveyId);
        Task<SurveyDefinition> CreateSurveyAsync(string? name);
        Task<SurveyDefinition?> UpdateSurveyJsonAsync(string surveyId, string json);
        Task<bool> ChangeNameAsync(string surveyId, string name);
        Task<bool> DeleteSurveyAsync(string surveyId);
        Task PostResultAsync(string postId, string resultJson);
        Task<SurveyResultsDefinition> GetResultsAsync(string postId);
    }
}

