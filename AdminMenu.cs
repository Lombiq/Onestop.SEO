using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.UI.Navigation;
using Orchard.Localization;

namespace Onestop.Seo {
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public AdminMenu() {
            T = NullLocalizer.Instance;
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder.Add(T("SEO"), "5", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu) {
            menu.LinkToFirstChild(false); // See: http://orchard.codeplex.com/workitem/18807
            menu.Action("Index", "Admin", new { area = "Onestop.Seo" }).Permission(Permissions.ManageSeo);
        }
    }
}