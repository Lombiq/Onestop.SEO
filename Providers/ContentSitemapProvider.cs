using Onestop.Seo.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Onestop.Seo.Providers {
    [OrchardFeature("Onestop.Seo.Sitemap")]
    public class ContentSitemapProvider : ISitemapProvider {
        private readonly IContentManager _contentManager;

        public ContentSitemapProvider(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public int Priority {
            get {
                return 10;
            }
        }

        private static SitemapEntry SitemapEntry(ContentPart part) {
            var result = new SitemapEntry();

            if (part.Is<IAliasAspect>()) {
                result.Url = "/" + part.As<IAliasAspect>().Path;
            }
            if (part.Is<TitlePart>()) {
                result.Name = part.As<TitlePart>().Title;
            }
            if (part.Is<CommonPart>()) {
                var commonPart = part.As<CommonPart>();
                result.LastModified = commonPart.ModifiedUtc ?? commonPart.CreatedUtc;
            }
            return result;
        }

        public IEnumerable<SitemapEntry> GetSitemapEntries() {
            return _contentManager.Query<TitlePart>(VersionOptions.Published)
                .List()
                .Where(contentPart => contentPart.Is<IAliasAspect>())
                .Select(SitemapEntry);
        }
    }
}