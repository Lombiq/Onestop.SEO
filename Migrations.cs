﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Onestop.Seo.Models;

namespace Onestop.Seo {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable(typeof(SeoGlobalSettingsPartRecord).Name,
                table => table
                    .ContentPartVersionRecord()
                    .Column<string>("HomeTitle", column => column.WithLength(1024))
                    .Column<string>("HomeDescription", column => column.Unlimited())
                    .Column<string>("HomeKeywords", column => column.Unlimited())
                    .Column<string>("ContentCategoriesTitlePattern", column => column.WithLength(1024))
                    .Column<string>("ContentPagesTitlePattern", column => column.WithLength(1024))
                    .Column<bool>("EnableCanonicalUrls")
                    .Column<bool>("UseCategoriesForKeywords")
                    .Column<bool>("AutogenerateDescriptions")
                );

            return 1;
        }
    }
}