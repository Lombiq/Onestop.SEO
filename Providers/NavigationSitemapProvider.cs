using Onestop.Seo.Models;
using Orchard.Core.Navigation.Services;
using Orchard.Environment.Extensions;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Onestop.Seo.Providers {
    [OrchardFeature("Onestop.Seo.Sitemap")]
    public class NavigationSitemapProvider : ISitemapProvider {
        private readonly IMenuService _menuService;
        private readonly INavigationManager _navigationManager;

        public NavigationSitemapProvider(IMenuService menuService, INavigationManager navigationManager) {
            _menuService = menuService;
            _navigationManager = navigationManager;
        }

        public int Priority {
            get {
                return 5;
            }
        }

        private static SitemapEntry Entry(MenuItem menuItem) {
            return new SitemapEntry {Url = menuItem.Href, Name = menuItem.Text.ToString()};
        }

        private static IEnumerable<SitemapEntry> MakeUrls(MenuItem menuItem) {
            if (!String.IsNullOrEmpty(menuItem.Href)) {
                yield return Entry(menuItem);
            }

            foreach (var childItem in menuItem.Items) {
                foreach (var sitemapResult in MakeUrls(childItem)) {
                    yield return sitemapResult;
                }
            }
        }

        public IEnumerable<SitemapEntry> GetSitemapEntries() {
            return _menuService.GetMenus()
                .Select(menu => _menuService.GetMenu(menu.Id))
                .SelectMany(menu => _navigationManager.BuildMenu(menu))
                .SelectMany(MakeUrls);
        }
    }
}