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

                if (recipe.ImageData != null)
                {
                    await App.Database.SaveRecipeAsync(recipe);
                }
                else
                {
                    Console.WriteLine("ImageData is null");
                }

                if (recipeImage.Source is StreamImageSource)
                {
                    using (var stream = await ((StreamImageSource)recipeImage.Source).Stream.Invoke(new System.Threading.CancellationToken()))
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        recipe.ImageData = memoryStream.ToArray();
                    }
                }
                else if (recipeImage.Source is FileImageSource fileImageSource && fileImageSource.File != null)
                {
                    var imagePath = fileImageSource.File;
                    recipe.ImageData = File.ReadAllBytes(imagePath);
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
            await Navigation.PushAsync(new IngredientPage((Recipe)BindingContext, this)
            {
                BindingContext = new Ingredient()
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (BindingContext == null || !(BindingContext is Recipe reciper))
                {
                    return;
                }

                if (reciper.ImageData != null)
                {
                    recipeImage.Source = ImageSource.FromStream(() => new MemoryStream(reciper.ImageData));
                }

                if (reciper.Id != 0)
                {
                    BindingContext = await App.Database.GetRecipeAsync(reciper.Id);
                    reciper = (Recipe)BindingContext;

                    if (reciper.ImageData != null)
                    {
                        recipeImage.Source = ImageSource.FromStream(() => new MemoryStream(reciper.ImageData));
                    }

                    recipeView.ItemsSource = await App.Database.GetRecipeIngredientsAsync(reciper.Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in OnAppearing: {ex.Message}");
            }
        }
    }
}
