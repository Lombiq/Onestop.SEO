using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.UI.Admin;
using Orchard;
using Orchard.Security;
using Orchard.Localization;
using Orchard.ContentManagement;
using Onestop.Seo.Services;
using Orchard.UI.Notify;
using Orchard.UI.Navigation;
using Orchard.Settings;
using Onestop.Seo.Models;
using Orchard.DisplayManagement;
using Orchard.Core.Contents.ViewModels;
using Orchard.Core.Common.Models;
using Orchard.Core.Contents.Controllers;
using Orchard.DisplayManagement.Descriptors;

namespace Onestop.Seo.Controllers {
    [Admin]
    public class AdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly IContentManager _contentManager;
        private readonly dynamic _shapeFactory;

        private readonly IPrefixedEditorManager _prefixedEditorManager;
        private readonly ISiteService _siteService;
        private readonly ISeoSettingsManager _seoSettingsManager;
        private readonly ISeoService _seoService;

        public Localizer T { get; set; }

        public AdminController(
            IOrchardServices orchardServices,
            ISiteService siteService,
            IPrefixedEditorManager prefixedEditorManager,
            ISeoSettingsManager seoSettingsManager,
            ISeoService seoService) {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _contentManager = orchardServices.ContentManager;
            _shapeFactory = _orchardServices.New;

            _prefixedEditorManager = prefixedEditorManager;
            _siteService = siteService;
            _seoSettingsManager = seoSettingsManager;
            _seoService = seoService;

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
        public ActionResult Rewriter(string rewriterType, ListContentsViewModel listViewModel, PagerParameters pagerParameters) {
            // These Authorize() calls are mainly placeholders for future permissions, that's why they're copy-pasted around.
            if (!_authorizer.Authorize(Permissions.ManageSeo, T("You're not allowed to manage SEO settings.")))
                return new HttpUnauthorizedResult();

            string title;
            switch (rewriterType) {
                case "TitleRewriter":
                    title = T("SEO Title Tag Rewriter").Text;
                    break;
                case "DescriptionRewriter":
                    title = T("SEO Description Tag Rewriter").Text;
                    break;
                default:
                    return new HttpNotFoundResult();
            }
            _orchardServices.WorkContext.Layout.Title = title;

            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var seoContentTypes = _seoService.ListSeoContentTypes();
            var query = _contentManager.Query(VersionOptions.Latest, seoContentTypes.Select(type => type.Name).ToArray());

            if (!string.IsNullOrEmpty(listViewModel.TypeName)) {
                var typeDefinition = seoContentTypes.Where(t => t.Name == listViewModel.TypeName).SingleOrDefault();
                if (typeDefinition == null) return HttpNotFound();

                listViewModel.TypeDisplayName = typeDefinition.DisplayName;
                query = query.ForType(listViewModel.TypeName);
            }

            switch (listViewModel.Options.OrderBy) {
                case ContentsOrder.Modified:
                    //query = query.OrderByDescending<ContentPartRecord, int>(ci => ci.ContentItemRecord.Versions.Single(civr => civr.Latest).Id);
                    query = query.OrderByDescending<CommonPartRecord, DateTime?>(cr => cr.ModifiedUtc);
                    break;
                case ContentsOrder.Published:
                    query = query.OrderByDescending<CommonPartRecord, DateTime?>(cr => cr.PublishedUtc);
                    break;
                case ContentsOrder.Created:
                    //query = query.OrderByDescending<ContentPartRecord, int>(ci => ci.Id);
                    query = query.OrderByDescending<CommonPartRecord, DateTime?>(cr => cr.CreatedUtc);
                    break;
            }

            listViewModel.Options.SelectedFilter = listViewModel.TypeName;
            listViewModel.Options.FilterOptions = seoContentTypes
                .Select(ctd => new KeyValuePair<string, string>(ctd.Name, ctd.DisplayName))
                .ToList().OrderBy(kvp => kvp.Value);

            var pagerShape = _shapeFactory.Pager(pager).TotalItemCount(query.Count());
            var pageOfContentItems = query.Slice(pager.GetStartIndex(), pager.PageSize).ToList();

            var list = _shapeFactory.List();
            //foreach (var item in pageOfContentItems) {
            //    var shape = _contentManager.BuildDisplay(item, "SeoSummaryAdmin-" + rewriterType);
            //    list.Add(shape);
            //}
            list.AddRange(
                pageOfContentItems.Select(
                    item => _prefixedEditorManager.BuildShape(item, (content => _contentManager.BuildDisplay(content, "SeoSummaryAdmin-" + rewriterType)))
                    )
                );

            dynamic viewModel = _shapeFactory.ViewModel()
                .ContentItems(list)
                .Options(listViewModel.Options)
                .Pager(pagerShape);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation, despite
            // being it highly unlikely with Onestop, just in case...
            return View((object)viewModel);
        }

        [HttpPost, ActionName("Rewriter")]
        [FormValueRequired("submit.Filter")]
        public ActionResult RewriterFilterPost(string rewriterType, ContentOptions options) {
            var routeValues = ControllerContext.RouteData.Values;
            routeValues["rewriterType"] = rewriterType;
            if (options != null) {
                routeValues["Options.OrderBy"] = options.OrderBy; //todo: don't hard-code the key
                if (_seoService.ListSeoContentTypes().Any(t => t.Name.Equals(options.SelectedFilter, StringComparison.OrdinalIgnoreCase))) {
                    routeValues["id"] = options.SelectedFilter;
                }
                else {
                    routeValues.Remove("id");
                }
            }

            return RedirectToAction("Rewriter", routeValues);
        }

        [HttpPost, ActionName("Rewriter")]
        [FormValueRequired("submit.SaveAll")]
        public ActionResult RewriterSaveAllPost(string rewriterType, IEnumerable<int> itemIds) {
            var routeValues = ControllerContext.RouteData.Values;
            routeValues["rewriterType"] = rewriterType;

            foreach (var item in _contentManager.GetMany<IContent>(itemIds, VersionOptions.Latest, QueryHints.Empty)) {
                _prefixedEditorManager.UpdateEditor(item, this);
            }

            return RedirectToAction("Rewriter", routeValues);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}