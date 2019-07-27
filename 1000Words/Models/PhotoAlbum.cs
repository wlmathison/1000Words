using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace _1000Words.Models
{
    public class PhotoAlbum
    {
        public int Id { get; set; }

        [Required]
        public int AlbumId { get; set; }

        public Album Album { get; set; }

        [Required]
        public int PhotoId { get; set; }

        public Photo Photo { get; set; }
    }
}
