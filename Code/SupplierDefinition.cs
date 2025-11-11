#nullable enable

namespace surveyjs_aspnet_mvc
{
    public class SupplierDefinition
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? description { get; set; }
        public int displayOrder { get; set; }
        public string? surveyId { get; set; }
    }
}

