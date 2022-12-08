using System;
using System.Collections.Generic;

namespace ImageTag.Data
{
    public class OrganizeDirectory
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public long IgnoreParent { get; set; }
        public long OrTags { get; set; }
        public long CopyOnly { get; set; }
        public Nullable<long> Rating { get; set; }
        public string Description { get; set; }
        public string ForeColor { get; set; }
        public string BackColor { get; set; }
        public long TheseTagsOnly { get; set; }

        public virtual List<OrganizeDirectory> ParentDirectories { get; set; } = new();
        public virtual List<OrganizeDirectory> ChildDirectories { get; set; } = new();
        public virtual List<Tag> Tags { get; set; } = new();
    }
}
