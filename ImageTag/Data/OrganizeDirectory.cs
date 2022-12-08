using System;
using System.Collections.Generic;

namespace ImageTag;

public partial class OrganizeDirectory
{
    public long Id { get; set; }

    public string Name { get; set; }

    public long? Rating { get; set; }

    public string Description { get; set; }

    public string ForeColor { get; set; }

    public string BackColor { get; set; }

    public long IgnoreParent { get; set; }

    public long OrTags { get; set; }

    public long CopyOnly { get; set; }

    public long TheseTagsOnly { get; set; }

    public virtual ICollection<OrganizeDirectory> ChildDirectories { get; } = new List<OrganizeDirectory>();

    public virtual ICollection<OrganizeDirectory> ParentDirectories { get; } = new List<OrganizeDirectory>();

    public virtual ICollection<Tag> Tags { get; } = new List<Tag>();
}
