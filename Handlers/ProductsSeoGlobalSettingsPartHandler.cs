using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JetBrains.Annotations;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Onestop.Seo.Models;
using Orchard.Environment;

namespace Onestop.Seo.Handlers {
    [UsedImplicitly]
    public class ProductsSeoGlobalSettingsPartHandler : ContentHandler {
        public ProductsSeoGlobalSettingsPartHandler(
            IRepository<ProductsSeoGlobalSettingsPartRecord> repository) {
            Filters.Add(new ActivatingFilter<ProductsSeoGlobalSettingsPart>("SeoSettings"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}