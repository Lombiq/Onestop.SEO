﻿using System;
using Orchard.ContentManagement;

namespace Onestop.Seo.Models {
    public interface ISeoGlobalSettings : IContent {
        string HomeTitle { get; set; }
        string HomeDescription { get; set; }
        string HomeKeywords { get; set; }
        void SetTitlePattern(string contentType, string pattern);
        string GetTitlePattern(string contentType);
        bool EnableCanonicalUrls { get; set; }
        bool UseCategoriesForKeywords { get; set; }
        bool AutogenerateDescriptions { get; set; }
    }
}
