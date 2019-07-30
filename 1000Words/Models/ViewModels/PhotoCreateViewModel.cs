using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace _1000Words.Models.ViewModels
{
    public class PhotoCreateViewModel
    {
        public DateTime? Date { get; set; }

        [Required]
        public IFormFile Photo { get; set; }
        public List<IFormFile> Photos { get; set; }
    }
}
