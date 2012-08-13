using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement;
using Onestop.Seo.Models;

namespace Onestop.Seo.Services {
    public interface ISeoService : IDependency {
        ISeoGlobalSettings GetGlobalSettings();
        dynamic UpdateSettings(IUpdateModel updater);
    }
}
