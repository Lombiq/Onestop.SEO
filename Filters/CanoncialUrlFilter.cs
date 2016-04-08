using System.Web.Mvc;
using Onestop.Seo.Services;
using Orchard.ContentManagement;
using Orchard.Environment;
using Orchard.Mvc.Filters;
using Orchard.UI.Resources;
using Orchard.Core.Settings.Models;
using Orchard;
using Onestop.Seo.Models;

namespace Onestop.Seo.Filters {
    public class CanoncialUrlFilter : FilterProvider, IResultFilter {
        private readonly Work<ISeoSettingsManager> _seoSettingsManagerWork;
        private readonly Work<ICurrentContentService> _currentContentServiceWork;
        private readonly Work<IContentManager> _contentManagerWork;
        private readonly Work<IResourceManager> _resourceManagerWork;

        public CanoncialUrlFilter(
            Work<ISeoSettingsManager> seoSettingsManagerWork,
            Work<ICurrentContentService> currentContentServiceWork,
            Work<IContentManager> contentManagerWork,
            Work<IResourceManager> resourceManagerWork) {
            _seoSettingsManagerWork = seoSettingsManagerWork;
            _currentContentServiceWork = currentContentServiceWork;
            _contentManagerWork = contentManagerWork;
            _resourceManagerWork = resourceManagerWork;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // Don't run on admin  or in non-full views
            if (Orchard.UI.Admin.AdminFilter.IsApplied(filterContext.RequestContext) 
                || !(filterContext.Result is ViewResult)
                || filterContext.RequestContext.HttpContext.Request.IsAjaxRequest()
                || !_seoSettingsManagerWork.Value.GetGlobalSettings().EnableCanonicalUrls)
                return;

            var SEOItemPart = _currentContentServiceWork.Value.GetContentForRequest().As<SeoPart>();

            if (SEOItemPart != null && !string.IsNullOrEmpty(SEOItemPart.CanonicalUrlOverride))
            {
                var workContext = filterContext.GetWorkContext();
                var siteSettings = workContext.CurrentSite.As<SiteSettingsPart>();
                var cUrl = SEOItemPart.CanonicalUrlOverride;

                if (cUrl.ToString().Substring(0, 1) != "/")
                {
                    cUrl = "/" + cUrl;
                }
                _resourceManagerWork.Value.RegisterLink(new LinkEntry
                {
                    Rel = "canonical",
                    Href = siteSettings.BaseUrl + cUrl
                });
                return;
            }

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