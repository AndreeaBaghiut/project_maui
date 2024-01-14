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

        protected override void OnAppearing()
        {
            base.OnAppearing();


            
        }
        public RecipesByCategoryPage(int categoryId)
        {
            if (categoryId == null)
            {
                throw new ArgumentNullException(nameof(categoryId));
            }

            InitializeComponent();
        }

        public async Task LoadRecipesByCategory(int categoryId)
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

            await LoadRecipesByCategory(selectedCategory.Id);

            ((ListView)sender).SelectedItem = null;
        }


    }
}

