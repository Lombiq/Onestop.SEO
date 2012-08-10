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

namespace Onestop.Seo.Filters {
    public class HomePageFilter : FilterProvider, IResultFilter {
        private readonly ISeoService _seoService;
        private readonly ISeoPageTitleBuilder _pageTitleBuilder;

        public HomePageFilter(
            ISeoService seoService,
            ISeoPageTitleBuilder pageTitleBuilder) {
            _seoService = seoService;
            _pageTitleBuilder = pageTitleBuilder;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
            
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // Only run on home page
            if (filterContext.HttpContext.Request.Path != "/") return;

            _pageTitleBuilder.OverrideTitle(_seoService.GetGlobalSettings().As<SeoGlobalSettingsPart>().HomeTitle);
        }
    }
}