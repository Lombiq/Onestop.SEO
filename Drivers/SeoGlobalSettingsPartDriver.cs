using Onestop.Seo.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Onestop.Seo.Drivers {
    public class SeoGlobalSettingsPartDriver : ContentPartDriver<SeoGlobalSettingsPart> {
        protected override string Prefix {
            get { return "Onestop.Seo.GlobalSettingsPart"; }
        }

        protected override DriverResult Editor(SeoGlobalSettingsPart part, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_SeoGlobalSettings_Homepage_Edit",
                             () => shapeHelper.EditorTemplate(
                                 TemplateName: "Parts.SeoGlobalSettings.Homepage",
                                 Model: part,
                                 Prefix: Prefix)),
                ContentShape("Parts_SeoGlobalSettings_TitlePatterns_Edit",
                             () => shapeHelper.EditorTemplate(
                                 TemplateName: "Parts.SeoGlobalSettings.TitlePatterns",
                                 Model: part,
                                 Prefix: Prefix)),
                ContentShape("Parts_SeoGlobalSettings_DescriptionPatterns_Edit",
                             () => shapeHelper.EditorTemplate(
                                 TemplateName: "Parts.SeoGlobalSettings.DescriptionPatterns",
                                 Model: part,
                                 Prefix: Prefix)),
                ContentShape("Parts_SeoGlobalSettings_KeywordsPatterns_Edit",
                             () => shapeHelper.EditorTemplate(
                                 TemplateName: "Parts.SeoGlobalSettings.KeywordsPatterns",
                                 Model: part,
                                 Prefix: Prefix)),
                ContentShape("Parts_SeoGlobalSettings_OtherSettings_Edit",
                             () => shapeHelper.EditorTemplate(
                                 TemplateName: "Parts.SeoGlobalSettings.OtherSettings",
                                 Model: part,
                                 Prefix: Prefix)),
                ContentShape("Parts_SeoGlobalSettings_RobotsTxt_Edit",
                            () => shapeHelper.EditorTemplate(
                                TemplateName: "Parts.SeoGlobalSettings.RobotsTxt",
                                Model: part,
                                Prefix: Prefix))
                );
        }

        protected override DriverResult Editor(SeoGlobalSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(SeoGlobalSettingsPart part, ExportContentContext context) {
            var element = context.Element(part.PartDefinition.Name);

            element.SetAttributeValue("HomeTitle", part.HomeTitle);
            element.SetAttributeValue("HomeDescription", part.HomeDescription);
            element.SetAttributeValue("HomeKeywords", part.HomeKeywords);
            element.SetAttributeValue("SeoPatternsDefinition", part.SeoPatternsDefinition);
            element.SetAttributeValue("SearchTitlePattern", part.SearchTitlePattern);
            element.SetAttributeValue("EnableCanonicalUrls", part.EnableCanonicalUrls);
            element.SetAttributeValue("TitleOverrideMaxLength", part.TitleOverrideMaxLength);
            element.SetAttributeValue("DescriptionOverrideMaxLength", part.DescriptionOverrideMaxLength);
            element.SetAttributeValue("KeywordsOverrideMaxLength", part.KeywordsOverrideMaxLength);
        }

        protected override void Importing(SeoGlobalSettingsPart part, ImportContentContext context) {
            var partName = part.PartDefinition.Name;

            context.ImportAttribute(partName, "HomeTitle", value => part.HomeTitle = value);
            context.ImportAttribute(partName, "HomeDescription", value => part.HomeDescription = value);
            context.ImportAttribute(partName, "HomeKeywords", value => part.HomeKeywords = value);
            context.ImportAttribute(partName, "SeoPatternsDefinition", value => part.SeoPatternsDefinition = value);
            context.ImportAttribute(partName, "SearchTitlePattern", value => part.SearchTitlePattern = value);
            context.ImportAttribute(partName, "EnableCanonicalUrls", value => part.EnableCanonicalUrls = bool.Parse(value));
            context.ImportAttribute(partName, "TitleOverrideMaxLength", value => part.TitleOverrideMaxLength = int.Parse(value));
            context.ImportAttribute(partName, "DescriptionOverrideMaxLength", value => part.DescriptionOverrideMaxLength = int.Parse(value));
            context.ImportAttribute(partName, "KeywordsOverrideMaxLength", value => part.KeywordsOverrideMaxLength = int.Parse(value));
        }
    }
}