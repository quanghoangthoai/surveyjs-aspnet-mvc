using System;

namespace surveyjs_aspnet_mvc.Models
{
    public class SurveyResponse
    {
        public int Id { get; set; }

        public int? SurveyId { get; set; }

        public string PostId { get; set; } = string.Empty;

        public string ResultJson { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Survey? Survey { get; set; }
    }
}

