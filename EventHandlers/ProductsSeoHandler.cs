using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Events;
using Onestop.Seo.Services;
using Orchard.Tokens;
using Orchard.ContentManagement;
using Onestop.Seo.Models;

namespace Onestop.Seo.EventHandlers {
    public interface IAnalyticsEvents : IEventHandler {
        void Category(dynamic category, string sort);
        void Product(dynamic product);
        void Checkout(dynamic cart);
        void Thanks(dynamic orderDetails);
    }

    public class ProductsSeoHandler : IAnalyticsEvents {
        private readonly ISeoSettingsManager _seoSettingsManager;
        private readonly ITokenizer _tokenizer;
        private readonly ISeoPageTitleBuilder _pageTitleBuilder;

        public ProductsSeoHandler(
            ISeoSettingsManager seoSettingsManager,
            ITokenizer tokenizer,
            ISeoPageTitleBuilder pageTitleBuilder) {
            _seoSettingsManager = seoSettingsManager;
            _tokenizer = tokenizer;
            _pageTitleBuilder = pageTitleBuilder;
        }

        public void Category(dynamic category, string sort) {
            var titlePattern = _seoSettingsManager.GetGlobalSettings().As<ProductsSeoGlobalSettingsPart>().ProductCategoryTitlePattern;
            if (String.IsNullOrEmpty(titlePattern)) return;

            var title = _tokenizer.Replace(
                        titlePattern,
                        new Dictionary<string, object> { { "ProductCategory", category } },
                        new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });

            _pageTitleBuilder.OverrideTitle(title);
        }

        public void Product(dynamic product) {
            var titlePattern = _seoSettingsManager.GetGlobalSettings().As<ProductsSeoGlobalSettingsPart>().ProductTitlePattern;
            if (String.IsNullOrEmpty(titlePattern)) return;

            var title = _tokenizer.Replace(
                        titlePattern,
                        new Dictionary<string, object> { { "Product", product } },
                        new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });

            _pageTitleBuilder.OverrideTitle(title);
        }

        public void Checkout(dynamic cart) {
        }

        public void Thanks(dynamic orderDetails) {
        }
    }
}