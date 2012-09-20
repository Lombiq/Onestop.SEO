using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Onestop.Seo.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Onestop.Seo.Drivers {
    public class ProductsSeoGlobalSettingsPartDriver : ContentPartDriver<ProductsSeoGlobalSettingsPart> {
        protected override string Prefix {
            get { return "Onestop.Seo.ProductGlobalSettingsPart"; }
        }

        protected override DriverResult Editor(ProductsSeoGlobalSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_ProductsSeoGlobalSettings_TitlePatterns_Edit",
                () => shapeHelper.EditorTemplate(
                        TemplateName: "Parts.ProductsSeoGlobalSettings.TitlePatterns",
                        Model: part,
                        Prefix: Prefix));
        }

        protected override DriverResult Editor(ProductsSeoGlobalSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(ProductsSeoGlobalSettingsPart part, ExportContentContext context) {
            var partName = part.PartDefinition.Name;

            context.Element(partName).SetAttributeValue("ProductTitlePattern", part.ProductTitlePattern);
            context.Element(partName).SetAttributeValue("ProductCategoryTitlePattern", part.ProductCategoryTitlePattern);
        }

        protected override void Importing(ProductsSeoGlobalSettingsPart part, ImportContentContext context) {
            var partName = part.PartDefinition.Name;

            context.ImportAttribute(partName, "ProductTitlePattern", value => part.ProductTitlePattern = value);
            context.ImportAttribute(partName, "ProductCategoryTitlePattern", value => part.ProductCategoryTitlePattern = value);
        }
    }
}