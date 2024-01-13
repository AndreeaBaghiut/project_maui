using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using p3.Models;

namespace p3
{
    public partial class RecipesByCategoryPage : ContentPage
    {
        public RecipesByCategoryPage(int categoryId)
        {
            InitializeComponent();
            LoadRecipesByCategory(categoryId);
        }

        async void LoadRecipesByCategory(int categoryId)
        {
            
                var recipes = await App.Database.GetRecipesByCategoryAsync(categoryId);

                if (recipes != null && recipes.Any())
                {
                    recipesByCategoryListView.ItemsSource = recipes;
                }            
      }
            
        async void OnCategorySelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var selectedCategory = (RecipeCategory)e.SelectedItem;

            // Ensure selectedCategory is not null
            if (selectedCategory != null)
            {
                // Navigate to a page where you display recipes associated with the selected category
                await Navigation.PushAsync(new RecipesByCategoryPage(selectedCategory.Id));
            }

    // Deselect the item
    ((ListView)sender).SelectedItem = null;
        }

    }
}
