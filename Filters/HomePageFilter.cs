using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Mvc.Filters;
using Orchard;
using Onestop.Seo.Services;
using Orchard.Mvc;
using Orchard.ContentManagement;
using Onestop.Seo.Models;
using Orchard.Environment;
using Orchard.UI.Resources;

namespace Onestop.Seo.Filters {
    public class HomePageFilter : FilterProvider, IResultFilter {
        private readonly Work<ISeoService> _seoServiceWork;
        private readonly Work<ISeoPageTitleBuilder> _pageTitleBuilderWork;
        private readonly Work<IResourceManager> _resourceManagerWork;

        public HomePageFilter(
            Work<ISeoService> seoServiceWork,
            Work<ISeoPageTitleBuilder> pageTitleBuilderWork,
            Work<IResourceManager> resourceManagerWork) {
            _seoServiceWork = seoServiceWork;
            _pageTitleBuilderWork = pageTitleBuilderWork;
            _resourceManagerWork = resourceManagerWork;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {

        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // Only run on home page
            if (filterContext.HttpContext.Request.Path != "/") return;

            var globalSettings = _seoServiceWork.Value.GetGlobalSettings();


            if (!String.IsNullOrEmpty(globalSettings.HomeTitle)) {
                _pageTitleBuilderWork.Value.OverrideTitle(globalSettings.HomeTitle); 
            }


            var resourceManager = _resourceManagerWork.Value;

            if (!String.IsNullOrEmpty(globalSettings.HomeDescription)) {
                resourceManager.SetMeta(new MetaEntry {
                    Name = "description",
                    Content = globalSettings.HomeDescription
                }); 
            }

            if (!String.IsNullOrEmpty(globalSettings.HomeKeywords)) {
                resourceManager.SetMeta(new MetaEntry {
                    Name = "keywords",
                    Content = globalSettings.HomeKeywords
                }); 
            }
        }
    }
}