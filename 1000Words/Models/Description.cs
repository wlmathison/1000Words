using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace _1000Words.Models
{
    public class Description
    {
        public int Id { get; set; }

        [Required]
        public string Keyword { get; set; }

        public virtual ICollection<PhotoDescription> PhotoDescriptions { get; set; }

    }
}
