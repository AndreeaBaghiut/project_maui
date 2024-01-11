using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteNetExtensions.Attributes;


namespace p3.Models
{
    class Image
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ShopName { get; set; }
        public string Adress { get; set; }
        public string ShopDetails
        {
            get
            {
                return ShopName + "+Adress;} }
        [OneToMany]
       public List<ShopList> ShopLists { get; set; }
    }
}
