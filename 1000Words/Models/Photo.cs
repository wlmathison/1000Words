using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
