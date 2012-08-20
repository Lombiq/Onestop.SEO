using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Onestop.Seo.Models;
using Orchard.Core.Common.Models;
using Orchard.Utility.Extensions;
using Orchard.Tokens;

namespace Onestop.Seo.Services {
    public class SeoService : ISeoService {
        private readonly ISeoSettingsManager _seoSettingsManager;
        private readonly ITokenizer _tokenizer;

        public SeoService(
            ISeoSettingsManager seoSettingsManager,
            ITokenizer tokenizer) {
                _seoSettingsManager = seoSettingsManager;
            _tokenizer = tokenizer;
        }

        public string GenerateTitle(IContent content) {
            var globalSettings = _seoSettingsManager.GetGlobalSettings();
            var titlePattern = globalSettings.GetTitlePattern(content.ContentItem.ContentType);
            if (String.IsNullOrEmpty(titlePattern)) return null;

            return _tokenizer.Replace(
                        titlePattern,
                        new Dictionary<string, object> { { "Content", content } },
                        new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });
        }

        public string GenerateDescription(IContent content) {
            if (!content.Has<BodyPart>()) return null;

            var text = content.As<BodyPart>().Text.RemoveTags().Trim();
            if (String.IsNullOrEmpty(text)) return null;

            var description = text;

            var maxLength = 150;
            if (text.Length > maxLength) {
                var spacePosition = text.IndexOf(" ", maxLength);
                if (spacePosition > -1) description = text.Substring(0, spacePosition);
            }

            return description;
        }
    }
}