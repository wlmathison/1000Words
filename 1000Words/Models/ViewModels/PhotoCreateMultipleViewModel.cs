using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _1000Words.Models.ViewModels
{
    public class PhotoCreateMultipleViewModel
    {
        public DateTime? Date { get; set; }

        public IFormFile Photo { get; set; }

        public List<IFormFile> Photos { get; set; }
    }
}