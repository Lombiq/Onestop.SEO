using System.Web.Mvc;
using System.Web.Routing;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment;
using Orchard.UI.Resources;

namespace Onestop.Seo.Shapes {
    public class PaginationShapes : IShapeTableProvider {
        private readonly Work<IResourceManager> _resourceManager;
        private readonly Work<UrlHelper> _urlHelper;

        public PaginationShapes(Work<IResourceManager> resourceManager, Work<UrlHelper> urlHelper) {
            _resourceManager = resourceManager;
            _urlHelper = urlHelper;
        }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Pager_Previous").OnDisplaying(context => RegisterLink("prev", context.Shape.RouteValues));
            builder.Describe("Pager_Next").OnDisplaying(context => RegisterLink("next", context.Shape.RouteValues));
        }

        private void RegisterLink(string rel, RouteValueDictionary routeValues) {
            _resourceManager.Value.RegisterLink(new LinkEntry {
                Href = _urlHelper.Value.RouteUrl(routeValues),
                Rel = rel
            });
        }
    }
}