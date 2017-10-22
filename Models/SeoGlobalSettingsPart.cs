using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Onestop.Seo.Models {
    public class SeoGlobalSettingsPart : ContentPart, ISeoGlobalSettings {
        public string HomeTitle {
            get { return this.Retrieve(x => x.HomeTitle, default(string), true); }
            set { this.Store(x => x.HomeTitle, value, true); }
        }

        public string HomeDescription {
            get { return this.Retrieve(x => x.HomeDescription, default(string), true); }
            set { this.Store(x => x.HomeDescription, value, true); }
        }

        public string HomeKeywords {
            get { return this.Retrieve(x => x.HomeKeywords, default(string), true); }
            set { this.Store(x => x.HomeKeywords, value, true); }
        }

        private readonly LazyField<IEnumerable<ContentTypeDefinition>> _seoContentTypes = new LazyField<IEnumerable<ContentTypeDefinition>>();
        public LazyField<IEnumerable<ContentTypeDefinition>> SeoContentTypesField { get { return _seoContentTypes; } }
        public IEnumerable<ContentTypeDefinition> SeoContentTypes {
            get { return _seoContentTypes.Value; }
        }

        #region SEO patterns
        public IDictionary<string, string> TitlePatternsViewDictionary {
            get { return GetSeoPatternsViewDictionary(SeoParameterType.Title); }

            set { SetSeoPatternsViewDictionary(SeoParameterType.Title, value); }
        }

        public IDictionary<string, string> DescriptionPatternsViewDictionary {
            get { return GetSeoPatternsViewDictionary(SeoParameterType.Description); }

            set { SetSeoPatternsViewDictionary(SeoParameterType.Description, value); }
        }

        public IDictionary<string, string> KeywordsPatternsViewDictionary {
            get { return GetSeoPatternsViewDictionary(SeoParameterType.Keywords); }

            set { SetSeoPatternsViewDictionary(SeoParameterType.Keywords, value); }
        }

        /// <summary>
        /// Only for model binding
        /// </summary>
        private IDictionary<SeoParameterType, IDictionary<string, string>> _seoPatternsViewDictionary;
        private IDictionary<SeoParameterType, IDictionary<string, string>> SeoPatternsViewDictionary {
            get {
                return _seoPatternsViewDictionary ??
                       (_seoPatternsViewDictionary = new Dictionary<SeoParameterType, IDictionary<string, string>>());
            }
            set { _seoPatternsViewDictionary = value; }
        }

        private IDictionary<string, string> GetSeoPatternsViewDictionary(SeoParameterType type) {
            if (!SeoPatternsViewDictionary.ContainsKey(type)) {
                var viewDictionary = SeoPatternsViewDictionary[type] = SeoContentTypes.ToDictionary(definition => definition.Name, definition => "");

                if (SeoPatternsDictionary.Count != 0 && SeoPatternsDictionary.ContainsKey(type)) {
                    foreach (var pattern in SeoPatternsDictionary[type]) {
                        viewDictionary[pattern.Key] = pattern.Value;
                    }
                }
            }

            return SeoPatternsViewDictionary[type];
        }

        private void SetSeoPatternsViewDictionary(SeoParameterType type, IDictionary<string, string> dictionary) {
            SeoPatternsViewDictionary[type] = dictionary;
            SeoPatternsDictionary[type] = dictionary;
            SaveSeoPatternsDictionary();
        }


        public void SetSeoPattern(SeoParameterType type, string contentType, string pattern) {
            if (!SeoPatternsDictionary.ContainsKey(type)) SeoPatternsDictionary[type] = new Dictionary<string, string>();
            SeoPatternsDictionary[type][contentType] = pattern;
            SaveSeoPatternsDictionary();
        }

        public string GetSeoPattern(SeoParameterType type, string contentType) {
            if (!SeoPatternsDictionary.ContainsKey(type) || !SeoPatternsDictionary[type].ContainsKey(contentType)) return null;
            return SeoPatternsDictionary[type][contentType];
        }

        private IDictionary<SeoParameterType, IDictionary<string, string>> _seoPatternsDictionary;
        private IDictionary<SeoParameterType, IDictionary<string, string>> SeoPatternsDictionary {
            get {
                if (_seoPatternsDictionary == null) {
                    if (String.IsNullOrEmpty(SeoPatternsDefinition)) {
                        _seoPatternsDictionary = new Dictionary<SeoParameterType, IDictionary<string, string>>();
                    }
                    else {
                        var serializer = new JavaScriptSerializer();
                        var tempDictionary = serializer.Deserialize<Dictionary<string, IDictionary<string, string>>>(SeoPatternsDefinition);
                        _seoPatternsDictionary = tempDictionary.ToDictionary(entry => serializer.ConvertToType<SeoParameterType>(entry.Key), entry => entry.Value);
                    }
                }

                return _seoPatternsDictionary;
            }

            set {
                _seoPatternsDictionary = value;
                SaveSeoPatternsDictionary();
            }
        }

        private void SaveSeoPatternsDictionary() {
            // This converting from enum-keyed to string-keyed dictionary is necessary for JavaScriptSerializer.
            // See: http://stackoverflow.com/questions/2892910/problems-with-json-serialize-dictionaryenum-int32
            var tempDictionary = SeoPatternsDictionary.Keys.ToDictionary(key => key.ToString(), key => SeoPatternsDictionary[key]);
            SeoPatternsDefinition = new JavaScriptSerializer().Serialize(tempDictionary);
        }
        #endregion

        /// <summary>
        /// Serialized title patterns
        /// </summary>
        public string SeoPatternsDefinition {
            get { return this.Retrieve(x => x.SeoPatternsDefinition, default(string), true); }
            set { this.Store(x => x.SeoPatternsDefinition, value, true); }
        }

        public string SearchTitlePattern {
            get { return this.Retrieve(x => x.SearchTitlePattern, default(string), true); }
            set { this.Store(x => x.SearchTitlePattern, value, true); }
        }

        public bool EnableCanonicalUrls {
            get { return this.Retrieve(x => x.EnableCanonicalUrls, default(bool), true); }
            set { this.Store(x => x.EnableCanonicalUrls, value, true); }
        }

        public int TitleOverrideMaxLength {
            get { return this.Retrieve(x => x.TitleOverrideMaxLength, default(int), true); }
            set { this.Store(x => x.TitleOverrideMaxLength, value, true); }
        }

        public int DescriptionOverrideMaxLength {
            get { return this.Retrieve(x => x.DescriptionOverrideMaxLength, default(int), true); }
            set { this.Store(x => x.DescriptionOverrideMaxLength, value, true); }
        }

        public int KeywordsOverrideMaxLength {
            get { return this.Retrieve(x => x.KeywordsOverrideMaxLength, default(int), true); }
            set { this.Store(x => x.KeywordsOverrideMaxLength, value, true); }
        }

        public string RobotsTxtText {
            get { return this.Retrieve(x => x.RobotsTxtText, default(string), true); }
            set { this.Store(x => x.RobotsTxtText, value, true); }
        }
    }
}