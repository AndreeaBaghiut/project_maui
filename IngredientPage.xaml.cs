using p3.Models;

namespace p3
{
    public partial class IngredientPage : ContentPage
    {
        Recipe r;
        //RecipePage recipePage;

        public IngredientPage(Recipe rec)
        {
            InitializeComponent();
            r = rec;
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var ingredient = (Ingredient)BindingContext;
            await App.Database.SaveIngredientAsync(ingredient);
            recipeView.ItemsSource = await App.Database.GetIngredientsAsync();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var ingredient = (Ingredient)BindingContext;
            await App.Database.DeleteIngredientAsync(ingredient);
            recipeView.ItemsSource = await App.Database.GetIngredientsAsync();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            recipeView.ItemsSource = await App.Database.GetIngredientsAsync();
        }

        async void OnAddButtonClicked(object sender, EventArgs e)
        {
            Ingredient i;

            if (recipeView.SelectedItem != null)
            {
                i = recipeView.SelectedItem as Ingredient;

                var rp = new RecipeIngredient()
                {
                    RecipeId = r.Id,
                    IngredientId = i.Id
                };

                await App.Database.SaveRecipeIngredientAsync(rp);
                i.RecipeIngredients = new List<RecipeIngredient> { rp };

                await Navigation.PopAsync();
            }
        }

        //{
        //  if (recipeView.SelectedItem is Ingredient selectedIngredient)
        //{
        //  var existingRecipeIngredient = r.RecipeIngredients.Find(ri => ri.IngredientId == selectedIngredient.Id);

        //if (existingRecipeIngredient == null)
        //{
        //  r.RecipeIngredients.Add(new RecipeIngredient
        //{
        //  RecipeId = r.Id,
        //IngredientId = selectedIngredient.Id
        //});

        //await App.Database.SaveRecipeAsync(r);
        //}

        //await Navigation.PopModalAsync();
        //}
        //}
    }
}
