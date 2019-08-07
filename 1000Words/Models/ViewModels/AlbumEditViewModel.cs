using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _1000Words.Models.ViewModels
{
    public class AlbumEditViewModel
    {
        public Album Album { get; set; }
        public List<PhotoAlbum> PhotoAlbums { get; set; }
    }
}
