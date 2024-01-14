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
                    database = new
RecipeDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
LocalApplicationData), "RecipeDatabase.db3"));

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
