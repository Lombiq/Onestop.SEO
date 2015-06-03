using Onestop.Seo.Services;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Mvc;
using Orchard.Themes;
using System.IO;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Onestop.Seo.Controllers {
    [OrchardFeature("Onestop.Seo.Sitemap"), Themed]
    public class SitemapController : Controller {
        private dynamic Shape { get; set; }
        private readonly ISitemapService _sitemapService;

        public SitemapController(IShapeFactory shapeFactory, ISitemapService sitemapService) {
            Shape = shapeFactory;
            _sitemapService = sitemapService;
        }

        public ActionResult Xml() {
            var xml = _sitemapService.BuildXml();
            using (var ms = new MemoryStream()) {
                xml.Save(ms, SaveOptions.DisableFormatting);
                ms.Position = 0;
                using (var reader = new StreamReader(ms)) {
                    return Content(reader.ReadToEnd(), "text/xml");
                }
            }
        }

        public ActionResult Html() {
            var entries = _sitemapService.GetEntries();
            return new ShapeResult(this, Shape.Sitemap(Entries: entries));
        }
    }
}