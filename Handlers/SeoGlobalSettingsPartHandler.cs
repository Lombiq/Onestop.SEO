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
using Onestop.Seo.Services;

namespace Onestop.Seo.Handlers {
    [UsedImplicitly]
    public class SeoGlobalSettingsPartHandler : ContentHandler {
        public SeoGlobalSettingsPartHandler(
            IRepository<SeoGlobalSettingsPartRecord> repository,
            Work<ISeoService> seoServiceWork) {
            Filters.Add(new ActivatingFilter<SeoGlobalSettingsPart>("SeoSettings"));
            Filters.Add(StorageFilter.For(repository));

            OnActivated<SeoGlobalSettingsPart>((context, part) => {
                part.SeoContentTypesField.Loader(() => seoServiceWork.Value.ListSeoContentTypes());
            });
        }
    }
}