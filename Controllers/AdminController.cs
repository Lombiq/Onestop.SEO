using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Onestop.Seo.Models;
using Onestop.Seo.Services;
using Onestop.Seo.ViewModels;
using Orchard;
using Orchard.Collections;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Common.Models;
using Orchard.Mvc;
using Orchard.Core.Contents.ViewModels;
using Orchard.Exceptions;
using Orchard.Indexing;
using Orchard.Localization;
using Orchard.Search.Helpers;
using Orchard.Search.Models;
using Orchard.Search.Services;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;

namespace Onestop.Seo.Controllers {
    [Admin]
    public class AdminController : AdminControllerBase {
        private readonly ISearchService _searchService;


        public AdminController(
            IOrchardServices orchardServices,
            ISearchService searchService,
            IPrefixedEditorManager prefixedEditorManager,
            ISeoSettingsManager seoSettingsManager,
            ISeoService seoService)
            : base(orchardServices, prefixedEditorManager, seoSettingsManager, seoService) {
            _searchService = searchService;
        }


        // If there will be a need for extending global SEO settings it would perhaps also need the usage of separate settings into groups.
        // See site settings (Orchard.Core.Settings.Controllers.AdminController and friends for how it is done.
        public ActionResult GlobalSettings() {
            if (!IsAuthorized()) return new HttpUnauthorizedResult();

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation, despite
            // being it highly unlikely with Onestop, just in case...
            return View((object)_contentManager.BuildEditor(_seoSettingsManager.GetGlobalSettings()));
        }

        [HttpPost, ActionName("GlobalSettings")]
        public ActionResult GlobalSettingsPost() {
            if (!IsAuthorized()) return new HttpUnauthorizedResult();

            var settings = _seoSettingsManager.GetGlobalSettingsDraftRequired();
            var editor = _contentManager.UpdateEditor(settings, this);
            _contentManager.Publish(settings.ContentItem);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation, despite
                // being it highly unlikely with Onestop, just in case...
                return View((object)editor);
            }

            _orchardServices.Notifier.Information(T("Settings updated"));

            return RedirectToAction("GlobalSettings");
        }

        [HttpPost]
        public ActionResult RestoreGlobalDefault(string config) {
            if (!IsAuthorized()) return new HttpUnauthorizedResult();

            var settings = _seoSettingsManager.GetGlobalSettingsDraftRequired();

            switch (config) {
                case "TitleOverrideMaxLength":
                    settings.TitleOverrideMaxLength = Defaults.TitleOverrideMaxLength;
                    break;
                case "DescriptionOverrideMaxLength":
                    settings.DescriptionOverrideMaxLength = Defaults.DescriptionOverrideMaxLength;
                    break;
                case "KeywordsOverrideMaxLength":
                    settings.KeywordsOverrideMaxLength = Defaults.KeywordsOverrideMaxLength;
                    break;
                default:
                    return HttpNotFound();
            }

            _contentManager.Publish(settings.ContentItem);

            return this.RedirectToAction("GlobalSettings");
        }

        [HttpGet]
        public ActionResult Rewriter(RewriterViewModel rewriterViewModel, PagerParameters pagerParameters) {
            if (!IsAuthorized()) return new HttpUnauthorizedResult();

            if (rewriterViewModel.TypeName == "Dynamic") return DynamicPageRewriter(new DynamicPageRewriterViewModel { RewriterType = rewriterViewModel.RewriterType });

            var siteSettings = _workContext.CurrentSite;
            var pager = new Pager(siteSettings, pagerParameters);

            var seoContentTypes = _seoService.ListSeoContentTypes();
            if (string.IsNullOrEmpty(rewriterViewModel.TypeName)) return HttpNotFound();
            var typeDefinition = seoContentTypes.SingleOrDefault(t => t.Name == rewriterViewModel.TypeName);
            if (typeDefinition == null) return HttpNotFound();
            rewriterViewModel.TypeDisplayName = typeDefinition.DisplayName;
            _orchardServices.WorkContext.Layout.Title = TitleForRewriter(rewriterViewModel.RewriterType, typeDefinition.DisplayName);

            var query = _contentManager.Query(VersionOptions.Latest, rewriterViewModel.TypeName);

            if (!String.IsNullOrEmpty(rewriterViewModel.Q)) {
                IPageOfItems<ISearchHit> searchHits = new PageOfItems<ISearchHit>(new ISearchHit[] { });
                try
                {
                    var searchSettings = siteSettings.As<SearchSettingsPart>();
                    searchHits = _searchService.Query(rewriterViewModel.Q, pager.Page, pager.PageSize, false,
                                                      searchSettings.SearchIndex, SearchSettingsHelper.GetSearchFields(searchSettings, searchSettings.SearchIndex),
                                                      searchHit => searchHit);
                    // Could use this: http://orchard.codeplex.com/workitem/18664
                    // Converting to List, because the expression should contain an ICollection
                    var hitIds = searchHits.Select(hit => hit.ContentItemId).ToList();
                    query.Where<CommonPartRecord>(record => hitIds.Contains(record.Id));
                }
                catch (Exception ex) {
                    if (ex.IsFatal()) throw;
                    _orchardServices.Notifier.Error(T("Invalid search query: {0}", ex.Message));
                }
            }

            switch (rewriterViewModel.Options.OrderBy) {
                case ContentsOrder.Modified:
                    query = query.OrderByDescending<CommonPartRecord>(cr => cr.ModifiedUtc);
                    break;
                case ContentsOrder.Published:
                    query = query.OrderByDescending<CommonPartRecord>(cr => cr.PublishedUtc);
                    break;
                case ContentsOrder.Created:
                    query = query.OrderByDescending<CommonPartRecord>(cr => cr.CreatedUtc);
                    break;
            }

            rewriterViewModel.Options.SelectedFilter = rewriterViewModel.TypeName;

            var pagerShape = _shapeFactory.Pager(pager).TotalItemCount(query.Count());
            var pageOfContentItems = query.Slice(pager.GetStartIndex(), pager.PageSize).ToList();

            var list = _shapeFactory.List();
            list.AddRange(
                pageOfContentItems.Select(
                    item => _prefixedEditorManager.BuildShape(item, (content => _contentManager.BuildDisplay(content, "SeoSummaryAdmin-" + rewriterViewModel.RewriterType)))
                    )
                );

            dynamic viewModel = _shapeFactory.ViewModel()
                .ContentItems(list)
                .Options(rewriterViewModel.Options)
                .Pager(pagerShape);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation, despite
            // being it highly unlikely with Onestop, just in case...
            return View((object)viewModel);
        }

        [HttpPost, ActionName("Rewriter")]
        [FormValueRequired("submit.Search")]
        public ActionResult RewriterSearch(RewriterViewModel rewriterViewModel) {
            if (!IsAuthorized()) return new HttpUnauthorizedResult();

            // With this we clear other filters and order by settings first
            var routeValues = ControllerContext.RouteData.Values;
            routeValues["q"] = rewriterViewModel.Q;
            return RedirectToAction("Rewriter", routeValues);
        }

        public ActionResult DynamicPageRewriter(DynamicPageRewriterViewModel viewModel) {
            if (!IsAuthorized()) return new HttpUnauthorizedResult();

            if (string.IsNullOrEmpty(viewModel.Url)) {
                return View("DynamicPageRewriter.Lookup", viewModel);
            }
            else if (!Uri.IsWellFormedUriString(viewModel.Url, UriKind.Absolute)) {
                _orchardServices.Notifier.Error(T("The url you entered was not a valid full url."));
                return View("DynamicPageRewriter.Lookup", viewModel);
            }

            var item = GetDynamicPageItem(viewModel.Url);

            if (item == null) item = _contentManager.New("SeoDynamicPage");

            viewModel.EditorShape = _contentManager.BuildEditor(item);

            return View("DynamicPageRewriter.Edit", viewModel);
        }

        [HttpPost, ActionName("DynamicPageRewriter")]
        public ActionResult DynamicPageRewriterPost(DynamicPageRewriterViewModel viewModel) {
            if (!IsAuthorized()) return new HttpUnauthorizedResult();

            var item = GetDynamicPageItem(viewModel.Url);

            if (item == null) {
                item = _contentManager.New("SeoDynamicPage");
                if (!Uri.IsWellFormedUriString(viewModel.Url, UriKind.Absolute)) {
                    ModelState.AddModelError("UriMalformed", T("The url you entered was not a valid full url.").Text);
                }
                else item.As<SeoDynamicPagePart>().Path = new Uri(viewModel.Url).AbsolutePath;

                _contentManager.Create(item);
            }
            else {
                item = _contentManager.Get(item.Id, VersionOptions.DraftRequired);
            }

            viewModel.EditorShape = _contentManager.UpdateEditor(item, this);
            _contentManager.Publish(item);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();
                return View("DynamicPageRewriter.Edit", viewModel);
            }

            _orchardServices.Notifier.Information(T("Overrides for the dynamic page saved."));

            return this.RedirectToAction("Rewriter", new { Id = "Dynamic", RewriterType = viewModel.RewriterType });
        }


        private ContentItem GetDynamicPageItem(string url) {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) return null;

            var uri = new Uri(url);

            return _contentManager
                        .Query("SeoDynamicPage")
                        .Where<SeoDynamicPagePartRecord>(record => record.Path == uri.AbsolutePath)
                        .List()
                        .SingleOrDefault();
        }
    }
}