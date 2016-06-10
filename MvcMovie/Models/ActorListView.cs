using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcMovie.Models
{
    public class CreateOrEditMovieVM
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }

        public decimal Price { get; set; }
        public string Rating { get; set; }

        public int[] ReceviedActors { get; set; }
        public MultiSelectList ActorList { get; set; }
    }
}