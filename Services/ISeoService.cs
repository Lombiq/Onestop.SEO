using Onestop.Seo.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using System.Collections.Generic;

namespace Onestop.Seo.Services {
    public interface ISeoService : IDependency {
        IEnumerable<ContentTypeDefinition> ListSeoContentTypes();
        string GenerateSeoParameter(SeoParameterType type, IContent content);
    }
}
