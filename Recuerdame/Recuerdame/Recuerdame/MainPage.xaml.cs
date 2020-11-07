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

namespace Recuerdame
{
    public partial class MainPage : ContentPage
    {
        private MediaFile _photo;

        public MainPage()
        {
            InitializeComponent();
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

            _photo = photo;
            ImgSource.Source = FileImageSource.FromFile(photo.Path);
        }

        private async void Tomar_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            {
                Directory = "Recuerdame",
                Name = "source.jpg"
            });
            _photo = photo;
            ImgSource.Source = FileImageSource.FromFile(photo.Path);
        }

        private async void Analizar_Clicked(object sender, EventArgs e)
        {
            const string endpoint = ""; //link

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Prediction-Key", ""); //Prediction key of "If you have an image file" 

            var contentStream = new StreamContent(_photo.GetStream());

            var response = await httpClient.PostAsync(endpoint, contentStream);

            if(!response.IsSuccessStatusCode)
            {
                UserDialogs.Instance.Toast("Un error ha ocurrido");
            }

            var json = await response.Content.ReadAsStringAsync();
        }
    }
}
