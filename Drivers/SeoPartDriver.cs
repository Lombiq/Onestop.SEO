using Onestop.Seo.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Onestop.Seo.Drivers {
    public class SeoPartDriver : ContentPartDriver<SeoPart> {
        protected override string Prefix {
            get { return "Onestop.Seo.SeoPart"; }
        }

        protected override DriverResult Display(SeoPart part, string displayType, dynamic shapeHelper) {
            return Editor(part, shapeHelper);
        }

        protected override DriverResult Editor(SeoPart part, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_Seo_SeoSummaryAdmin_Edit",
                    () => shapeHelper.EditorTemplate(
                            TemplateName: "Parts.Seo.SeoSummaryAdmin",
                            Model: part,
                            Prefix: Prefix)),
                ContentShape("Parts_Seo_TitleOverride_SeoSummaryAdmin_Edit",
                    () => shapeHelper.EditorTemplate(
                            TemplateName: "Parts.Seo.TitleOverride.SeoSummaryAdmin",
                            Model: part,
                            Prefix: Prefix)),
                ContentShape("Parts_Seo_DescriptionOverride_SeoSummaryAdmin_Edit",
                    () => shapeHelper.EditorTemplate(
                            TemplateName: "Parts.Seo.DescriptionOverride.SeoSummaryAdmin",
                            Model: part,
                            Prefix: Prefix)),
                ContentShape("Parts_Seo_KeywordsOverride_SeoSummaryAdmin_Edit",
                    () => shapeHelper.EditorTemplate(
                            TemplateName: "Parts.Seo.KeywordsOverride.SeoSummaryAdmin",
                            Model: part,
                            Prefix: Prefix)),
                ContentShape("Parts_Seo_Edit", // Generic editor, not shown by default for any content types
                    () => shapeHelper.EditorTemplate(
                            TemplateName: "Parts.Seo",
                            Model: part,
                            Prefix: Prefix)));
        }

        protected override DriverResult Editor(SeoPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);

            if (part.TitleOverride == part.GeneratedTitle) part.TitleOverride = null;
            if (part.DescriptionOverride == part.GeneratedDescription) part.DescriptionOverride = null;
            if (part.KeywordsOverride == part.GeneratedKeywords) part.KeywordsOverride = null;

            return Editor(part, shapeHelper);
        }

        protected override void Exporting(SeoPart part, ExportContentContext context) {
            var element = context.Element(part.PartDefinition.Name);

            element.SetAttributeValue("TitleOverride", part.TitleOverride);
            element.SetAttributeValue("DescriptionOverride", part.DescriptionOverride);
            element.SetAttributeValue("KeywordsOverride", part.KeywordsOverride);
        }

        protected override void Importing(SeoPart part, ImportContentContext context) {
            var partName = part.PartDefinition.Name;

            context.ImportAttribute(partName, "TitleOverride", value => part.TitleOverride = value);
            context.ImportAttribute(partName, "DescriptionOverride", value => part.DescriptionOverride = value);
            context.ImportAttribute(partName, "KeywordsOverride", value => part.KeywordsOverride = value);
        }
    }
}