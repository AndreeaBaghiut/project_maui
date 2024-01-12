using p3.Models;
using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;

namespace p3
{
    public partial class RecipePage : ContentPage
    {
        public RecipePage()
        {
            InitializeComponent();
            CheckAndRequestStoragePermission();
        }

        async void CheckAndRequestStoragePermission()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        async void OnSelectImageButtonClicked(object sender, EventArgs e)
        {
            try
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
                    Console.WriteLine("BindingContext is null in OnSelectImageButtonClicked");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in OnSelectImageButtonClicked: {ex.Message}");
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
            try
            {
                var recipe = (Recipe)BindingContext;

                if (recipe == null)
                {
                    Console.WriteLine("Recipe is null");
                    return;
                }

                // Ensure category is set based on the selected item in the picker
                recipe.Category = (RecipeCategory)categoryPicker.SelectedItem;


                if (recipe.ImageData == null)
                {
                    Console.WriteLine("ImageData is null");
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
                    Console.WriteLine("GetRecipeAsync returned null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save Button Exception: {ex.Message}");
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
                return;
            }

            try
            {
                var recipes = await App.Database.GetRecipeAsync();

                if (recipes != null)
                {
                    recipeView.ItemsSource = recipes;
                }
                else
                {
                    recipe.ImageData = new byte[0]; // Set default image data in case it's not set yet
                    recipeImage.Source = ImageSource.FromResource("p3.Assets.NoImageAvailable.png"); // Display default image

                    if (recipe.Id != 0)
                    {
                        // Fetch the recipe's image data asynchronously
                        Task<byte[]> getDataTask = Task.Run(() => recipe.ImageData);

                        // Wait for the image data to be retrieved
                        await getDataTask;

                        // Update the recipeImage's source if the image data is valid
                        if (recipe.ImageData != null)
                        {
                            recipeImage.Source = ImageSource.FromStream(() => new MemoryStream(recipe.ImageData));
                        }

                        // Retrieve the recipe ingredients and set them as the source of the recipeView list
                        recipeView.ItemsSource = await App.Database.GetRecipeIngredientsAsync(recipe.Id);
                        categoryPicker.SelectedItem = recipe.Category;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in OnAppearing: {ex.Message}");
            }
        }


    }
}
