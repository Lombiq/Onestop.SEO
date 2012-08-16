using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JetBrains.Annotations;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Onestop.Seo.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Environment;

namespace Onestop.Seo.Handlers {
    [UsedImplicitly]
    public class SeoGlobalSettingsPartHandler : ContentHandler {
        public SeoGlobalSettingsPartHandler(
            IRepository<SeoGlobalSettingsPartRecord> repository,
            Work<IContentDefinitionManager> contentDefinitionManagerWork) {
            Filters.Add(new ActivatingFilter<SeoGlobalSettingsPart>("SeoSettings"));
            Filters.Add(StorageFilter.For(repository));

            OnActivated<SeoGlobalSettingsPart>((context, part) => {
                part.SeoContentTypesField.Loader(
                    () => 
                        contentDefinitionManagerWork.Value.ListTypeDefinitions().Where(t => t.Parts.Any(p => p.PartDefinition.Name == typeof(SeoOverridesPart).Name)));
            });
        }
    }
}