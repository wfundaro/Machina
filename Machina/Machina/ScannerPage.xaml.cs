using Machina.Model;
using Machina.Service;
using Plugin.Media.Abstractions;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Machina
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannerPage : ContentPage
    {
        private bool detectionOnProcess = true;
        private FaceDetectModels faceResult = null;
        private readonly ISimpleAudioPlayer soundPlayer = CrossSimpleAudioPlayer.Current;
        SpeechOptions speechSettings = null;
        public ScannerPage(MediaFile file)
        {
            InitializeComponent();
            infoLayout.IsVisible = false;

            NavigationPage.SetHasNavigationBar(this, false);
            var stream = file.GetStreamWithImageRotatedForExternalStorage();
            //Copy stream for prevent ios problem
            MemoryStream imageStream = new MemoryStream();
            stream.CopyTo(imageStream);
            stream.Position = 0;
            imageStream.Position = 0;
            faceImage.Source = ImageSource.FromStream(() => imageStream );

            LaserAnimationWithSoundAndDisplayResults();
            StartDetection(stream);
        }

        //Laser animation
        private async Task LaserAnimationWithSoundAndDisplayResults()
        {
            laserImage.Opacity = 0;
            await Task.Delay(500);
            await laserImage.FadeTo(1, 500);
            PlaySound("scan.wav");
            await laserImage.TranslateTo(0, 360, 1800);
            double y = 0;
            while (detectionOnProcess)
            {
                PlayCurrentSound();
                await laserImage.TranslateTo(0, y, 1800);
                y = (y == 0) ? 360 : 0;
            }
            laserImage.IsVisible = false;
            PlaySound("result.wav");
            await DisplayResult();
            await ResultsSpeech();
        }

        private void ContinueButtonClicked(object sender, EventArgs args)
        {
            Navigation.PopAsync();
        }

        private async Task DisplayResult()
        {
            statusLabel.Text = "Analyse terminée";

            // On a récupéré les infos du visage
            ageLabel.Text = faceResult.faceAttributes.age.ToString();
            genderLabel.Text = faceResult.faceAttributes.gender.Substring(0, 1).ToUpper();
            infoLayout.IsVisible = true;
            continueButton.Opacity = 1;
        }
        private async Task StartDetection(Stream stream)
        {
            faceResult = await CognitiveService.FaceDetect(stream);
            detectionOnProcess = false;
        }
        private void PlaySound(string soundName)
        {
            soundPlayer.Load(GetStreamFromFile(soundName));
            soundPlayer.Play();
        }

        private void PlayCurrentSound()
        {
            soundPlayer.Stop();
            soundPlayer.Play();
        }
        private Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("Machina." + filename);
            return stream;
        }
        private async Task Speak(string text)
        {
            if (speechSettings == null)
            {
                await InitSpeak();
            }
            await TextToSpeech.SpeakAsync(text, speechSettings);
        }
        private async Task ResultsSpeech()
        {
            if (faceResult == null)
            {
                await Speak("Humain non détecté");
            }
            else
            {
                await Speak("Humain détecté");
                await Speak("Sexe");
                string gender = faceResult.faceAttributes.gender.ToLower().Equals("male") ? "Sexe masculin" : "Sexe féminin";
                await Speak(gender);
                await Speak("âge" + faceResult.faceAttributes.age.ToString() + " ans");
            }
        }

        private async Task InitSpeak()
        {
            var locales = await TextToSpeech.GetLocalesAsync();
            var locale = locales.Where(o => o.Language.ToLower().Equals("fr")).FirstOrDefault();
            speechSettings = new SpeechOptions()
            {
                Volume = .75f,
                Pitch = .2f,
                Locale = locale
            };
        }
    }
}