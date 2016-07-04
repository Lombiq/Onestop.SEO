using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;
using Orchard.ContentManagement.Utilities;

namespace Onestop.Seo.Models {
    public class SeoPart : ContentPart<SeoPartRecord> {
        private readonly LazyField<ISeoGlobalSettings> _globalSettings = new LazyField<ISeoGlobalSettings>();
        public LazyField<ISeoGlobalSettings> GlobalSettingsField { get { return _globalSettings; } }
        public ISeoGlobalSettings GlobalSettings {
            get { return _globalSettings.Value; }
        }

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

        private readonly LazyField<string> _generatedKeywords = new LazyField<string>();
        public LazyField<string> GeneratedKeywordsField { get { return _generatedKeywords; } }
        public string GeneratedKeywords {
            get { return _generatedKeywords.Value; }
        }

        public string TitleOverride {
            get { return Retrieve(p => p.TitleOverride); }
            set { Store(p => p.TitleOverride, value); }
        }

        public string DescriptionOverride {
            get { return Retrieve(p => p.DescriptionOverride); }
            set { Store(p => p.DescriptionOverride, value); }
        }

        public string KeywordsOverride {
            get { return Retrieve(p => p.KeywordsOverride); }
            set { Store(p => p.KeywordsOverride, value); }
        }
    }

    public class SeoPartRecord : ContentPartVersionRecord {
        [StringLength(1024)]
        public virtual string TitleOverride { get; set; }

        [StringLengthMax]
        public virtual string DescriptionOverride { get; set; }

        [StringLengthMax]
        public virtual string KeywordsOverride { get; set; }
    }
}