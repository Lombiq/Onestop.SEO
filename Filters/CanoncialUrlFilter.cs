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
using Onestop.Seo.Services;

namespace Onestop.Seo.Filters {
    public class CanoncialUrlFilter : FilterProvider, IResultFilter {
        private readonly Work<ICurrentContentService> _currentContentServiceWork;
        private readonly Work<IContentManager> _contentManagerWork;
        private readonly Work<IResourceManager> _resourceManagerWork;

        public CanoncialUrlFilter(
            Work<ICurrentContentService> currentContentServiceWork,
            Work<IContentManager> contentManagerWork,
            Work<IResourceManager> resourceManagerWork) {
            _currentContentServiceWork = currentContentServiceWork;
            _contentManagerWork = contentManagerWork;
            _resourceManagerWork = resourceManagerWork;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // Don't run on admin
            if (Orchard.UI.Admin.AdminFilter.IsApplied(filterContext.RequestContext)) return;

            // If the page we're currently on is a content item, produce a canonical url for it
            var item = _currentContentServiceWork.Value.GetContentForRequest();
            if (item == null) return;
            _resourceManagerWork.Value.RegisterLink(new LinkEntry {
                Rel = "canonical",
                Href = new UrlHelper(filterContext.RequestContext).RouteUrl(_contentManagerWork.Value.GetItemMetadata(item).DisplayRouteValues)
            });
        }
    }
}