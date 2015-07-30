using System.Linq;
using Onestop.Seo.Models;
using Orchard.Caching.Services;
using Orchard.ContentManagement;

namespace Onestop.Seo.Services {
    public class SeoSettingsManager : ISeoSettingsManager {
        private readonly IContentManager _contentManager;
        private readonly ICacheService _cacheService;

        private const string CacheKey = "Onestop.Seo.Settings.Id";


        public SeoSettingsManager(IContentManager contentManager, ICacheService cacheService) {
            _contentManager = contentManager;
            _cacheService = cacheService;
        }


        public ISeoGlobalSettings GetGlobalSettings() {

            var settings = _contentManager.Query(VersionOptions.Latest, "SeoSettings").Join<SeoGlobalSettingsPartRecord>().Slice(0, 1).FirstOrDefault();

            if (settings == null)
            {
                settings = _contentManager.New("SeoSettings");
                _contentManager.Create(settings);

            }

            return _contentManager.Get(settings.Id, VersionOptions.Latest, new QueryHints().ExpandRecords<SeoGlobalSettingsPartRecord>()).As<SeoGlobalSettingsPart>();
        }
    }
}