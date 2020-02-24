using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCVMobil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisitInfo : PopupPage
    {
        public VisitInfo(string nombre, string documento, string destino, string fechaentrada, string fechaverificacion )
        {
            InitializeComponent();
            Nombretxt.Text = nombre;
            Cedulatxt.Text = documento;
            Destinotxt.Text = destino;
            Fechaenttxt.Text = fechaentrada;
            Fechavertxt.Text = fechaverificacion;
            
        }

        private async void Okbtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}