using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Onestop.Seo.Models;
using Orchard.ContentManagement;
using Orchard;

namespace Onestop.Seo.Services {
    public interface ISeoSettingsManager : IDependency {
        ISeoGlobalSettings GetGlobalSettings();
        dynamic UpdateSettings(IUpdateModel updater);
    }
}
