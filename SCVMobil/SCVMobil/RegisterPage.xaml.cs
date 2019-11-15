using Microsoft.AppCenter.Crashes;
using Rg.Plugins.Popup.Services;
using SCVMobil.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCVMobil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(string data)
        {
            InitializeComponent();
            EntryCedula.Text = data;
        }

    

        private async void Agregar_Clicked_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EntryCedula.Text) || string.IsNullOrEmpty(EntryNombre.Text) || string.IsNullOrEmpty(EntryApellido.Text) || EntryCedula.Text.Length != 11)
            {
                await PopupNavigation.PushAsync(new PopUpDatosIncorrectos());
            }
            else
            {

                var padronRegistro = new PADRON();
                padronRegistro.CEDULA = EntryCedula.Text;
                padronRegistro.NOMBRES = EntryNombre.Text.ToUpper();
                padronRegistro.APELLIDO1 = EntryApellido.Text.ToUpper();
                padronRegistro.APELLIDO2 = "";
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                try
                {
                    db.Insert(padronRegistro);
                    await PopupNavigation.PushAsync(new PopUpDatosCorrectos());
                    Navigation.PushAsync(new CompanyPage(EntryCedula.Text, EntryNombre.Text, EntryApellido.Text));
                    EntryApellido.Text = "";
                    EntryCedula.Text = "";
                    EntryNombre.Text = "";
                    return;


                }
                catch (Exception ey)
                {
                    var properties = new Dictionary<string, string> {
                        { "Category", "Error insertando en la base de datos." },
                        { "Code", "MainPage.xaml.cs Line: 313" },
                        { "Lector", Preferences.Get("LECTOR", "0")}
                    };
                    Crashes.TrackError(ey, properties);
                    Debug.WriteLine("No se pudo Insertar el documento. Exception: " + ey.ToString());

                }


            }
        }
    }
}