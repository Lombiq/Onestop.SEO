using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.UI.PageTitle;
using Onestop.Seo.Services;
using Orchard.Settings;

namespace Onestop.Seo {
    [OrchardSuppressDependency("Orchard.UI.PageTitle.PageTitleBuilder")]
    public class SeoPageTitleBuilder : ISeoPageTitleBuilder {
        private readonly ISeoService _seoService;
        private readonly ISiteService _siteService;

        private readonly List<string> _titleParts;
        private string _titleSeparator;
        private string _titleOverride;

        public SeoPageTitleBuilder(
            ISeoService seoService,
            ISiteService siteService) {
            _seoService = seoService;
            _siteService = siteService;

            _titleParts = new List<string>(5);
        }

        public void AddTitleParts(params string[] titleParts) {
            if (titleParts.Length > 0)
                foreach (string titlePart in titleParts)
                    if (!string.IsNullOrEmpty(titlePart))
                        _titleParts.Add(titlePart);
        }

        public void AppendTitleParts(params string[] titleParts) {
            if (titleParts.Length > 0)
                foreach (string titlePart in titleParts)
                    if (!string.IsNullOrEmpty(titlePart))
                        _titleParts.Insert(0, titlePart);
        }

        public string GenerateTitle() {
            if (!String.IsNullOrEmpty(_titleOverride)) return _titleOverride;

            if (_titleSeparator == null) {
                _titleSeparator = _siteService.GetSiteSettings().PageTitleSeparator;
            }

            if (_titleParts.Count != 0) {
                return String.Join(_titleSeparator, _titleParts.AsEnumerable().ToArray());
            }

            return "";
        }

        public void OverrideTitle(string title) {
            _titleOverride = title;
        }
    }
}