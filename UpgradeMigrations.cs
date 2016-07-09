using Onestop.Seo.Models;
using Orchard.ContentManagement;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade.Services;

namespace Onestop.Seo {
    [OrchardFeature("Onestop.Seo.Upgrade")]
    public class UpgradeMigrations : DataMigrationImpl {
        private readonly IContentManager _contentManager;
        private readonly IUpgradeService _upgradeService;


        public UpgradeMigrations(IContentManager contentManager, IUpgradeService upgradeService) {
            _contentManager = contentManager;
            _upgradeService = upgradeService;
        }


        // Migrating versioned SeoGlobalSettingsPartRecord entries to versioned infoset.
        public int Create() {
            var tableName = _upgradeService.GetPrefixedTableName("Onestop_SEO_SeoGlobalSettingsPartRecord");

            IEnumerable<ContentItem> allVersions = null;

            if (_upgradeService.TableExists(tableName)) {
                _upgradeService.ExecuteReader(string.Format("SELECT * FROM {0}", tableName),
                    (reader, connection) => {
                        if (allVersions == null) allVersions = _contentManager.GetAllVersions((int)reader["ContentItemRecord_id"]);

                        var item = allVersions.FirstOrDefault(p => p.VersionRecord.Id == ConvertFromDBValue<int>(reader["Id"]));

                        if (item != null && item.Has<SeoGlobalSettingsPart>()) {
                            var part = item.As<SeoGlobalSettingsPart>();

                            part.HomeTitle = ConvertFromDBValue<string>(reader["HomeTitle"]);
                            part.HomeDescription = ConvertFromDBValue<string>(reader["HomeDescription"]);
                            part.HomeKeywords = ConvertFromDBValue<string>(reader["HomeKeywords"]);
                            part.SeoPatternsDefinition = ConvertFromDBValue<string>(reader["SeoPatternsDefinition"]);
                            part.SearchTitlePattern = ConvertFromDBValue<string>(reader["SearchTitlePattern"]);
                            part.EnableCanonicalUrls = ConvertFromDBValue<bool>(reader["EnableCanonicalUrls"]);
                            part.TitleOverrideMaxLength = ConvertFromDBValue<int>(reader["TitleOverrideMaxLength"]);
                            part.DescriptionOverrideMaxLength = ConvertFromDBValue<int>(reader["DescriptionOverrideMaxLength"]);
                            part.KeywordsOverrideMaxLength = ConvertFromDBValue<int>(reader["KeywordsOverrideMaxLength"]);
                            part.RobotsTxtText = ConvertFromDBValue<string>(reader["RobotsTxtText"]);
                        }
                    });

                _upgradeService.ExecuteReader(string.Format("DROP TABLE {0}", tableName), null);
            }


            return 1;
        }


        private static T ConvertFromDBValue<T>(object obj) {
            return (obj == null || obj == DBNull.Value) ? default(T) : (T)obj;
        }
    }
}