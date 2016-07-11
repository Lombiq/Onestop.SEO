using Onestop.Seo.Services;
using System.Web.Mvc;

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