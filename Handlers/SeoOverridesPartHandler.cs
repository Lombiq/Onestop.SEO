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
    public class SeoOverridesPartHandler : ContentHandler {
        public SeoOverridesPartHandler(IRepository<SeoOverridesPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}