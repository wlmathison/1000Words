using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace _1000Words.Models
{
    public class Photo
    {
        public int Id { get; set; }

        public DateTime? Date { get; set; }

        [Required]
        public string Path { get; set; }

        public bool IsFavorite { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public virtual ICollection<PhotoAlbum> PhotoAlbums { get; set; }

        [NotMapped]
        [Display(Name ="Date Taken")]
        public string FormattedDate
        {
            get
            {
                if (Date != null)
                {
                    return Date.Value.ToShortDateString();
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
