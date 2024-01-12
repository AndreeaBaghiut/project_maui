using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using p3.Models;

namespace p3
{
    public partial class CategoryPage : ContentPage
    {
        public CategoryPage()
        {
            InitializeComponent();
             AddPredefinedCategories();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCategories();
        }

        async Task AddPredefinedCategories()
        {
            try
            {
                // Add predefined categories only if they don't exist
                var predefinedCategories = new List<RecipeCategory>
        {
            new RecipeCategory { CategoryName = "Desert" },
            new RecipeCategory { CategoryName = "Mic-dejun" },
            new RecipeCategory { CategoryName = "Prânz" },
            new RecipeCategory { CategoryName = "Cină" },
            new RecipeCategory { CategoryName = "Festiv" }
        };

                foreach (var category in predefinedCategories)
                {
                    var existingCategory = await App.Database.GetCategoryByNameAsync(category.CategoryName);

                    if (existingCategory == null)
                    {
                        // Category doesn't exist, so save it
                        await App.Database.SaveCategoryAsync(category);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AddPredefinedCategories: {ex.Message}");
            }
        }



        async void LoadCategories()
        {
            try
            {
                var categories = await App.Database.GetCategoriesAsync();
                categoryListView.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in LoadCategories: {ex.Message}");
            }
        }

        async void OnCategorySelected(object sender, EventArgs e)
        {
            if (e is SelectedItemChangedEventArgs args)
            {
                var selectedCategory = (RecipeCategory)args.SelectedItem;

                // Navigate to the RecipesByCategoryPage for the selected category
                await Navigation.PushAsync(new RecipesByCategoryPage(selectedCategory.Id));
            }
        }

    }
}
