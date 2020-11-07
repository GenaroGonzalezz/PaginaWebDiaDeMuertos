using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace Recuerdame
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatBot : ContentPage
    {
        public ChatBot()
        {
            InitializeComponent();
            var browser = new WebView();

            browser.Source = "https://webchat.botframework.com/embed/preguntasmuertos-bot?s=2K1K_zKguck._Iolu5aLOsJRffNN9ELB-1V_3G_AT16HYzkGkgztfmI";
            this.Content = browser;
        }
    }
}