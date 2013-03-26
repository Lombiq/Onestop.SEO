using Onestop.Seo.Models;
using Onestop.Seo.Services;
using Orchard.ContentManagement;
using Orchard.Environment;
using Orchard.Mvc.Filters;
using Orchard.UI.Resources;
using System;
using System.Web.Mvc;
using System.Linq;

namespace Onestop.Seo.Filters {
    public class SeoContentFilter : FilterProvider, IResultFilter {
        private readonly Work<ISeoSettingsManager> _seoSettingsManagerWork;
        private readonly Work<ICurrentContentService> _currentContentServiceWork;
        private readonly Work<ISeoService> _seoServiceWork;
        private readonly Work<ISeoPageTitleBuilder> _pageTitleBuilderWork;
        private readonly Work<IResourceManager> _resourceManagerWork;
        private readonly IContentManager _contentManager;

        public SeoContentFilter(
            Work<ISeoSettingsManager> seoSettingsManagerWork,
            Work<ICurrentContentService> currentContentServiceWork,
            Work<ISeoService> seoServiceWork,
            Work<ISeoPageTitleBuilder> pageTitleBuilderWork,
            Work<IResourceManager> resourceManagerWork,
            IContentManager contentManager) {
            _seoSettingsManagerWork = seoSettingsManagerWork;
            _currentContentServiceWork = currentContentServiceWork;
            _seoServiceWork = seoServiceWork;
            _pageTitleBuilderWork = pageTitleBuilderWork;
            _resourceManagerWork = resourceManagerWork;
            _contentManager = contentManager;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // Don't run on admin
            if (Orchard.UI.Admin.AdminFilter.IsApplied(filterContext.RequestContext)) return;

            if (filterContext.HttpContext.Request.IsHomePage()) return;


            string title, description, keywords;

            var item = _currentContentServiceWork.Value.GetContentForRequest();
            if (item != null) {
                if (!item.Has<SeoPart>()) return;
                var seoPart = item.As<SeoPart>();
                title = !String.IsNullOrEmpty(seoPart.TitleOverride) ? seoPart.TitleOverride : seoPart.GeneratedTitle;
                description = !String.IsNullOrEmpty(seoPart.DescriptionOverride) ? seoPart.DescriptionOverride : seoPart.GeneratedDescription;
                keywords = !String.IsNullOrEmpty(seoPart.KeywordsOverride) ? seoPart.KeywordsOverride : seoPart.GeneratedKeywords;
            }
            else {
                item = _contentManager
                            .Query("SeoDynamicPage")
                            .Where<SeoDynamicPagePartRecord>(record => record.Path == filterContext.HttpContext.Request.Url.AbsolutePath)
                            .List()
                            .SingleOrDefault();

                if (item == null) return;

                var seoPart = item.As<SeoPart>();
                title = seoPart.TitleOverride;
                description = seoPart.DescriptionOverride;
                keywords = seoPart.KeywordsOverride;
            }


            if (!String.IsNullOrEmpty(title)) _pageTitleBuilderWork.Value.OverrideTitle(title);

            if (!String.IsNullOrEmpty(description)) {
                _resourceManagerWork.Value.SetMeta(new MetaEntry {
                    Name = "description",
                    Content = description
                });
            }
            
            if (!String.IsNullOrEmpty(keywords)) {
                _resourceManagerWork.Value.SetMeta(new MetaEntry {
                    Name = "keywords",
                    Content = keywords
                });
            }
        }
    }
}