using Onestop.Seo.Services;
using Orchard.Environment;
using Orchard.Tokens;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Onestop.Seo.Controllers {
    public class RobotsTxtController : Controller {
        private readonly ISeoSettingsManager _seoSettingsManager;
        private readonly ITokenizer _tokenizer;

        public RobotsTxtController(
            ISeoSettingsManager seoSettingsManager,
            ITokenizer tokenizer) {
            _seoSettingsManager = seoSettingsManager;
            _tokenizer = tokenizer;
        }

        public string Index() {
            return _tokenizer.Replace(
                _seoSettingsManager.GetGlobalSettings().RobotsTxtText,
                new Dictionary<string, object>(),
                new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });
        }
    }
}