using System;
using System.Collections.Generic;

namespace ImageTag;

public partial class Tag
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long TagType { get; set; }

    public byte[] LastUsed { get; set; }

    public virtual ICollection<Tag> Children { get; } = new List<Tag>();

    public virtual ICollection<OrganizeDirectory> Directories { get; } = new List<OrganizeDirectory>();

    public virtual ICollection<Image> Images { get; } = new List<Image>();

    public virtual ICollection<Tag> Parents { get; } = new List<Tag>();
}
