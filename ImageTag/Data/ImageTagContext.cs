using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ImageTag.Data
{
    public class ImageTagContext : DbContext
    {
        public DbSet<Image> Images { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<OrganizeDirectory> OrganizeDirectories { get; set; }

        public string DbPath { get; }

        public ImageTagContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "imagetag.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}