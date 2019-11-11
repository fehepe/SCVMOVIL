using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCVMobil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Verificacion : ContentPage
    {
        public Verificacion(Invitados invitados)
        {
            InitializeComponent();
            var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
            var COMPANIAS = db.Query<COMPANIAS>("SELECT * FROM COMPANIAS WHERE COMPANIA_ID = " + invitados.Compania_ID + "");
            //var PERSONAS = db.Query<PERSONAS>("SELECT PERSONA_ID FROM PERSONAS WHERE NOMBRES_APELLIDOS = '" + invitados.Placa + "'");
            lblNombre.Text = invitados.Nombres.ToString();
            lblApellido.Text = invitados.Apellidos.ToString();
            lblDestino.Text = COMPANIAS.First() == null ? "" : COMPANIAS.First().NOMBRE;
            //lblVisita.Text = PERSONAS.First().NOMBRES_APELLIDOS;
            lblCedula.Text = invitados.Cargo.ToString();
        }

        private async void btnCorrecto_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}