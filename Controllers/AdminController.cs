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
using Orchard.Core.Contents.Controllers;
using Orchard.Core.Contents.ViewModels;
using Orchard.Exceptions;
using Orchard.Indexing;
using Orchard.Localization;
using Orchard.Search.Models;
using Orchard.Search.Services;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;

namespace Onestop.Seo.Controllers {
    [Admin]
    public class AdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly IContentManager _contentManager;
        private readonly dynamic _shapeFactory;

        private readonly ISearchService _searchService;
        private readonly IPrefixedEditorManager _prefixedEditorManager;
        private readonly ISiteService _siteService;
        private readonly ISeoSettingsManager _seoSettingsManager;
        private readonly ISeoService _seoService;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public Localizer T { get; set; }

        public AdminController(
            IOrchardServices orchardServices,
            ISearchService searchService,
            ISiteService siteService,
            IPrefixedEditorManager prefixedEditorManager,
            ISeoSettingsManager seoSettingsManager,
            ISeoService seoService,
            IContentDefinitionManager contentDefinitionManager) {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _contentManager = orchardServices.ContentManager;
            _shapeFactory = _orchardServices.New;
            _searchService = searchService;

            _prefixedEditorManager = prefixedEditorManager;
            _siteService = siteService;
            _seoSettingsManager = seoSettingsManager;
            _seoService = seoService;
            _contentDefinitionManager = contentDefinitionManager;

            T = NullLocalizer.Instance;
        }

        // If there will be a need for extending global SEO settings it would perhaps also need the usage of separate settings into groups.
        // See site settings (Orchard.Core.Settings.Controllers.AdminController and friends for how it is done.
        public ActionResult GlobalSettings() {
            if (!_authorizer.Authorize(Permissions.ManageSeo, T("You're not allowed to manage SEO settings.")))
                return new HttpUnauthorizedResult();

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation, despite
            // being it highly unlikely with Onestop, just in case...
            return View((object)_contentManager.BuildEditor(_seoSettingsManager.GetGlobalSettings()));
        }

        [HttpPost, ActionName("GlobalSettings")]
        public ActionResult GlobalSettingsPost() {
            if (!_authorizer.Authorize(Permissions.ManageSeo, T("You're not allowed to manage SEO settings.")))
                return new HttpUnauthorizedResult();

            var editor = _seoSettingsManager.UpdateSettings(this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation, despite
                // being it highly unlikely with Onestop, just in case...
                return View((object)editor);
            }

            _orchardServices.Notifier.Information(T("Settings updated"));

            return RedirectToAction("GlobalSettings");
        }

        [HttpGet]
        public ActionResult Rewriter(RewriterViewModel rewriterViewModel, PagerParameters pagerParameters) {
            // These Authorize() calls are mainly placeholders for future permissions, that's why they're copy-pasted around.
            if (!_authorizer.Authorize(Permissions.ManageSeo, T("You're not allowed to manage SEO settings.")))
                return new HttpUnauthorizedResult();

            if (rewriterViewModel.TypeName == "Dynamic") return DynamicPageRewriter(new DynamicPageRewriterViewModel { RewriterType = rewriterViewModel.RewriterType });

            var siteSettings = _siteService.GetSiteSettings();
            var pager = new Pager(siteSettings, pagerParameters);

            var seoContentTypes = _seoService.ListSeoContentTypes();
            if (string.IsNullOrEmpty(rewriterViewModel.TypeName)) return HttpNotFound();
            var typeDefinition = seoContentTypes.SingleOrDefault(t => t.Name == rewriterViewModel.TypeName);
            if (typeDefinition == null) return HttpNotFound();
            rewriterViewModel.TypeDisplayName = typeDefinition.DisplayName;
            _orchardServices.WorkContext.Layout.Title = TitleForRewriter(T, rewriterViewModel.RewriterType, typeDefinition.DisplayName);

            var query = _contentManager.Query(VersionOptions.Latest, rewriterViewModel.TypeName);

            if (!String.IsNullOrEmpty(rewriterViewModel.Q)) {
                IPageOfItems<ISearchHit> searchHits = new PageOfItems<ISearchHit>(new ISearchHit[] { });
                try {
                    searchHits = _searchService.Query(rewriterViewModel.Q, pager.Page, pager.PageSize, false,
                                                      siteSettings.As<SearchSettingsPart>().SearchedFields,
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
        [FormValueRequired("submit.Filter")]
        public ActionResult RewriterFilterPost(RewriterViewModel rewriterViewModel) {
            var routeValues = ControllerContext.RouteData.Values;

            // Keeping search query if there's one
            if (!String.IsNullOrEmpty(rewriterViewModel.Q)) routeValues["q"] = rewriterViewModel.Q;

            if (rewriterViewModel.Options != null) {
                routeValues["Options.OrderBy"] = rewriterViewModel.Options.OrderBy; //todo: don't hard-code the key
            }

            return RedirectToAction("Rewriter", routeValues);
        }

        [HttpPost, ActionName("Rewriter")]
        [FormValueRequired("submit.SaveAll")]
        public ActionResult RewriterSaveAllPost(RewriterViewModel rewriterViewModel, IEnumerable<int> itemIds) {
            foreach (var itemId in itemIds) {
                var item = _contentManager.Get(itemId, VersionOptions.DraftRequired);
                _prefixedEditorManager.UpdateEditor(item, this);
                _contentManager.Publish(item);
            }

            // This would be better, but: http://orchard.codeplex.com/workitem/18979
            //foreach (var item in _contentManager.GetMany<IContent>(itemIds, VersionOptions.DraftRequired, QueryHints.Empty)) {
            //    _prefixedEditorManager.UpdateEditor(item, this);
            //    _contentManager.Publish(item.ContentItem);
            //}

            return RedirectToAction("Rewriter", ControllerContext.RouteData.Values);
        }

        [HttpPost, ActionName("Rewriter")]
        [FormValueRequired("submit.ClearAll")]
        public ActionResult RewriterClearAllPost(RewriterViewModel rewriterViewModel) {
            var itemIds = _contentManager
                .Query(_seoService.ListSeoContentTypes().Select(type => type.Name).ToArray())
                .List()
                .Select(item => item.Id);

            switch (rewriterViewModel.RewriterType) {
                case "TitleRewriter":
                    foreach (var itemId in itemIds) {
                        var item = _contentManager.Get<SeoPart>(itemId, VersionOptions.DraftRequired);
                        item.TitleOverride = null;
                        _contentManager.Publish(item.ContentItem);
                    }
                    break;
                case "DescriptionRewriter":
                    foreach (var itemId in itemIds) {
                        var item = _contentManager.Get<SeoPart>(itemId, VersionOptions.DraftRequired);
                        item.DescriptionOverride = null;
                        _contentManager.Publish(item.ContentItem);
                    }
                    break;
                case "KeywordsRewriter":
                    foreach (var itemId in itemIds) {
                        var item = _contentManager.Get<SeoPart>(itemId, VersionOptions.DraftRequired);
                        item.KeywordsOverride = null;
                        _contentManager.Publish(item.ContentItem);
                    }
                    break;
                default:
                    return new HttpNotFoundResult();
            }

            // This would be better, but: http://orchard.codeplex.com/workitem/18979
            //var items = _contentManager
            //                .Query(VersionOptions.DraftRequired, _seoService.ListSeoContentTypes().Select(type => type.Name).ToArray())
            //                .Join<SeoPartRecord>()
            //                .List<SeoPart>();

            //switch (rewriterViewModel.RewriterType) {
            //    case "TitleRewriter":
            //        foreach (var item in items) {
            //            item.TitleOverride = null;
            //            _contentManager.Publish(item.ContentItem);
            //        }
            //        break;
            //    case "DescriptionRewriter":
            //        foreach (var item in items) {
            //            item.DescriptionOverride = null;
            //            _contentManager.Publish(item.ContentItem);
            //        }
            //        break;
            //    case "KeywordsRewriter":
            //        foreach (var item in items) {
            //            item.KeywordsOverride = null;
            //            _contentManager.Publish(item.ContentItem);
            //        }
            //        break;
            //    default:
            //        return new HttpNotFoundResult();
            //}

            return RedirectToAction("Rewriter", ControllerContext.RouteData.Values);
        }

        [HttpPost, ActionName("Rewriter")]
        [FormValueRequired("submit.SaveIndividual")]
        public ActionResult RewriterSaveIndividual(RewriterViewModel rewriterViewModel, [Bind(Prefix = "submit.SaveIndividual")]int itemId) {
            var item = _contentManager.Get(itemId, VersionOptions.DraftRequired);

            if (item == null) return new HttpNotFoundResult();

            _prefixedEditorManager.UpdateEditor(item, this);
            _contentManager.Publish(item);

            return RedirectToAction("Rewriter", ControllerContext.RouteData.Values);
        }

        [HttpPost, ActionName("Rewriter")]
        [FormValueRequired("submit.Search")]
        public ActionResult RewriterSearch(RewriterViewModel rewriterViewModel) {
            // With this we clear other filters and order by settings first
            var routeValues = ControllerContext.RouteData.Values;
            routeValues["q"] = rewriterViewModel.Q;
            return RedirectToAction("Rewriter", routeValues);
        }

        public ActionResult DynamicPageRewriter(DynamicPageRewriterViewModel viewModel) {
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

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }


        public static LocalizedString TitleForRewriter(Localizer localizer, string rewriterType, string tabName) {
            switch (rewriterType) {
                case "TitleRewriter":
                    return localizer("SEO Title Tag Rewriter: {0}", tabName);
                case "DescriptionRewriter":
                    return localizer("SEO Description Tag Rewriter: {0}", tabName);
                default: //case "KeywordsRewriter"
                    return localizer("SEO Keywords Tag Rewriter: {0}", tabName);
            }
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