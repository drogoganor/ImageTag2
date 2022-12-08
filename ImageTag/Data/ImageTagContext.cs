using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ImageTag;

public partial class ImagetagContext : DbContext
{
    public ImagetagContext()
    {
    }

    public ImagetagContext(DbContextOptions<ImagetagContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<OrganizeDirectory> OrganizeDirectories { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=T:\\imagetag.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("Image");

            entity.HasIndex(e => e.Id, "IX_Image_ID").IsUnique();

            entity.HasIndex(e => e.Path, "IX_Image_Path").IsUnique();

            entity.HasIndex(e => e.Checksum, "ImageChecksum_IDX");

            entity.HasIndex(e => e.Id, "ImageID_IDX").IsUnique();

            entity.HasIndex(e => e.Path, "ImagePath_IDX").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Checksum).IsRequired();
            entity.Property(e => e.Path).IsRequired();

            entity.HasMany(d => d.Tags).WithMany(p => p.Images)
                .UsingEntity<Dictionary<string, object>>(
                    "ImageTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Image>().WithMany()
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("ImageId", "TagId");
                        j.ToTable("ImageTag");
                        j.HasIndex(new[] { "ImageId", "TagId" }, "ImageTag_IDX").IsUnique();
                    });
        });

        modelBuilder.Entity<OrganizeDirectory>(entity =>
        {
            entity.ToTable("OrganizeDirectory");

            entity.HasIndex(e => e.Id, "IX_OrganizeDirectory_ID").IsUnique();

            entity.HasIndex(e => e.Id, "OrganizeDirectory_IDX").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).IsRequired();

            entity.HasMany(d => d.ChildDirectories).WithMany(p => p.ParentDirectories)
                .UsingEntity<Dictionary<string, object>>(
                    "OrganizeDirectoryDirectory",
                    r => r.HasOne<OrganizeDirectory>().WithMany()
                        .HasForeignKey("ChildDirectoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<OrganizeDirectory>().WithMany()
                        .HasForeignKey("ParentDirectoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("ParentDirectoryId", "ChildDirectoryId");
                        j.ToTable("OrganizeDirectoryDirectory");
                        j.HasIndex(new[] { "ChildDirectoryId" }, "OrgDirDir_Child_IDX");
                        j.HasIndex(new[] { "ParentDirectoryId" }, "OrgDirDir_Parent_IDX");
                        j.HasIndex(new[] { "ParentDirectoryId", "ChildDirectoryId" }, "OrganizeDirectoryDirectory_IDX").IsUnique();
                    });

            entity.HasMany(d => d.ParentDirectories).WithMany(p => p.ChildDirectories)
                .UsingEntity<Dictionary<string, object>>(
                    "OrganizeDirectoryDirectory",
                    r => r.HasOne<OrganizeDirectory>().WithMany()
                        .HasForeignKey("ParentDirectoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<OrganizeDirectory>().WithMany()
                        .HasForeignKey("ChildDirectoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("ParentDirectoryId", "ChildDirectoryId");
                        j.ToTable("OrganizeDirectoryDirectory");
                        j.HasIndex(new[] { "ChildDirectoryId" }, "OrgDirDir_Child_IDX");
                        j.HasIndex(new[] { "ParentDirectoryId" }, "OrgDirDir_Parent_IDX");
                        j.HasIndex(new[] { "ParentDirectoryId", "ChildDirectoryId" }, "OrganizeDirectoryDirectory_IDX").IsUnique();
                    });

            entity.HasMany(d => d.Tags).WithMany(p => p.Directories)
                .UsingEntity<Dictionary<string, object>>(
                    "OrganizeDirectoryTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<OrganizeDirectory>().WithMany()
                        .HasForeignKey("DirectoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("DirectoryId", "TagId");
                        j.HasIndex(new[] { "DirectoryId" }, "OrgDirectoryTags_Dir_IDX");
                        j.HasIndex(new[] { "TagId" }, "OrgDirectoryTags_Tag_IDX");
                        j.HasIndex(new[] { "DirectoryId", "TagId" }, "OrganizeDirectoryTags_IDX").IsUnique();
                    });
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");

            entity.HasIndex(e => e.Id, "IX_Tag_ID").IsUnique();

            entity.HasIndex(e => e.Name, "IX_Tag_Name").IsUnique();

            entity.HasIndex(e => e.Id, "TagID_IDX").IsUnique();

            entity.HasIndex(e => e.LastUsed, "TagLastUsed_IDX");

            entity.HasIndex(e => e.Name, "TagName_IDX").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LastUsed).HasColumnType("DATETIME");
            entity.Property(e => e.Name).IsRequired();

            entity.HasMany(d => d.Children).WithMany(p => p.Parents)
                .UsingEntity<Dictionary<string, object>>(
                    "TagParent",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Tag>().WithMany()
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("ParentId", "ChildId");
                        j.ToTable("TagParent");
                        j.HasIndex(new[] { "ParentId", "ChildId" }, "TagParent_IDX").IsUnique();
                    });

            entity.HasMany(d => d.Parents).WithMany(p => p.Children)
                .UsingEntity<Dictionary<string, object>>(
                    "TagParent",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Tag>().WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("ParentId", "ChildId");
                        j.ToTable("TagParent");
                        j.HasIndex(new[] { "ParentId", "ChildId" }, "TagParent_IDX").IsUnique();
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
