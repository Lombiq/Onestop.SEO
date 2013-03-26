using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JetBrains.Annotations;
using Onestop.Seo.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Onestop.Seo.Handlers {
    [UsedImplicitly]
    public class SeoDynamicPagePartHandler : ContentHandler {
        public SeoDynamicPagePartHandler(
            IRepository<SeoDynamicPagePartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}