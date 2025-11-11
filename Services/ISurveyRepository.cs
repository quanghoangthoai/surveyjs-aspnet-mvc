#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using surveyjs_aspnet_mvc;

namespace surveyjs_aspnet_mvc.Services
{
    public interface ISurveyRepository
    {
        Task<IEnumerable<SurveyDefinition>> GetActiveAsync();
        Task<SurveyDefinition?> GetSurveyAsync(string surveyId);
        Task<SurveyDefinition> CreateSurveyAsync(string? name, bool isSupplierEvaluation = true, int? supplierId = null, string? templateId = null);
        Task<SurveyDefinition?> UpdateSurveyJsonAsync(string surveyId, string json);
        Task<bool> ChangeNameAsync(string surveyId, string name);
        Task<bool> DeleteSurveyAsync(string surveyId);
        Task PostResultAsync(string postId, string resultJson, int? supplierId = null);
        Task<SurveyResultsDefinition> GetResultsAsync(string postId);
        Task<IEnumerable<SupplierDefinition>> GetSuppliersAsync();
        Task<SupplierDefinition> CreateSupplierAsync(string name, string? description, int displayOrder, string? surveyId);
        Task<SupplierDefinition?> AssignSurveyToSupplierAsync(int supplierId, string surveyId);
        Task<SurveyDefinition?> GetNextSupplierSurveyAsync(string? currentSurveyId);
        Task<IEnumerable<SurveyDefinition>> GetSurveyTemplatesAsync();
    }
}

