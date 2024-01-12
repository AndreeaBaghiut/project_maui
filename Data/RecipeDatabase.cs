using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using p3.Models;
using SQLiteNetExtensions.Attributes;

namespace p3.Data
{
    public class RecipeDatabase
    {
        readonly SQLiteAsyncConnection _database;


        public RecipeDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Recipe>().Wait();
            _database.CreateTableAsync<Ingredient>().Wait();
            _database.CreateTableAsync<RecipeIngredient>().Wait();
            _database.CreateTableAsync<RecipeCategory>().Wait();
        }


        // public async Task InitializeAsync()
        //{
        //  await _database.CreateTableAsync<Recipe>().ConfigureAwait(false);
        //await _database.CreateTableAsync<Ingredient>().ConfigureAwait(false);
        //await _database.CreateTableAsync<RecipeIngredient>().ConfigureAwait(false);
        //}

        public Task<List<Recipe>> GetRecipeAsync()
        {
            return _database.Table<Recipe>().ToListAsync();
        }

        public Task<Recipe> GetRecipeAsync(int id)
        {
            return _database.Table<Recipe>()
            .Where(i => i.Id == id)
           .FirstOrDefaultAsync();
        }

        public Task<int> SaveRecipeAsync(Recipe rec)
        {
            if (rec.Id != 0)
            {
                return _database.UpdateAsync(rec);
            }
            else
            {
                return _database.InsertAsync(rec);
            }
        }

        public Task<int> DeleteRecipeAsync(Recipe rec)
        {
            return _database.DeleteAsync(rec);
        }

        public Task<int> SaveIngredientAsync(Ingredient ingredient)
        {
            if (ingredient.Id != 0)
            {
                return _database.UpdateAsync(ingredient);
            }
            else
            {
                return _database.InsertAsync(ingredient);
            }
        }
        public Task<int> DeleteIngredientAsync(Ingredient ingredient)
        {
            return _database.DeleteAsync(ingredient);
        }
        public Task<List<Ingredient>> GetIngredientsAsync()
        {
            return _database.Table<Ingredient>().ToListAsync();
        }

        public Task<int> SaveRecipeIngredientAsync(RecipeIngredient recin)
        {
            if (recin.Id != 0)
            {
                return _database.UpdateAsync(recin);
            }
            else
            {
                return _database.InsertAsync(recin);
            }
        }

        public Task<List<Ingredient>> GetRecipeIngredientsAsync(int recipeId)
        {
            return _database.QueryAsync<Ingredient>(
                "SELECT I.Id, I.Description FROM Ingredient I" +
                " INNER JOIN RecipeIngredient RI"
                + " on I.Id = RI.IngredientId where RI.RecipeId = ?",
                recipeId);
        }

        public Task<List<RecipeCategory>> GetCategoriesAsync()
        {
            return _database.Table<RecipeCategory>().ToListAsync();
        }

        public Task<int> SaveCategoryAsync(RecipeCategory category)
        {
            if (category.Id != 0)
            {
                return _database.UpdateAsync(category);
            }
            else
            {
                return _database.InsertAsync(category);
            }
        }

        public Task<List<Recipe>> GetRecipesByCategoryAsync(int categoryId)
        {
            return _database.Table<Recipe>().
                Where(r => r.Category != null && r.Category.Id == categoryId).
                ToListAsync();
        }

        public Task<RecipeCategory> GetCategoryByNameAsync(string categoryName)
        {
            return _database.Table<RecipeCategory>()
                .Where(c => c.CategoryName == categoryName)
                .FirstOrDefaultAsync();
        }


    }
}


