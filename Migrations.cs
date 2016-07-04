using Onestop.Seo.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Onestop.Seo {
    public class Migrations : DataMigrationImpl {
        public int Create() {
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
                    .Column<string>("CanonicalUrlOverride", column => column.WithLength(1024))
                    .Column<string>("HTMLCardOverride", column => column.WithLength(2048))
                );

            ContentDefinitionManager.AlterPartDefinition(typeof(SeoPart).Name,
                builder => builder
                    .Attachable()
                    .WithDescription("Provides settings for search engine optimization.")
                );

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


            return 6;
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
            // Deprecated code removed due to infoset migration.


            return 3;
        }

        public int UpdateFrom3() {
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

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(SeoPartRecord).Name,
                table =>
                {
                    table.AddColumn<string>("CanonicalUrlOverride", column => column.WithLength(1024));
                    table.AddColumn<string>("HTMLCardOverride", column => column.WithLength(2048));
                });

            return 5;
        }

        public int UpdateFrom5()
        {
            ContentDefinitionManager.AlterPartDefinition(typeof(SeoPart).Name,
                builder => builder
                    .WithDescription("Provides settings for search engine optimization.")
                );

            return 6;
        }

    }
}