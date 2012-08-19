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

namespace Onestop.Seo.Controllers {
    [Admin]
    public class AdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly IContentManager _contentManager;
        private readonly dynamic _shapeFactory;
        private readonly ISiteService _siteService;
        private readonly ISeoService _seoService;

        public Localizer T { get; set; }

        public AdminController(
            IOrchardServices orchardServices,
            ISiteService siteService,
            ISeoService seoService) {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _contentManager = orchardServices.ContentManager;
            _shapeFactory = _orchardServices.New;

            _siteService = siteService;
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
            return View((object)_contentManager.BuildEditor(_seoService.GetGlobalSettings()));
        }

        [HttpPost, ActionName("GlobalSettings")]
        public ActionResult GlobalSettingsPost() {
            if (!_authorizer.Authorize(Permissions.ManageSeo, T("You're not allowed to manage SEO settings.")))
                return new HttpUnauthorizedResult();

            var editor = _seoService.UpdateSettings(this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation, despite
                // being it highly unlikely with Onestop, just in case...
                return View((object)editor);
            }

            _orchardServices.Notifier.Information(T("Settings updated"));

            return RedirectToAction("GlobalSettings");
        }

        public ActionResult TitleRewriter(PagerParameters pagerParameters) {
            return Rewriter(pagerParameters, "TitleRewriter");
        }

        public ActionResult DescriptionRewriter(PagerParameters pagerParameters) {
            return Rewriter(pagerParameters, "DescriptionRewriter");
        }

        private ActionResult Rewriter(PagerParameters pagerParameters, string group) {
            // These Authorize() calls are mainly placeholders for future permissions, that's why they're copy-pasted around.
            if (!_authorizer.Authorize(Permissions.ManageSeo, T("You're not allowed to manage SEO settings.")))
                return new HttpUnauthorizedResult();

            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var seoContentTypes = _contentManager.GetContentTypeDefinitions().Where(t => t.Parts.Any(p => p.PartDefinition.Name == typeof(SeoPart).Name));
            var query = _contentManager.Query(VersionOptions.Latest, seoContentTypes.Select(type => type.Name).ToArray());

            var pagerShape = _shapeFactory.Pager(pager).TotalItemCount(query.Count());
            var pageOfContentItems = query.Slice(pager.GetStartIndex(), pager.PageSize).ToList();

            var list = _shapeFactory.List();
            list.AddRange(pageOfContentItems.Select(item => _contentManager.BuildDisplay(item, "SeoSummaryAdmin", group)));

            dynamic viewModel = _shapeFactory.ViewModel()
                .ContentItems(list)
                .Pager(pagerShape);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation, despite
            // being it highly unlikely with Onestop, just in case...
            return View((object)viewModel);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}