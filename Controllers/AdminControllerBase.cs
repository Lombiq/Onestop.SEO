using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Onestop.Seo.Models;
using Onestop.Seo.Services;
using Onestop.Seo.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Contents.Controllers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;

namespace Onestop.Seo.Controllers {
    // Used in Commerce.Products.Seo too.
    [Admin]
    public abstract class AdminControllerBase : Controller, IUpdateModel {
        protected readonly IOrchardServices _orchardServices;
        protected readonly IAuthorizer _authorizer;
        protected readonly IContentManager _contentManager;
        protected readonly dynamic _shapeFactory;
        protected readonly WorkContext _workContext;
        protected readonly ITransactionManager _transactionManager;

        protected readonly IPrefixedEditorManager _prefixedEditorManager;
        protected readonly ISeoSettingsManager _seoSettingsManager;
        protected readonly ISeoService _seoService;

        public Localizer T { get; set; }


        protected AdminControllerBase(
            IOrchardServices orchardServices,
            IPrefixedEditorManager prefixedEditorManager,
            ISeoSettingsManager seoSettingsManager,
            ISeoService seoService) {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _contentManager = orchardServices.ContentManager;
            _shapeFactory = orchardServices.New;
            _workContext = orchardServices.WorkContext;
            _transactionManager = orchardServices.TransactionManager;

            _prefixedEditorManager = prefixedEditorManager;
            _seoSettingsManager = seoSettingsManager;
            _seoService = seoService;

            T = NullLocalizer.Instance;
        }


        [HttpPost, ActionName("Rewriter")]
        [FormValueRequired("submit.Filter")]
        public ActionResult RewriterFilterPost(RewriterViewModel rewriterViewModel) {
            if (!IsAuthorized()) return new HttpUnauthorizedResult();

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
            if (!IsAuthorized()) return new HttpUnauthorizedResult();

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
            if (!IsAuthorized()) return new HttpUnauthorizedResult();

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
            if (!IsAuthorized()) return new HttpUnauthorizedResult();

            var item = _contentManager.Get(itemId, VersionOptions.DraftRequired);

            if (item == null) return new HttpNotFoundResult();

            var part = item.As<SeoPart>();

            /* You cannot update a single part without hiding everything else in placement... so you cannoy call updateeditro here */
            UpdateModel(part, PrefixedEditorManager.AttachPrefixToPrefix(itemId, "Onestop.Seo.SeoPart"));

            if (part.TitleOverride == part.GeneratedTitle) part.TitleOverride = null;
            if (part.DescriptionOverride == part.GeneratedDescription) part.DescriptionOverride = null;
            if (part.KeywordsOverride == part.GeneratedKeywords) part.KeywordsOverride = null;
            
            _contentManager.Publish(item);

            return RedirectToAction("Rewriter", ControllerContext.RouteData.Values);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }


        protected LocalizedString TitleForRewriter(string rewriterType, string tabName) {
            switch (rewriterType) {
                case "TitleRewriter":
                    return T("SEO Title Tag Rewriter: {0}", tabName);
                case "DescriptionRewriter":
                    return T("SEO Description Tag Rewriter: {0}", tabName);
                default: //case "KeywordsRewriter"
                    return T("SEO Keywords Tag Rewriter: {0}", tabName);
            }
        }

        protected bool IsAuthorized() {
            return _authorizer.Authorize(Onestop.Seo.Permissions.ManageSeo, T("You're not allowed to manage SEO settings."));
        }
    }
}