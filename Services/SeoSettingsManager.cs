using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Onestop.Seo.Models;

namespace Onestop.Seo.Services {
    public class SeoSettingsManager : ISeoSettingsManager {
        private readonly IContentManager _contentManager;

        public SeoSettingsManager(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public ISeoGlobalSettings GetGlobalSettings() {
            var settings = _contentManager.Query(VersionOptions.Latest, "SeoSettings").Join<SeoGlobalSettingsPartRecord>().Slice(0, 1).FirstOrDefault();
            if (settings != null) return settings.As<SeoGlobalSettingsPart>();
            settings = _contentManager.New("SeoSettings");
            _contentManager.Create(settings);
            return settings.As<SeoGlobalSettingsPart>();
        }

        public dynamic UpdateSettings(IUpdateModel updater) {
            var settings = _contentManager.Get<SeoGlobalSettingsPart>(GetGlobalSettings().ContentItem.Id, VersionOptions.DraftRequired);
            var editor = _contentManager.UpdateEditor(settings, updater);
            _contentManager.Publish(settings.ContentItem);
            return editor;
        }
    }
}