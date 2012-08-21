using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement;
using Onestop.Seo.Models;
using Orchard.ContentManagement.MetaData.Models;

namespace Onestop.Seo.Services {
    public interface ISeoService : IDependency {
        IEnumerable<ContentTypeDefinition> ListSeoContentTypes();
        string GenerateTitle(IContent content);
        string GenerateDescription(IContent content);
    }
}
