﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Data.Conventions;
using System.Web.Script.Serialization;
using Orchard.Core.Common.Utilities;
using Orchard.ContentManagement.MetaData.Models;

namespace Onestop.Seo.Models {
    public class SeoGlobalSettingsPart : ContentPart<SeoGlobalSettingsPartRecord>, ISeoGlobalSettings {
        public string HomeTitle {
            get { return Record.HomeTitle; }
            set { Record.HomeTitle = value; }
        }

        public string HomeDescription {
            get { return Record.HomeDescription; }
            set { Record.HomeDescription = value; }
        }

        public string HomeKeywords {
            get { return Record.HomeKeywords; }
            set { Record.HomeKeywords = value; }
        }

        private readonly LazyField<IEnumerable<ContentTypeDefinition>> _seoContentTypes = new LazyField<IEnumerable<ContentTypeDefinition>>();
        public LazyField<IEnumerable<ContentTypeDefinition>> SeoContentTypesField { get { return _seoContentTypes; } }
        public IEnumerable<ContentTypeDefinition> SeoContentTypes {
            get { return _seoContentTypes.Value; }
        }

        #region Content title patterns
        private IDictionary<string, string> _titlePatternsViewDictionary;
        /// <summary>
        /// Only for model binding
        /// </summary>
        public IDictionary<string, string> TitlePatternsViewDictionary { // Better name?
            get {
                if (_titlePatternsViewDictionary == null) {
                    _titlePatternsViewDictionary = SeoContentTypes.ToDictionary(definition => definition.Name, definition => "");

                    if (TitlePatternsDictionary.Count != 0) {
                        foreach (var pattern in TitlePatternsDictionary) {
                            _titlePatternsViewDictionary[pattern.Key] = pattern.Value;
                        }
                    }
                }

                return _titlePatternsViewDictionary;
            }

            set {
                _titlePatternsViewDictionary = value;
                TitlePatternsDictionary = value;
            }
        }

        public void SetTitlePattern(string contentType, string pattern) {
            TitlePatternsDictionary[contentType] = pattern;
            SaveTitlePatternsDictionary();
        }

        public string GetTitlePattern(string contentType) {
            if (!TitlePatternsDictionary.ContainsKey(contentType)) return null;
            return TitlePatternsDictionary[contentType];
        }

        private IDictionary<string, string> _titlePatternsDictionary;
        private IDictionary<string, string> TitlePatternsDictionary {
            get {
                if (_titlePatternsDictionary == null) {
                    if (String.IsNullOrEmpty(ContentTitlePatternsDefinition)) {
                        _titlePatternsDictionary = new Dictionary<string, string>();
                    }
                    else {
                        _titlePatternsDictionary = new JavaScriptSerializer().Deserialize<IDictionary<string, string>>(ContentTitlePatternsDefinition);
                    }
                }

                return _titlePatternsDictionary;
            }

            set {
                _titlePatternsDictionary = value;
                SaveTitlePatternsDictionary();
            }
        }

        private void SaveTitlePatternsDictionary() {
            ContentTitlePatternsDefinition = new JavaScriptSerializer().Serialize(TitlePatternsDictionary);
        }

        /// <summary>
        /// Serialized title patterns
        /// </summary>
        public string ContentTitlePatternsDefinition {
            get { return Record.ContentTitlePatternsDefinition; }
            set { Record.ContentTitlePatternsDefinition = value; }
        }
        #endregion

        public string SearchTitlePattern {
            get { return Record.SearchTitlePattern; }
            set { Record.SearchTitlePattern = value; }
        }

        public string ProductTitlePattern {
            get { return Record.ProductTitlePattern; }
            set { Record.ProductTitlePattern = value; }
        }

        public string ProductCategoryTitlePattern {
            get { return Record.ProductCategoryTitlePattern; }
            set { Record.ProductCategoryTitlePattern = value; }
        }

        public bool EnableCanonicalUrls {
            get { return Record.EnableCanonicalUrls; }
            set { Record.EnableCanonicalUrls = value; }
        }

        public bool UseCategoriesForKeywords {
            get { return Record.UseCategoriesForKeywords; }
            set { Record.UseCategoriesForKeywords = value; }
        }

        public bool AutogenerateDescriptions {
            get { return Record.AutogenerateDescriptions; }
            set { Record.AutogenerateDescriptions = value; }
        }
    }

    public class SeoGlobalSettingsPartRecord : ContentPartVersionRecord {
        [StringLength(1024)]
        public virtual string HomeTitle { get; set; }

        [StringLengthMax]
        public virtual string HomeDescription { get; set; }

        [StringLengthMax]
        public virtual string HomeKeywords { get; set; }

        [StringLengthMax]
        public virtual string ContentTitlePatternsDefinition { get; set; }

        [StringLength(1024)]
        public virtual string SearchTitlePattern { get; set; }

        [StringLength(1024)]
        public virtual string ProductTitlePattern { get; set; }

        [StringLength(1024)]
        public virtual string ProductCategoryTitlePattern { get; set; }

        public virtual bool EnableCanonicalUrls { get; set; }

        public virtual bool UseCategoriesForKeywords { get; set; }

        public virtual bool AutogenerateDescriptions { get; set; }
    }
}