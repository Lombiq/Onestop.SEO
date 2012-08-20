using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement;
using System.ComponentModel.DataAnnotations;
using Orchard.Data.Conventions;
using Orchard.Core.Common.Utilities;

namespace Onestop.Seo.Models {
    public class SeoPart : ContentPart<SeoPartRecord> {
        private readonly LazyField<string> _generatedTitle = new LazyField<string>();
        public LazyField<string> GeneratedTitleField { get { return _generatedTitle; } }
        public string GeneratedTitle {
            get { return _generatedTitle.Value; }
        }

        private readonly LazyField<string> _generatedDescription = new LazyField<string>();
        public LazyField<string> GeneratedDescriptionField { get { return _generatedDescription; } }
        public string GeneratedDescription {
            get { return _generatedDescription.Value; }
        }

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