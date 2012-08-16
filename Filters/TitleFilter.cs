using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Mvc.Filters;
using System.Web.Mvc;
using Onestop.Seo.Services;
using Orchard.Environment;
using Orchard.Tokens;

namespace Onestop.Seo.Filters {
    public class TitleFilter : FilterProvider, IResultFilter {
        private readonly Work<ISeoService> _seoServiceWork;
        private readonly Work<ICurrentContentService> _currentContentServiceWork;
        private readonly Work<ITokenizer> _tokenizerWork;
        private readonly Work<ISeoPageTitleBuilder> _pageTitleBuilderWork;

        public TitleFilter(
            Work<ISeoService> seoServiceWork,
            Work<ICurrentContentService> currentContentServiceWork,
            Work<ITokenizer> tokenizerWork,
            Work<ISeoPageTitleBuilder> pageTitleBuilderWork) {
            _seoServiceWork = seoServiceWork;
            _currentContentServiceWork = currentContentServiceWork;
            _tokenizerWork = tokenizerWork;
            _pageTitleBuilderWork = pageTitleBuilderWork;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {

        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // Don't run on admin
            if (Orchard.UI.Admin.AdminFilter.IsApplied(filterContext.RequestContext)) return;

            // Don't run on home page
            if (filterContext.HttpContext.Request.Path == "/") return;


            var item = _currentContentServiceWork.Value.GetContentForRequest();
            if (item == null) return;

            var globalSettings = _seoServiceWork.Value.GetGlobalSettings();
            var titlePattern = globalSettings.GetTitlePattern(item.ContentItem.ContentType);
            if (String.IsNullOrEmpty(titlePattern)) return;

            var title = _tokenizerWork.Value.Replace(
                        titlePattern,
                        new Dictionary<string, object> { { "Content", item } },
                        new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });

            _pageTitleBuilderWork.Value.OverrideTitle(title);
        }
    }
}