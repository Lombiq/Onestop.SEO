using Onestop.Seo.Services;
using Orchard.Environment;
using Orchard.Tokens;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Onestop.Seo.Controllers {
    public class RobotsTxtController : Controller {
        private readonly ISeoSettingsManager _seoSettingsManager;
        private readonly Work<ICurrentContentService> _currentContentServiceWork;
        private readonly Work<ITokenizer> _tokenizerWork;

        public RobotsTxtController(
            ISeoSettingsManager seoSettingsManager,
            Work<ICurrentContentService> currentContentServiceWork,
            Work<ITokenizer> tokenizerWork) {
            _seoSettingsManager = seoSettingsManager;
            _currentContentServiceWork = currentContentServiceWork;
            _tokenizerWork = tokenizerWork;
        }

        public string Index() {
            var globalSettingsPart = _seoSettingsManager.GetGlobalSettings();

            return Tokenize(globalSettingsPart.RobotsTxtText);
        }

        private string Tokenize(string pattern) {
            Dictionary<string, object> replaceData = null;
            var currentContent = _currentContentServiceWork.Value.GetContentForRequest();
            if (currentContent != null) replaceData = new Dictionary<string, object> { { "Content", currentContent } };

            return _tokenizerWork.Value.Replace(
                            pattern,
                            replaceData,
                            new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });
        }
    }
}