using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Mvc.Filters;
using Orchard.Alias;
using Orchard.ContentManagement;
using Orchard.Environment;
using Orchard.UI.Resources;

namespace Onestop.Seo.Filters {
    public class CanoncialUrlFilter : FilterProvider, IResultFilter {
        private readonly IAliasService _aliasService;
        private readonly Work<IContentManager> _contentManagerWork;
        private readonly Work<IResourceManager> _resourceManagerWork;

        public CanoncialUrlFilter(
            IAliasService aliasService,
            Work<IContentManager> contentManagerWork,
            Work<IResourceManager> resourceManagerWork) {
            _aliasService = aliasService;
            _contentManagerWork = contentManagerWork;
            _resourceManagerWork = resourceManagerWork;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // Don't run on admin
            if (Orchard.UI.Admin.AdminFilter.IsApplied(filterContext.RequestContext)) return;

            // Checking if the page we're currently on is a content item
            var itemRoute = _aliasService.Get(filterContext.HttpContext.Request.Path.Trim('/'));
            if (itemRoute == null) return;

            // If yes, produce a canonical url for it
            var itemId = Convert.ToInt32(itemRoute["Id"]);
            var contentManager = _contentManagerWork.Value;
            var item = contentManager.Get(itemId);
            if (item == null) return;
            _resourceManagerWork.Value.RegisterLink(new LinkEntry {
                Rel = "canonical",
                Href = new UrlHelper(filterContext.RequestContext).RouteUrl(contentManager.GetItemMetadata(item).DisplayRouteValues)
            });
        }
    }
}