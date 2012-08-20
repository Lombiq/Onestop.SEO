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
        private readonly Work<ISeoSettingsManager> _seoSettingsManagerWork;
        private readonly Work<ISeoPageTitleBuilder> _pageTitleBuilderWork;
        private readonly Work<IResourceManager> _resourceManagerWork;

        public HomePageFilter(
            Work<ISeoSettingsManager> seoSettingsManagerWork,
            Work<ISeoPageTitleBuilder> pageTitleBuilderWork,
            Work<IResourceManager> resourceManagerWork) {
            _seoSettingsManagerWork = seoSettingsManagerWork;
            _pageTitleBuilderWork = pageTitleBuilderWork;
            _resourceManagerWork = resourceManagerWork;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {

        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // Only run on home page
            if (filterContext.HttpContext.Request.Path != "/") return;

            var globalSettings = _seoSettingsManagerWork.Value.GetGlobalSettings();


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