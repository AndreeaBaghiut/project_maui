using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace p3.Models
{
    public class RecipeCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? CategoryName { get; set; }
    }
}

