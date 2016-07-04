using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Onestop.Seo.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Onestop.Seo.Drivers {
    public class SeoDynamicPagePartDriver : ContentPartDriver<SeoDynamicPagePart> {
        protected override void Exporting(SeoDynamicPagePart part, ExportContentContext context) {
            var element = context.Element(part.PartDefinition.Name);

            element.SetAttributeValue("Path", part.Path);
        }

        protected override void Importing(SeoDynamicPagePart part, ImportContentContext context) {
            var partName = part.PartDefinition.Name;

            context.ImportAttribute(partName, "Path", value => part.Path = value);
        }
    }
}