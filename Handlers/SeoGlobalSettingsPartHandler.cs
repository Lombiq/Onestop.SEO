using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JetBrains.Annotations;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Onestop.Seo.Models;

namespace Onestop.Seo.Handlers {
    [UsedImplicitly]
    public class SeoGlobalSettingsPartHandler : ContentHandler {
        public SeoGlobalSettingsPartHandler(IRepository<SeoGlobalSettingsPartRecord> repository) {
            Filters.Add(new ActivatingFilter<SeoGlobalSettingsPart>("SeoSettings"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}