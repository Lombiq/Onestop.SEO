using Orchard.Environment.Extensions;
using System;

namespace Onestop.Seo.Models {
    [OrchardFeature("Onestop.Seo.Sitemap")]
    [Serializable]
    public class SitemapEntry {
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime? LastModified { get; set; }
        public float? Priority { get; set; }
        public SitemapChangeFrequency? ChangeFrequency { get; set; }
    }

    [Serializable]
    public enum SitemapChangeFrequency {
        Always, Hourly, Daily, Weekly, Monthly, Yearly, Never
    }
}