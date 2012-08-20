using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement;
using Onestop.Seo.Models;

namespace Onestop.Seo.Services {
    public interface ISeoService : IDependency {
        string GenerateTitle(IContent content);
        string GenerateDescription(IContent content);
    }
}
