using Plugin.Media;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Machina
{
    public partial class MainPage : ContentPage
    {
        public ICommand AnimationClickedCommand { get; set; }
        public MainPage()
        {
            AnimationClickedCommand = new Command(() =>
            {
                _ = StartButtonClickAsync();
            });
            BindingContext = this;
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void Start_Button_Clicked(object sender, EventArgs e)
        {
            await StartButtonClickAsync();
        }
        private async Task StartButtonClickAsync()
        {
            var network = Connectivity.NetworkAccess;
            if(network != NetworkAccess.Internet)
            {
                await DisplayAlert("No network", "No internet connection available", "ok");
                return;
            }

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
