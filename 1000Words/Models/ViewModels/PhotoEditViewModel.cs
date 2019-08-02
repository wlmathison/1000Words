using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace _1000Words.Models.ViewModels
{
    public class PhotoEditViewModel
    {
        [Required]
        public Photo Photo { get; set; }
        public List<Description> Descriptions { get; set; }
        public List<PhotoDescription> PhotoDescriptions { get; set; }
        public List<string> CheckedKeywords { get; set; }
    }
}
