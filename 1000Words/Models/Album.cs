using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace _1000Words.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public virtual ICollection<PhotoAlbum> PhotoAlbums { get; set; }

        public virtual ICollection<PhotoDescription> PhotoDescriptions { get; set; }
    }
}
