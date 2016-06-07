using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    public class Actor
    {
        public int ID { get; set; }

        [Required]
        public string ActorName { get; set; }

        public virtual ICollection<Movie> Movies { get; set; }
    }

    public class Movie
    {
        public Movie()
        {
            this.Actors = new HashSet<Actor>();
        }
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string Genre { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [StringLength(5)]
        public string Rating { get; set; }

        public virtual ICollection<Actor> Actors { get; set; }
    }

    public class MovieDBContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
    }
}