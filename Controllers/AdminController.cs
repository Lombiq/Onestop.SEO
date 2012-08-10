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

namespace Onestop.Seo.Controllers {
    [Admin]
    public class AdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly IContentManager _contentManager;
        private readonly ISeoService _seoService;

        public Localizer T { get; set; }

        public AdminController(
            IOrchardServices orchardServices,
            ISeoService seoService) {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _contentManager = orchardServices.ContentManager;

            _seoService = seoService;

            T = NullLocalizer.Instance;
        }

        // If there will be a need for extending global SEO settings it would perhaps also need the usage of separate settings into groups.
        // See site settings (Orchard.Core.Settings.Controllers.AdminController and friends for how it is done.
        public ActionResult Index() {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageSeo, T("You're not allowed to manage SEO settings.")))
                return new HttpUnauthorizedResult();

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation, despite
            // being it highly unlikely with Onestop, just in case...
            return View((object)_contentManager.BuildEditor(_seoService.GetGlobalSettings()));
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPost() {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageSeo, T("You're not allowed to manage SEO settings.")))
                return new HttpUnauthorizedResult();

            var editor = _seoService.UpdateSettings(this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation, despite
                // being it highly unlikely with Onestop, just in case...
                return View((object)editor);
            }

            _orchardServices.Notifier.Information(T("Settings updated"));

            return RedirectToAction("Index");
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}