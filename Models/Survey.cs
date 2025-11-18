using System;
using System.Collections.Generic;

namespace surveyjs_aspnet_mvc.Models
{
    public class Survey
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Json { get; set; } = "{}";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SurveyResponse> Responses { get; set; } = new List<SurveyResponse>();
    }
}

