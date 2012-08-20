using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Mvc.Filters;
using System.Web.Mvc;
using Onestop.Seo.Services;
using Orchard.Environment;
using Orchard.Tokens;
using Orchard.ContentManagement;
using Contrib.Taxonomies.Models;
using Contrib.Taxonomies.Fields;
using Orchard.UI.Resources;
using Contrib.Taxonomies.Services;

namespace Onestop.Seo.Filters {
    public class CategoriesKeywordsFilter : FilterProvider, IResultFilter {
        private readonly Work<ISeoSettingsManager> _seoSettingsManagerWork;
        private readonly Work<ICurrentContentService> _currentContentServiceWork;
        private readonly Work<ITaxonomyService> _taxonomyServiceWork;
        private readonly Work<IResourceManager> _resourceManagerWork;

        public CategoriesKeywordsFilter(
            Work<ISeoSettingsManager> seoSettingsManagerWork,
            Work<ICurrentContentService> currentContentServiceWork,
            Work<ITaxonomyService> taxonomyServiceWork,
            Work<IResourceManager> resourceManagerWork) {
            _seoSettingsManagerWork = seoSettingsManagerWork;
            _currentContentServiceWork = currentContentServiceWork;
            _taxonomyServiceWork = taxonomyServiceWork;
            _resourceManagerWork = resourceManagerWork;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // Don't run on admin
            if (Orchard.UI.Admin.AdminFilter.IsApplied(filterContext.RequestContext)) return;

            if (!_seoSettingsManagerWork.Value.GetGlobalSettings().UseCategoriesForKeywords) return;

            var item = _currentContentServiceWork.Value.GetContentForRequest();
            if (item == null) return;

            var terms = _taxonomyServiceWork.Value.GetTermsForContentItem(item.Id, "Categories");
            if (terms.Count() == 0) return;

            _resourceManagerWork.Value.SetMeta(new MetaEntry {
                Name = "keywords",
                Content = String.Join(", ", terms.Select(term => term.Name))
            });
        }
    }
}