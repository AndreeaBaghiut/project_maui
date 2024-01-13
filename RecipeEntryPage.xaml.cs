using Microsoft.Maui.Controls;
using p3.Models;

namespace p3
{
    public partial class RecipeEntryPage : ContentPage
    {
        public RecipeEntryPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var recipes = await App.Database.GetRecipeAsync();

            if (recipeView != null)
            {
                recipeView.ItemsSource = recipes;
            }

        }


        async void OnRecipeAddedClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecipePage
            {
                BindingContext = new Recipe()
            });
        }

        async void OnRecipeViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new RecipePage
                {
                    BindingContext = e.SelectedItem as Recipe
                });
            }
        }
    }
}