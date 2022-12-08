using System.Collections.Generic;

namespace ImageTag.Data
{
    public class Image
    {
        public long ID { get; set; }
        public string Path { get; set; }
        public string Checksum { get; set; }
        public long? Rating { get; set; }
        public long? Explicit { get; set; }

        public List<Tag> Tags { get; set; } = new();
    }
}
