
using Onestop.Seo.Models;
using Onestop.Seo.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Environment;

namespace Onestop.Seo.Handlers {
    
    public class SeoGlobalSettingsPartHandler : ContentHandler {
        public SeoGlobalSettingsPartHandler(
            IRepository<SeoGlobalSettingsPartRecord> repository,
            Work<ISeoService> seoServiceWork) {
            Filters.Add(StorageFilter.For(repository));

            OnActivated<SeoGlobalSettingsPart>((context, part) => {
                part.SeoContentTypesField.Loader(() => seoServiceWork.Value.ListSeoContentTypes());
            });
        }
    }
}