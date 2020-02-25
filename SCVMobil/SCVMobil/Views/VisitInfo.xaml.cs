using Rg.Plugins.Popup.Pages;
using SCVMobil.Models;
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
        public VisitInfo(Invitados invitados, string company)
        {
            InitializeComponent();
            Nombretxt.Text = invitados.Nombres + " " +  invitados.Apellidos;
            Cedulatxt.Text = invitados.Cargo;
            Destinotxt.Text = company;
            Fechaenttxt.Text = invitados.Fecha_Registro.ToString();
            
            
        }

        private void Okbtn_Clicked(object sender, EventArgs e)
        {
            this.IsVisible = false;
        }
    }
}