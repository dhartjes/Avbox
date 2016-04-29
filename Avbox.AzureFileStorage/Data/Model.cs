using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System;
using System.IO;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace AzureFileStorage.Data
{
    public class AzureFileInfoContext : DbContext
    {
        public DbSet<AzureFileInfo> AzureFileInfos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //base.OnModelCreating(modelBuilder);
        }
    }

    public class AzureFileInfo
    {
        public int Id { get; set; }
        public string AzureUri { get; set; }
        public string AzureDirectory { get; set; }
        public string Link { get; set; }
        public string Recipient { get; set; }
        public DateTime UploadDate { get; set; }
        public long FileSize { get; set; }
        public string FileName { get; set; }

        internal void GenerateLink()
        {
            this.Link = Path.Combine("Download", "Details", this.AzureDirectory, this.Id.ToString()).Replace('\\', '/');
        }
    }
}
