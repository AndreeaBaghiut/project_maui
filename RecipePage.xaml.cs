using p3.Models;
using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;

namespace p3
{
    public partial class RecipePage : ContentPage
    {
        public List<RecipeCategory>? _categories;

        public RecipePage()
        {
            InitializeComponent();
            CheckAndRequestStoragePermission();
        }

        async void CheckAndRequestStoragePermission()
        {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

                if (status != PermissionStatus.Granted)
                {
                    var result = await Permissions.RequestAsync<Permissions.StorageWrite>();

                    if (result != PermissionStatus.Granted)
                    {
                        Console.WriteLine("StorageWrite permission denied");
                    }
                }
            }

        async void OnSelectImageButtonClicked(object sender, EventArgs e)
        {
                var recipe = (Recipe)BindingContext;

                if (recipe != null)
                {
                    var mediaOptions = new MediaPickerOptions
                    {
                        Title = "Select Image"
                    };

                    var selectedImage = await MediaPicker.PickPhotoAsync(mediaOptions);

                    if (selectedImage != null)
                    {
                        recipe.ImageData = await ImageToBytes(selectedImage);
                        recipeImage.Source = ImageSource.FromStream(() => selectedImage.OpenReadAsync().Result);
                    }
                }
                else
                {
                    return;
                }          
        }

        async Task<byte[]> ImageToBytes(FileResult image)
        {
            using (var stream = await image.OpenReadAsync())
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var recipe = (Recipe)BindingContext;

            if (recipe == null)
            {
                return;
            }

            recipe.Category = (RecipeCategory)categoryPicker.SelectedItem;

            if (recipe.ImageData == null)
            {
                return;
            }

            if (recipeImage.Source is StreamImageSource streamImageSource)
            {
                using (var stream = await streamImageSource.Stream.Invoke(new System.Threading.CancellationToken()))
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    recipe.ImageData = memoryStream.ToArray();
                }
            }
            else if (recipeImage.Source is FileImageSource fileImageSource && fileImageSource.File != null)
            {
                recipe.ImageData = File.ReadAllBytes(fileImageSource.File);
            }

            await App.Database.SaveRecipeAsync(recipe);

            var updatedRecipe = await App.Database.GetRecipeAsync(recipe.Id);

            if (updatedRecipe != null)
            {
                BindingContext = updatedRecipe;
                await Navigation.PopAsync();
            }
            else
            {
                return;
            }
        }



        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var recipe = (Recipe)BindingContext;
            await App.Database.DeleteRecipeAsync(recipe);
            await Navigation.PopAsync();
        }

        async void OnChooseButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new IngredientPage((Recipe)this.BindingContext)
            {
                BindingContext = new Ingredient()
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var recipe = (Recipe)BindingContext;

            if (recipe == null)
            {
                Console.WriteLine("Recipe is null");
                return;
            }

            recipeView.ItemsSource = await App.Database.GetRecipeIngredientsAsync(recipe.Id);
            _categories = await App.Database.GetCategoriesAsync();
            categoryPicker.ItemsSource = _categories;

            if (recipe.Id != 0)
            {
                Task<byte[]> getDataTask = Task.Run(() => recipe.ImageData);

                await getDataTask;

                if (recipe.ImageData != null)
                {
                    recipeImage.Source = ImageSource.FromStream(() => new MemoryStream(recipe.ImageData));
                }

                recipeView.ItemsSource = await App.Database.GetRecipeIngredientsAsync(recipe.Id);
            }

        }


    }
}