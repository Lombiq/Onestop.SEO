using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Onestop.Seo.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;

namespace Onestop.Seo.Drivers {
    public class SeoOverridesPartDriver : ContentPartDriver<SeoOverridesPart> {
        protected override string Prefix {
            get { return "Onestop.Seo.SeoOverridesPart"; }
        }

        protected override DriverResult Display(SeoOverridesPart part, string displayType, dynamic shapeHelper) {
            return Editor(part, shapeHelper);
        }

        protected override DriverResult Editor(SeoOverridesPart part, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_SeoOverrides_Title_SeoSummaryAdmin_Edit",
                    () => shapeHelper.EditorTemplate(
                            TemplateName: "Parts.SeoOverrides.Title.SeoSummaryAdmin",
                            Model: part,
                            Prefix: Prefix)).OnGroup("TitleRewriter"),
                ContentShape("Parts_SeoOverrides_Description_SeoSummaryAdmin_Edit",
                    () => shapeHelper.EditorTemplate(
                            TemplateName: "Parts.SeoOverrides.Description.SeoSummaryAdmin",
                            Model: part,
                            Prefix: Prefix)).OnGroup("DescriptionRewriter"));
        }

        protected override DriverResult Editor(SeoOverridesPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(SeoOverridesPart part, ExportContentContext context) {
            var partName = part.PartDefinition.Name;

            context.Element(partName).SetAttributeValue("TitleOverride", part.TitleOverride);
            context.Element(partName).SetAttributeValue("DescriptionOverride", part.DescriptionOverride);
        }

        protected override void Importing(SeoOverridesPart part, ImportContentContext context) {
            var partName = part.PartDefinition.Name;

            context.ImportAttribute(partName, "TitleOverride", value => part.TitleOverride = value);
            context.ImportAttribute(partName, "DescriptionOverride", value => part.DescriptionOverride = value);
        }
    }
}