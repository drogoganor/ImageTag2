using System;
using System.Collections.Generic;

namespace ImageTag.Data
{
    public class Tag
    {
        public Tag()
        {
            this.Images = new HashSet<Image>();
            this.ParentTags = new HashSet<Tag>();
            this.ChildTags = new HashSet<Tag>();
            this.OrganizeDirectories = new HashSet<OrganizeDirectory>();
        }
    
        public long ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long TagType { get; set; }
        public DateTime? LastUsed { get; set; }

        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Tag> ParentTags { get; set; }
        public virtual ICollection<Tag> ChildTags { get; set; }
        public virtual ICollection<OrganizeDirectory> OrganizeDirectories { get; set; }
    }
}
