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
    public partial class PopUpGuardarConfig : PopupPage
    {
        public PopUpGuardarConfig()
        {
            InitializeComponent();
        }

        private async void configsave_Clicked(object sender, EventArgs e)
        {
            this.Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 1]); //volver al appsettings//
            this.IsVisible = false; //cerrar popup//
            
            //await Navigation.PushAsync(new AppSettingsPage());
            //await this.Navigation.PopAsync();
        }
    }
}