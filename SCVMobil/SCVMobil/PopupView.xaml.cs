using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCVMobil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupView : PopupPage
    {
        public PopupView()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (entPassword.Text == "1")
            {                
                entPassword.Text = string.Empty;
                Navigation.PushAsync(new AppSettingsPage());
            }
            else
            {
                try
                {
                    var duration = TimeSpan.FromSeconds(0.5);
                }
                catch
                {
                    //TODO: HANDLE NO VIBRATE
                }
            }
        }
    }
}