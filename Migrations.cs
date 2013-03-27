using Onestop.Seo.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Onestop.Seo {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable(typeof(SeoGlobalSettingsPartRecord).Name,
                table => table
                    .ContentPartVersionRecord()
                    .Column<string>("HomeTitle", column => column.WithLength(1024))
                    .Column<string>("HomeDescription", column => column.Unlimited())
                    .Column<string>("HomeKeywords", column => column.Unlimited())
                    .Column<string>("SeoPatternsDefinition", column => column.Unlimited())
                    .Column<string>("SearchTitlePattern", column => column.WithLength(1024))
                    .Column<bool>("EnableCanonicalUrls")
                    .Column<int>("TitleOverrideMaxLength")
                    .Column<int>("DescriptionOverrideMaxLength")
                    .Column<int>("KeywordsOverrideMaxLength")
                );

            // Creating the type in the migration is necessary for CommonPart what in turn is necessary for Audit Trail
            ContentDefinitionManager.AlterTypeDefinition("SeoSettings",
                cfg => cfg
                    .WithPart("CommonPart", p => p
                        .WithSetting("DateEditorSettings.ShowDateEditor", "false")
                        .WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false"))
                    .WithPart(typeof(SeoGlobalSettingsPart).Name));

            SchemaBuilder.CreateTable(typeof(SeoPartRecord).Name,
                table => table
                    .ContentPartVersionRecord()
                    .Column<string>("TitleOverride", column => column.WithLength(1024))
                    .Column<string>("DescriptionOverride", column => column.Unlimited())
                    .Column<string>("KeywordsOverride", column => column.Unlimited())
                );

            ContentDefinitionManager.AlterPartDefinition(typeof(SeoPart).Name, builder => builder.Attachable());

            SchemaBuilder.CreateTable(typeof(SeoDynamicPagePartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("Path", column => column.WithLength(1024).Unique())
                ).AlterTable(typeof(SeoDynamicPagePartRecord).Name,
                table => table.CreateIndex("Path", new string[] { "Path" })
            );

            ContentDefinitionManager.AlterTypeDefinition("SeoDynamicPage",
                cfg => cfg
                    .WithPart("CommonPart", p => p
                        .WithSetting("DateEditorSettings.ShowDateEditor", "false")
                        .WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false"))
                    .WithPart(typeof(SeoDynamicPagePart).Name)
                    .WithPart(typeof(SeoPart).Name)
                    .WithSetting("Stereotype", "SeoNonContent")
                    .Draftable());


            return 4;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterTypeDefinition("SeoSettings",
                cfg => cfg
                    .WithPart("CommonPart", p => p
                        .WithSetting("DateEditorSettings.ShowDateEditor", "false")
                        .WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false"))
                    .WithPart(typeof(SeoGlobalSettingsPart).Name));


            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder.AlterTable(typeof(SeoGlobalSettingsPartRecord).Name,
                table => {
                    table.AddColumn<int>("TitleOverrideMaxLength");
                    table.AddColumn<int>("DescriptionOverrideMaxLength");
                    table.AddColumn<int>("KeywordsOverrideMaxLength");
                });


            return 3;
        }

        public int UpdateFrom3() {
            SchemaBuilder.CreateTable(typeof(SeoDynamicPagePartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("Path", column => column.WithLength(1024).Unique())
                ).AlterTable(typeof(SeoDynamicPagePartRecord).Name,
                table =>  table.CreateIndex("Path", new string[] { "Path" })
            );

            ContentDefinitionManager.AlterTypeDefinition("SeoDynamicPage",
                cfg => cfg
                    .WithPart("CommonPart", p => p
                        .WithSetting("DateEditorSettings.ShowDateEditor", "false")
                        .WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false"))
                    .WithPart(typeof(SeoDynamicPagePart).Name)
                    .WithPart(typeof(SeoPart).Name)
                    .WithSetting("Stereotype", "SeoNonContent")
                    .Draftable());


            return 4;
        }
    }
}