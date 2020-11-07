using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Net.Http;
using Acr.UserDialogs;
using Newtonsoft.Json;

namespace Recuerdame
{
    public partial class MainPage : ContentPage
    {
        private MediaFile _photo;

        public MainPage()
        {
            InitializeComponent();
            //MainPage = new NavigationPage(new ChatBot());
        }

        int count = 0;
        private void Handle_Clicked(object sender, EventArgs e)
        {
            count++;
            ((Button)sender).Text = $"You clicked {count} times.";
        }
        private async void Elegir_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            var photo = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions());

            if (photo == null)
            {
                return;
            }
            else
            {
                _photo = photo;
                ImgSource.Source = FileImageSource.FromFile(photo.Path);
            }
        }

        private async void Tomar_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camara", "No hay camara disponible", "Aceptar");
                return;
            }

            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                SaveToAlbum = true,
                Directory = "Recuerdame",
                Name = "source.jpg",
                DefaultCamera = CameraDevice.Front,
                PhotoSize = PhotoSize.Medium
            });
            _photo = photo;
            ImgSource.Source = FileImageSource.FromFile(photo.Path);



            //var imageSource = ImageSource.FromStream(() =>
            //{
            //    var stream = photo.GetStream();
            //    ImgSource.Source = FileImageSource.FromStream(stream);
            //});


            if (photo == null)
            {
                return;
            }
        }

        private async void Analizar_Clicked(object sender, EventArgs e)
        {
            //const string endpoint = ""; //link

            //var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Add("Prediction-Key", ""); //Prediction key of "If you have an image file" 

            //var contentStream = new StreamContent(_photo.GetStream());

            //using (Acr.UserDialogs.UserDialogs.Instance.Loading("Procesando..."))
            //{
            //    var response = await httpClient.PostAsync(endpoint, contentStream);

            //    if (!response.IsSuccessStatusCode)
            //    {
            //        UserDialogs.Instance.Toast("Un error ha ocurrido");
            //    }

            //    var json = await response.Content.ReadAsStringAsync();

            //    var prediction = JsonConvert.DeserializeObject<PredictionsResponse>(json);
            //    var tag = prediction.Predictions.First();

            //    Resultado.Text = $"{tag.Tag} - {tag.Probability:p0}";
            //    Precision.Progress = tag.Probability;
            //}
        }

        private void Chatbot_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new ChatBot());
        }
    }
}
