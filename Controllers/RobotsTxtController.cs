using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Onestop.Seo.Models;
using Orchard.ContentManagement;
using Onestop.Seo.Services;

namespace Onestop.Seo.Controllers {
    public class RobotsTxtController : Controller {
        private readonly ISeoSettingsManager _seoSettingsManager;


        public RobotsTxtController(ISeoSettingsManager seoSettingsManager) {
            _seoSettingsManager = seoSettingsManager;
        }


        public string RobotsTxtText() {
            var globalSettingsPart = _seoSettingsManager.GetGlobalSettings();

            return globalSettingsPart.RobotsTxtText;
        }
    }
}