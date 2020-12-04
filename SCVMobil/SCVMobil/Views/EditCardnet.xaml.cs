using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCVMobil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCardnet : ContentPage
    {
        public EditCardnet()
        {
            InitializeComponent();
            fisrttext.Text = Preferences.Get("Empresa", "");
            secontext.Text = Preferences.Get("Departamento", "");
            thirdtext.Text = Preferences.Get("Piso", "");
        }

        private async void Savebtn_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("Empresa", fisrttext.Text);
            Preferences.Set("Departamento", secontext.Text);
            Preferences.Set("Piso", thirdtext.Text);
            await Navigation.PopAsync();
        }
    }
}