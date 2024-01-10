using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;


namespace p3.Models
{
    public class RecipeIngredient
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(Recipe))]
        public int? RecipeId { get; set; }
        [ForeignKey(typeof(Ingredient))]
        public int? IngredientId { get; set; }
    }
}
