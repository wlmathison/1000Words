using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _1000Words.Models.ViewModels
{
    public class AlbumDetailsViewModel
    {
        public Album Album { get; set; }
        public List<Photo> Photos { get; set; }
    }
}
