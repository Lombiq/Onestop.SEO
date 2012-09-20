using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace Onestop.Seo.Models {
    public class ProductsSeoGlobalSettingsPart : ContentPart<ProductsSeoGlobalSettingsPartRecord> {
        public string ProductTitlePattern {
            get { return Record.ProductTitlePattern; }
            set { Record.ProductTitlePattern = value; }
        }

        public string ProductCategoryTitlePattern {
            get { return Record.ProductCategoryTitlePattern; }
            set { Record.ProductCategoryTitlePattern = value; }
        }
    }

    public class ProductsSeoGlobalSettingsPartRecord : ContentPartVersionRecord {
        [StringLength(1024)]
        public virtual string ProductTitlePattern { get; set; }

        [StringLength(1024)]
        public virtual string ProductCategoryTitlePattern { get; set; }
    }
}