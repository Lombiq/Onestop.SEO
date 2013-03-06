using JetBrains.Annotations;
using Onestop.Seo.Models;
using Onestop.Seo.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment;

namespace Onestop.Seo.Handlers {
    [UsedImplicitly]
    public class SeoPartHandler : ContentHandler {
        public SeoPartHandler(
            IRepository<SeoPartRecord> repository,
            Work<ISeoService> seoServiceWork,
            Work<ISeoSettingsManager> settingsManagerWork) {
            Filters.Add(StorageFilter.For(repository));

            OnActivated<SeoPart>((context, part) => {
                part.GlobalSettingsField.Loader(() => settingsManagerWork.Value.GetGlobalSettings());
            });

            OnActivated<SeoPart>((context, part) => {
                part.GeneratedTitleField.Loader(() => seoServiceWork.Value.GenerateSeoParameter(SeoParameterType.Title, part.ContentItem));
            });

            OnActivated<SeoPart>((context, part) => {
                part.GeneratedDescriptionField.Loader(() => seoServiceWork.Value.GenerateSeoParameter(SeoParameterType.Description, part.ContentItem));
            });

            OnActivated<SeoPart>((context, part) => {
                part.GeneratedKeywordsField.Loader(() => seoServiceWork.Value.GenerateSeoParameter(SeoParameterType.Keywords, part.ContentItem));
            });
        }
    }
}