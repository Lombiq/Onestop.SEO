using Orchard.Localization;
using Orchard.UI.Navigation;

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
            menu.Action("GlobalSettings", "Admin", new { area = "Onestop.Seo" }).Permission(Permissions.ManageSeo);

            menu.Add(T("Title Tag Rewriter"), "1",
                item => item.Action("Rewriter", "Admin", new { area = "Onestop.Seo", rewriterType = "TitleRewriter" }).Permission(Permissions.ManageSeo));
            menu.Add(T("Description Tag Rewriter"), "2",
                item => item.Action("Rewriter", "Admin", new { area = "Onestop.Seo", rewriterType = "DescriptionRewriter" }).Permission(Permissions.ManageSeo));
            menu.Add(T("Keywords Tag Rewriter"), "2",
                item => item.Action("Rewriter", "Admin", new { area = "Onestop.Seo", rewriterType = "KeywordsRewriter" }).Permission(Permissions.ManageSeo));
        }
    }
}