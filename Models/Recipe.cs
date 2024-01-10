using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteNetExtensions.Attributes;

namespace p3.Models
{
    public class Recipe
    {
       [PrimaryKey, AutoIncrement]
        public int Id  { get; set; }
        public string? Title { get; set; }
        public string? Ingredients { get; set; }
        public string? Description { get; set; }
        public byte[]? ImageData { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<RecipeIngredient>? RecipeIngredients { get; set; }
    }
}
