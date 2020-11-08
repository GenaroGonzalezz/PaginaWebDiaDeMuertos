
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Net.Http;
using Acr.UserDialogs;
using Newtonsoft.Json;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using System.Net;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using System.Net.Http.Headers;

namespace Recuerdame
{
    public partial class MainPage : ContentPage
    {
        private MediaFile _photo;
        string endpoint = "https://servsearch.cognitiveservices.azure.com/customvision/v3.0/Prediction/d6057e20-e203-4a2c-852d-bbbaa2fdc00f/detect/iterations/Iteration12/image"; //link
        public MainPage()
        {
            InitializeComponent();
            //MainPage = new NavigationPage(new ChatBot());
        }


        /*int count = 0;
        private void Handle_Clicked(object sender, EventArgs e)
        {
            count++;
            ((Button)sender).Text = $"You clicked {count} times.";
        }*/


        private async void Elegir_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Prediction-Key", "9916eea6e3be47dc8f87206b10e7ccb7"); //Prediction key of "If you have an image file" 

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Error", "No se puede tomar una imagen", "Cancelar");
                return;
            }

            var photo = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions());

            _photo = photo;
            ImgSource.Source = FileImageSource.FromFile(photo.Path);

            if (photo == null)
            {
                await DisplayAlert("Error", "No hay archivo", "Cancelar");
                return;
            }
            /*if (photo == null)
            {
                UserDialogs.Instance.Toast("Un error ha ocurrido");
                return;
            }
            else
            {
                //Task<MediaFile> PickPhotoAsync(PickMediaOptions options = null);

                _photo = photo;
                ImgSource.Source = FileImageSource.FromFile(photo.Path);
            }*/
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
            if (photo == null)
            {
                await DisplayAlert("Error", "No hay archivo", "Cancelar");
                return;
            }


            //var imageSource = ImageSource.FromStream(() =>
            //{
            //    var stream = photo.GetStream();
            //    ImgSource.Source = FileImageSource.FromStream(stream);
            //});



        }

        /* public static async Task MakePredictionRequest(string imageFilePath)
         {
             var client = new HttpClient();

             // Request headers - replace this example key with your valid Prediction-Key.
             client.DefaultRequestHeaders.Add("Prediction-Key", "9916eea6e3be47dc8f87206b10e7ccb7");

             // Prediction URL - replace this example URL with your valid Prediction URL.
             string url = "https://servsearch.cognitiveservices.azure.com/customvision/v3.0/Prediction/d6057e20-e203-4a2c-852d-bbbaa2fdc00f/detect/iterations/Iteration12/image";

             HttpResponseMessage response;

             // Request body. Try this sample with a locally stored image.
             byte[] byteData = GetImageAsByteArray(imageFilePath);

             using (var content = new ByteArrayContent(byteData))
             {
                 content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                 response = await client.PostAsync(url, content);
                 Console.WriteLine(await response.Content.ReadAsStringAsync());
             }
         }*/


        private async void Analizar_Clicked(object sender, EventArgs e)
        {
            var httpClient = new HttpClient();

            // const string endpoint = "https://westus2.api.cognitiveservices.azure.com/customvision/v3.0/Prediction/d6057e20-e203-4a2c-852d-bbbaa2fdc00f/detect/iterations/Iteration12/image";
            //var endpoint = "https://servsearch.cognitiveservices.azure.com/customvision/v3.0/Prediction/d6057e20-e203-4a2c-852d-bbbaa2fdc00f/detect/iterations/Iteration12/url";
            //var endpoint = "https://westus2.dev.cognitive.microsoft.com/docs/services/5adf991815e1060e6355ad44/operations/56f91f2e778daf14a499e1fa";
            const string endpoint = "https://servsearch.cognitiveservices.azure.com/customvision/v3.0/Prediction/d6057e20-e203-4a2c-852d-bbbaa2fdc00f/detect/iterations/Iteration12/image"; //link
            httpClient.DefaultRequestHeaders.Add("Prediction-Key", "9916eea6e3be47dc8f87206b10e7ccb7"); //Prediction key of "If you have an image file" 



            // try
            //{

            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Procesando..."))
            {
                var contentStream = new StreamContent(_photo.GetStream());
                var response = await httpClient.PostAsync(endpoint, contentStream);
                if (!response.IsSuccessStatusCode)
                {

                    UserDialogs.Instance.Toast("Un error ha ocurrido");
                    return;
                }


                var json = await response.Content.ReadAsStringAsync();
                var prediction = JsonConvert.DeserializeObject<Rootobject>(json);
                var tag = prediction.predictions.First();

                Resultado.Text = $"{tag.tagName} - {tag.probability:p0}";
                Precision.Progress = tag.probability;
                // return;
            }
        }

        public class Rootobject
        {
            public string id { get; set; }
            public string project { get; set; }
            public string iteration { get; set; }
            public DateTime created { get; set; }
            public Prediction[] predictions { get; set; }
        }

        public class Prediction
        {
            public float probability { get; set; }
            public string tagId { get; set; }
            public string tagName { get; set; }
            public Boundingbox boundingBox { get; set; }
        }

        public class Boundingbox
        {
            public float left { get; set; }
            public float top { get; set; }
            public float width { get; set; }
            public float height { get; set; }
        }
        private void Chatbot_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new ChatBot());
        }
    }
}
    /*if (!response.IsSuccessStatusCode)
            {
                UserDialogs.Instance.Toast("Un error ha ocurrido");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var prediction = JsonConvert.DeserializeObject<PredictionResponse>(json);
            var tag = prediction.Predictions.First();

            Resultado.Text = $"{tag.Entities} - {tag.Intents:p0}";
            //Precision.Progress = tag.Probability;
        }*/


    //catch (System.NullReferenceException)
    //{
    //  UserDialogs.Instance.Toast("Oops no cargaste imagen.");
    //return;
    //}

    //---------------------------------------------------------------------------------------------------------


    //Al comentar la clase PredictionResponse se eliminan los errores en "Resultado.Text" y "Precision.Progress"
    //Pero hacer esto causa un fallo en la variable tag con respecto a la necesidad de la clase "PredictionResponse"

   /* public class PredictionResponse
    {
        public string Tag { get; set; }
        public string Id { get; set; }
        public string Probability { get; set; }
        public string Project { get; set; }
        public string Iteration { get; set; }
        public DateTime Created { get; set; }
        public Prediction[] Predictions { get; set; }

    }*/
