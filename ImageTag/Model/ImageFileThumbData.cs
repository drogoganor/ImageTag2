using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ImageTag.Data;

namespace ImageTag.Model
{
    public class ImageFileThumbData
    {
        public ImageSource ImageSource { get; set; }
        public string Filename { get; set; }
        public string FullPath { get; set; }
        public string Checksum { get; set; }
        public Image Image { get; set; }

        public ImageFileThumbData()
        {
        }
    }
}
