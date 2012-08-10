using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement;

namespace Onestop.Seo.Services {
    public interface ISeoService : IDependency {
        IContent GetGlobalSettings();
        dynamic UpdateSettings(IUpdateModel updater);
    }
}
