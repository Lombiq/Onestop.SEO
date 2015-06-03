using Onestop.Seo.Models;
using Orchard;
using System.Collections.Generic;

namespace Onestop.Seo.Providers {
    public interface ISitemapProvider : IDependency {
        IEnumerable<SitemapEntry> GetSitemapEntries();

        /// <summary>
        /// Priority of thie sitemap provider.  Higher number is a higher priority.
        /// </summary>
        int Priority { get; }
    }
}