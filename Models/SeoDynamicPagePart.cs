using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Onestop.Seo.Models {
    /// <summary>
    /// Used to have a content item corresponding to a "dynamic page" (i.e. a page that is not a content item, not a commerce entity but a page
    /// displayed from a normal action).
    /// </summary>
    public class SeoDynamicPagePart : ContentPart<SeoDynamicPagePartRecord> {
        public string Path {
            get { return Retrieve(p => p.Path); }
            set { Store(p => p.Path, value); }
        }
    }

    public class SeoDynamicPagePartRecord : ContentPartRecord {
        [StringLength(1024)]
        public virtual string Path { get; set; }
    }
}