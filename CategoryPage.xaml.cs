﻿using System;
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
                // Verificați dacă categoriile predefinite există deja în baza de date
                var existingCategories = await App.Database.GetCategoriesAsync();

                if (existingCategories.Count == 0)
                {
                    // Adăugați categoriile predefinite doar dacă nu există deja în baza de date
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
                            // Categoria nu există deja, deci o salvăm
                            await App.Database.SaveCategoryAsync(category);
                        }
                    }
                }
            }
           
        

        async void LoadCategories()
        {
           
                var categories = await App.Database.GetCategoriesAsync();
                categoryListView.ItemsSource = categories;
            
           
        }

        async void OnCategorySelected(object sender, EventArgs e)
        {
            if (e is SelectedItemChangedEventArgs args && args.SelectedItem != null)
            {
                var selectedCategory = (RecipeCategory)args.SelectedItem;

                // Navigate to the RecipesByCategoryPage for the selected category
                await Navigation.PushAsync(new RecipesByCategoryPage(selectedCategory.Id));
            }
        }


    }
}
