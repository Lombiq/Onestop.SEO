using JetBrains.Annotations;
using Onestop.Seo.Models;
using Onestop.Seo.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment;

namespace Onestop.Seo.Handlers {
    [UsedImplicitly]
    public class SeoGlobalSettingsPartHandler : ContentHandler {
        public SeoGlobalSettingsPartHandler(Work<ISeoService> seoServiceWork) {
            OnActivated<SeoGlobalSettingsPart>((context, part) => {
                part.SeoContentTypesField.Loader(() => seoServiceWork.Value.ListSeoContentTypes());
            });
        }
    }
}