using Onestop.Seo.Models;
using Onestop.Seo.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment;

namespace Onestop.Seo.Handlers {
    public class SeoPartHandler : ContentHandler {
        public SeoPartHandler(
            IRepository<SeoPartRecord> repository,
            Work<ISeoService> seoServiceWork,
            Work<ISeoSettingsManager> settingsManagerWork) {
            Filters.Add(StorageFilter.For(repository));

            OnActivated<SeoPart>((context, part) => {
                part.GlobalSettingsField.Loader(() => settingsManagerWork.Value.GetGlobalSettings());
                part.GeneratedTitleField.Loader(() => seoServiceWork.Value.GenerateSeoParameter(SeoParameterType.Title, part.ContentItem));
                part.GeneratedDescriptionField.Loader(() => seoServiceWork.Value.GenerateSeoParameter(SeoParameterType.Description, part.ContentItem));
                part.GeneratedKeywordsField.Loader(() => seoServiceWork.Value.GenerateSeoParameter(SeoParameterType.Keywords, part.ContentItem));
            });
        }
    }
}