#nullable enable
using System.Collections.Generic;

namespace surveyjs_aspnet_mvc.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public int? SurveyId { get; set; }
        public Survey? Survey { get; set; }
        public ICollection<SurveyResponse> Responses { get; set; } = new List<SurveyResponse>();
    }
}

