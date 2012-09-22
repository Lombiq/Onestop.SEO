using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Onestop.Seo.Models;
using Orchard.Core.Common.Models;
using Orchard.Utility.Extensions;
using Orchard.Tokens;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.MetaData;

namespace Onestop.Seo.Services {
    public class SeoService : ISeoService {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ISeoSettingsManager _seoSettingsManager;
        private readonly ITokenizer _tokenizer;

        public SeoService(
            IContentDefinitionManager contentDefinitionManager,
            ISeoSettingsManager seoSettingsManager,
            ITokenizer tokenizer) {
            _contentDefinitionManager = contentDefinitionManager;
            _seoSettingsManager = seoSettingsManager;
            _tokenizer = tokenizer;
        }

        public IEnumerable<ContentTypeDefinition> ListSeoContentTypes() {
            return _contentDefinitionManager.ListTypeDefinitions().Where(t => t.Parts.Any(p => p.PartDefinition.Name == typeof(SeoPart).Name));
        }

        public string GenerateSeoParameter(SeoParameterType type, IContent content) {
            var globalSettings = _seoSettingsManager.GetGlobalSettings();
            var pattern = globalSettings.GetSeoPattern(type, content.ContentItem.ContentType);
            if (String.IsNullOrEmpty(pattern)) return null;

            return _tokenizer.Replace(
                        pattern,
                        new Dictionary<string, object> { { "Content", content } },
                        new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });
        }
    }
}