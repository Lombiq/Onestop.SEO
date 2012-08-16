using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Mvc.Filters;
using System.Web.Mvc;
using Orchard.Environment;
using Onestop.Seo.Services;
using Orchard.Tokens;

namespace Onestop.Seo.Filters {
    public class SearchTitleFilter : FilterProvider, IActionFilter {
        private readonly Work<ISeoService> _seoServiceWork;
        private readonly Work<ITokenizer> _tokenizerWork;
        private readonly Work<ISeoPageTitleBuilder> _pageTitleBuilderWork;

        public SearchTitleFilter(
            Work<ISeoService> seoServiceWork,
            Work<ITokenizer> tokenizerWork,
            Work<ISeoPageTitleBuilder> pageTitleBuilderWork) {
            _seoServiceWork = seoServiceWork;
            _tokenizerWork = tokenizerWork;
            _pageTitleBuilderWork = pageTitleBuilderWork;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {
            var routeValues = filterContext.HttpContext.Request.RequestContext.RouteData.Values;
            if (routeValues["area"] != "Orchard.Search" || routeValues["controller"] != "search" || routeValues["action"] != "index") return;

            var titlePattern = _seoServiceWork.Value.GetGlobalSettings().SearchTitlePattern;
            if (String.IsNullOrEmpty(titlePattern)) return;

            var title = _tokenizerWork.Value.Replace(
                        titlePattern,
                        null,
                        new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });

            _pageTitleBuilderWork.Value.OverrideTitle(title);
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) {
        }
    }
}