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
        private readonly Work<ISeoSettingsManager> _seoSettingsManagerWork;
        private readonly Work<ICurrentContentService> _currentContentServiceWork;
        private readonly Work<ISeoService> _seoServiceWork;
        private readonly Work<ISeoPageTitleBuilder> _pageTitleBuilderWork;

        public TitleFilter(
            Work<ISeoSettingsManager> seoSettingsManagerWork,
            Work<ICurrentContentService> currentContentServiceWork,
            Work<ISeoService> seoServiceWork,
            Work<ISeoPageTitleBuilder> pageTitleBuilderWork) {
            _seoSettingsManagerWork = seoSettingsManagerWork;
            _currentContentServiceWork = currentContentServiceWork;
            _seoServiceWork = seoServiceWork;
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

            var title = _seoServiceWork.Value.GenerateTitle(item);
            if (String.IsNullOrEmpty(title)) return;

            _pageTitleBuilderWork.Value.OverrideTitle(title);
        }
    }
}