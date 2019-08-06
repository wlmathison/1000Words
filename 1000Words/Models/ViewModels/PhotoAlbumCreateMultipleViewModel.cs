using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _1000Words.Models.ViewModels
{
    public class PhotoAlbumCreateMultipleViewModel
    {
        public List<Photo> Photos { get; set; }
        public PhotoAlbum PhotoAlbum { get; set; }
    }
}
