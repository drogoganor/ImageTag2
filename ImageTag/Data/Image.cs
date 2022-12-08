using System;
using System.Collections.Generic;

namespace ImageTag;

public partial class Image
{
    public long Id { get; set; }

    public string Path { get; set; }

    public string Checksum { get; set; }

    public long? Rating { get; set; }

    public long? Explicit { get; set; }

    public virtual ICollection<Tag> Tags { get; } = new List<Tag>();
}
