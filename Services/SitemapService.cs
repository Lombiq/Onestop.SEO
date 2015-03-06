using Onestop.Seo.Models;
using Onestop.Seo.Providers;
using Orchard;
using Orchard.Caching.Services;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Onestop.Seo.Services {
    public interface ISitemapService : IDependency {
        XDocument BuildXml();
        IEnumerable<SitemapEntry> GetEntries();
    }

    [OrchardFeature("Onestop.Seo.Sitemap")]
    public class SitemapService : ISitemapService {
        private const string CacheKeyFormat = "SiteMap.{0}";
        private const int CacheExpirationHours = 24;

        private readonly XNamespace _xmlNamespace = @"http://www.sitemaps.org/schemas/sitemap/0.9";
        private readonly IEnumerable<ISitemapProvider> _sitemapProviders;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ICacheService _cacheService;
        private readonly Lazy<string> _baseUrl; 

        public SitemapService(IEnumerable<ISitemapProvider> sitemapProviders, IWorkContextAccessor workContextAccessor, ICacheService cacheService) {
            _baseUrl = new Lazy<string>(GetBaseUrl);
            _sitemapProviders = sitemapProviders;
            _workContextAccessor = workContextAccessor;
            _cacheService = cacheService;
        }

        private string GetBaseUrl() {
            return _workContextAccessor.GetContext().CurrentSite.BaseUrl.TrimEnd('/');
        }

        private XElement Url(SitemapEntry sitemapResult) {
            var url = new XElement(_xmlNamespace + "url");
            if (!String.IsNullOrEmpty(sitemapResult.Url)) {
                var fullPath = _baseUrl.Value + sitemapResult.Url;
                url.Add(new XElement(_xmlNamespace + "loc", fullPath));
            }
            if (sitemapResult.LastModified.HasValue) {
                url.Add(new XElement(_xmlNamespace + "lastmod", sitemapResult.LastModified.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz")));
            }
            if (sitemapResult.Priority.HasValue) {
                url.Add(new XElement(_xmlNamespace + "priority", sitemapResult.Priority.Value.ToString("#.#")));
            }
            if (sitemapResult.ChangeFrequency.HasValue) {
                url.Add(new XElement(_xmlNamespace + "changefreq", sitemapResult.ChangeFrequency.Value.ToString().ToLower()));
            }
            return url;
        }

        public XDocument BuildXml() {
            return new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(_xmlNamespace + "urlset", GetEntries().Select(Url)));
        }

        public IEnumerable<SitemapEntry> GetEntries() {
            return _sitemapProviders
                .OrderByDescending(provider => provider.Priority)
                .SelectMany(provider => _cacheService.Get(String.Format(CacheKeyFormat, provider.GetType().Name),
                    () => provider.GetSitemapEntries().ToArray(), new TimeSpan(CacheExpirationHours, 0, 0)))
                .GroupBy(entry => entry.Url)
                .Select(grouped => grouped.First())
                .OrderByDescending(entry => entry.Priority);
        }
    }
}