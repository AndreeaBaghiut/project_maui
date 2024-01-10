using Microsoft.Maui.Controls;
using p3.Models;
using System.IO.Compression;
//using SixLabors.ImageSharp;

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
                        // Handle the case where permission is denied.
                        // For example, you could show a message to the user or disable certain functionality.
                        Console.WriteLine("StorageWrite permission denied");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here.
                // You can log it, show a message to the user, or take appropriate actions.
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


        // Adaugă această metodă pentru a converti imaginea în șir de octeți
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

                // Verificăm dacă s-a setat sursa imaginii direct dintr-un șir de octeți
                if (recipeImage.Source is StreamImageSource)
                {
                    // Convertem sursa imaginii într-un șir de octeți
                    using (var stream = await ((StreamImageSource)recipeImage.Source).Stream.Invoke(new System.Threading.CancellationToken()))
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        recipe.ImageData = memoryStream.ToArray();
                    }
                }
                // Verificăm dacă s-a selectat o imagine cu o cale de fișier
                else if (recipeImage.Source is FileImageSource fileImageSource && fileImageSource.File != null)
                {
                    // Obținem calea imaginii
                    var imagePath = fileImageSource.File;

                    // Citim imaginea într-un șir de octeți
                    recipe.ImageData = File.ReadAllBytes(imagePath);
                }

                // Salvăm rețeta și așteptăm finalizarea salvării
                await App.Database.SaveRecipeAsync(recipe);

                // Obținem rețeta actualizată
                var updatedRecipe = await App.Database.GetRecipeAsync(recipe.Id);

                // Verificăm dacă rețeta returnată este validă
                if (updatedRecipe != null)
                {
                    // Reîmprospătăm BindingContext-ul
                    BindingContext = updatedRecipe;

                    // Navigăm înapoi la pagina anterioară
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
                // Verificăm dacă BindingContext este nul sau nu este o instanță de Recipe
                if (BindingContext == null || !(BindingContext is Recipe reciper))
                {
                    // Dacă BindingContext este null sau nu este o instanță de Recipe, nu avem ce să facem aici
                    return;
                }

                // Verificăm dacă ImageData este nenul și încărcăm din nou imaginea
                if (reciper.ImageData != null)
                {
                    recipeImage.Source = ImageSource.FromStream(() => new MemoryStream(reciper.ImageData));
                }

                // Verificăm dacă avem ID valid pentru rețetă
                if (reciper.Id != 0)
                {
                    // Obținem rețeta actualizată
                    BindingContext = await App.Database.GetRecipeAsync(reciper.Id);

                    // Actualizăm din nou referința la rețetă după ce am obținut-o
                    reciper = (Recipe)BindingContext;

                    // Setăm sursa imaginii din nou, dacă există ImageData
                    if (reciper.ImageData != null)
                    {
                        recipeImage.Source = ImageSource.FromStream(() => new MemoryStream(reciper.ImageData));
                    }

                    // Actualizăm și ListView
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