using Plugin.Media;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Machina
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void Start_Button_Clicked(object sender, EventArgs e)
        {
            await StartButtonClickAsync();
        }
        private async Task StartButtonClickAsync()
        {
            await CrossMedia.Current.Initialize();
            if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ": No camera available.", "OK");
                return;
            }
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "visage.jpg",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
            });
            if(file == null)
            {
                return;
            }         
            await Navigation.PushAsync(new ScannerPage(file), false);
        }
    }
}
