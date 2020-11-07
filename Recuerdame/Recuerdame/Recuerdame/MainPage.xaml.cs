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
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;

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
        private void Chatbot_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new ChatBot());
        }

        private async void Analizar_Clicked(object sender, EventArgs e)
        {
            const string endpoint = "https://servsearch.cognitiveservices.azure.com/customvision/v3.0/Prediction/d6057e20-e203-4a2c-852d-bbbaa2fdc00f/detect/iterations/Iteration12/image"; //link

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Prediction-Key", "9916eea6e3be47dc8f87206b10e7ccb7"); //Prediction key of "If you have an image file" 
            var contentStream = new StreamContent(_photo.GetStream());

            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Procesando..."))
            {
                var response = await httpClient.PostAsync(endpoint, contentStream);

                if (!response.IsSuccessStatusCode)
                {
                    UserDialogs.Instance.Toast("Un error ha ocurrido");
                    return;
                }
                var json = await response.Content.ReadAsStringAsync();
                var prediction = JsonConvert.DeserializeObject<PredictionResponse>(json);
                var tag = prediction.Predictions.First();

                Resultado.Text = $"{tag.Tag} - {tag.Probability:p0}";
                Precision.Progress = tag.Probability;
            }
        }
    }

    //Al comentar la clase PredictionResponse se eliminan los errores en "Resultado.Text" y "Precision.Progress"
    //Pero hacer esto causa un fallo en la variable tag con respecto a la necesidad de la clase "PredictionResponse"
    public class PredictionResponse
    {
        public string Tag { get; set; }

        public string Id { get; set; }
        public string Probability { get; set; }
        public string Project { get; set; }
        public string Iteration { get; set; }
        public DateTime Created { get; set; }
        public Prediction[] Predictions { get; set; }

    }
}
