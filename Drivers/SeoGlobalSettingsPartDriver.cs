﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Drivers;
using Onestop.Seo.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;

namespace Onestop.Seo.Drivers {
    public class SeoGlobalSettingsPartDriver : ContentPartDriver<SeoGlobalSettingsPart> {
        protected override string Prefix {
            get { return "Onestop.Seo.GlobalSettingsPart"; }
        }

        protected override DriverResult Editor(SeoGlobalSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_SeoGlobalSettings_Edit",
                () => {
                    return shapeHelper.EditorTemplate(
                        TemplateName: "Parts.SeoGlobalSettings",
                        Model: part,
                        Prefix: Prefix);
                });
        }

        protected override DriverResult Editor(SeoGlobalSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(SeoGlobalSettingsPart part, ExportContentContext context) {
            var partName = part.PartDefinition.Name;

            context.Element(partName).SetAttributeValue("HomeTitle", part.HomeTitle);
            context.Element(partName).SetAttributeValue("HomeDescription", part.HomeDescription);
            context.Element(partName).SetAttributeValue("HomeKeywords", part.HomeKeywords);
            context.Element(partName).SetAttributeValue("ContentTitlePatternsDefinition", part.ContentTitlePatternsDefinition);
            context.Element(partName).SetAttributeValue("EnableCanonicalUrls", part.EnableCanonicalUrls);
            context.Element(partName).SetAttributeValue("UseCategoriesForKeywords", part.UseCategoriesForKeywords);
            context.Element(partName).SetAttributeValue("AutogenerateDescriptions", part.AutogenerateDescriptions);
        }

        protected override void Importing(SeoGlobalSettingsPart part, ImportContentContext context) {
            var partName = part.PartDefinition.Name;

            context.ImportAttribute(partName, "HomeTitle", value => part.HomeTitle = value);
            context.ImportAttribute(partName, "HomeDescription", value => part.HomeDescription = value);
            context.ImportAttribute(partName, "HomeKeywords", value => part.HomeKeywords = value);
            context.ImportAttribute(partName, "ContentTitlePatternsDefinition", value => part.ContentTitlePatternsDefinition = value);
            context.ImportAttribute(partName, "EnableCanonicalUrls", value => part.EnableCanonicalUrls = bool.Parse(value));
            context.ImportAttribute(partName, "UseCategoriesForKeywords", value => part.UseCategoriesForKeywords = bool.Parse(value));
            context.ImportAttribute(partName, "AutogenerateDescriptions", value => part.AutogenerateDescriptions = bool.Parse(value));
        }
    }
}