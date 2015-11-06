using System.Web.Mvc;
using Onestop.Seo.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment;
using Orchard.Mvc.Filters;
using Orchard.UI.Resources;
using Onestop.Seo.Models;
using Orchard.Core.Settings.Models;
using Orchard.Environment.Extensions;

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


            if (Orchard.UI.Admin.AdminFilter.IsApplied(filterContext.RequestContext) 
                || !(filterContext.Result is ViewResult)
                || filterContext.RequestContext.HttpContext.Request.IsAjaxRequest()
                || !_seoSettingsManagerWork.Value.GetGlobalSettings().EnableCanonicalUrls)
                return;

            // If the page we're currently on does not have a canonical URL set; or the auto generate is on but there are no canonical urls set for it use the automatic option.
            var item = _currentContentServiceWork.Value.GetContentForRequest();
            if (item == null) return;
            _resourceManagerWork.Value.RegisterLink(new LinkEntry {
                Rel = "canonical",
                Href = new UrlHelper(filterContext.RequestContext).RouteUrl(_contentManagerWork.Value.GetItemMetadata(item).DisplayRouteValues)
            });
        }
    }
}