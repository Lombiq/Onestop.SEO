using Onestop.Seo.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Onestop.Seo.Services {
    public interface ISeoSettingsManager : IDependency {
        ISeoGlobalSettings GetGlobalSettings();
    }

    public static class SeoSettingsManagerExtensions {
        public static ISeoGlobalSettings GetGlobalSettingsDraftRequired(this ISeoSettingsManager settingsManager) {
            var settings = settingsManager.GetGlobalSettings();
            return settings.ContentItem.ContentManager.Get<ISeoGlobalSettings>(settings.ContentItem.Id, VersionOptions.DraftRequired);
        }
    }
}
