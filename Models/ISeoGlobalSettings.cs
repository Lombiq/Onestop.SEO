using Orchard.ContentManagement;

namespace Onestop.Seo.Models {
    public enum SeoParameterType {
        Title,
        Description,
        Keywords
    }

    public interface ISeoGlobalSettings : IContent {
        string HomeTitle { get; set; }
        string HomeDescription { get; set; }
        string HomeKeywords { get; set; }
        void SetSeoPattern(SeoParameterType type, string contentType, string pattern);
        string GetSeoPattern(SeoParameterType type, string contentType);
        string SearchTitlePattern { get; set; }
        bool EnableCanonicalUrls { get; set; }
        int TitleOverrideMaxLength { get; set; }
        int DescriptionOverrideMaxLength { get; set; }
        int KeywordsOverrideMaxLength { get; set; }
    }
}