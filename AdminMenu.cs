using System.Collections.Generic;
using Onestop.Seo.Services;
using Orchard.Localization;
using Orchard.UI.Navigation;
using System.Linq;
using System;

namespace Onestop.Seo {
    public class AdminMenu : INavigationProvider {
        private readonly ISeoService _seoService;

        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public AdminMenu(ISeoService seoService) {
            _seoService = seoService;

            T = NullLocalizer.Instance;
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder.Add(T("SEO"), "5", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu) {
            menu.LinkToFirstChild(false); // See: http://orchard.codeplex.com/workitem/18807
            menu.Action("GlobalSettings", "Admin", new { area = "Onestop.Seo" }).Permission(Permissions.ManageSeo);


            var seoContentTypes = _seoService.ListSeoContentTypes();


            RunThroughRewriters(menu, (rewriter, builder) => {
                int l = 1;
                if (seoContentTypes.Any()) {
                    foreach (var contentType in seoContentTypes) {
                        if (l == 1) {
                            builder.Action("Rewriter", "Admin", new { area = "Onestop.Seo", rewriterType = rewriter.Type, Id = contentType.Name });
                        }

                        builder
                            .Add(T(contentType.DisplayName), l.ToString(), tab => tab.Action("Rewriter", "Admin", new { area = "Onestop.Seo", rewriterType = rewriter.Type, Id = contentType.Name })
                                .LocalNav()
                                .Permission(Permissions.ManageSeo));

                        l++;
                    }
                }

                builder
                    .Add(T("Dynamic pages"), l.ToString(), tab => tab.Action("Rewriter", "Admin", new { area = "Onestop.Seo", rewriterType = rewriter.Type, Id = "Dynamic" })
                        .LocalNav()
                        .Permission(Permissions.ManageSeo));
            });
        }


        public static void RunThroughRewriters(NavigationItemBuilder menu, Action<Rewriter, NavigationItemBuilder> itemBuilder) {
            var rewriters = new List<Rewriter> {
                    new Rewriter { DisplayName = new LocalizedString("Title Tag Rewriter"), Type = "TitleRewriter" },
                    new Rewriter { DisplayName = new LocalizedString("Description Tag Rewriter"), Type = "DescriptionRewriter" },
                    new Rewriter { DisplayName = new LocalizedString("Keywords Tag Rewriter"), Type = "KeywordsRewriter" },
                    new Rewriter { DisplayName = new LocalizedString("Canonical Tag Rewriter"), Type = "CanonicalRewriter" },
                    new Rewriter { DisplayName = new LocalizedString("HTMLCard Rewriter"), Type = "HTMLCardRewriter" }

                };

            int i = 1;
            foreach (var rewriter in rewriters) {
                menu.Add(rewriter.DisplayName, i.ToString(), builder => itemBuilder(rewriter, builder));
                i++;
            }
        }


        public class Rewriter {
            public string Type { get; set; }
            public LocalizedString DisplayName { get; set; }
        }
    }
}