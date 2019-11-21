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
    public partial class PopUpCedulaNoEncontradaGrupo : PopupPage
    {
        public PopUpCedulaNoEncontradaGrupo()
        {
            InitializeComponent();
        }
        private void Agregar_Clicked(object sender, EventArgs e)
        {
            this.IsVisible = false;
        }
    }
}