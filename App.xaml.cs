using System;
using p3.Data;
using System.IO;

namespace p3
{
    public partial class App : Application
    {
        static RecipeDatabase database;

        public static RecipeDatabase Database
        {
            get
            {
                if (database == null)
                {
                    // string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Recipe.db3");
                    database = new
RecipeDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
LocalApplicationData), "Recipe.db3"));

                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

       // private static async void InitializeDatabaseAsync()
        //{
          //  if (database != null)
            //{
              //  await database.InitializeAsync();
            //}
        //}
    }
}
