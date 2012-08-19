using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement;
using System.ComponentModel.DataAnnotations;
using Orchard.Data.Conventions;

namespace Onestop.Seo.Models {
    public class SeoPart : ContentPart<SeoPartRecord> {
        public string TitleOverride {
            get { return Record.TitleOverride; }
            set { Record.TitleOverride = value; }
        }

        public string DescriptionOverride {
            get { return Record.DescriptionOverride; }
            set { Record.DescriptionOverride = value; }
        }
    }

    public class SeoPartRecord : ContentPartVersionRecord {
        [StringLength(1024)]
        public virtual string TitleOverride { get; set; }

        [StringLengthMax]
        public virtual string DescriptionOverride { get; set; }
    }
}