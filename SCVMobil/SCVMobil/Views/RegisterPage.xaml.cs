using Microsoft.AppCenter.Crashes;
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
using SCVMobil.Models;

namespace SCVMobil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(String data)
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(data))
            {
                DisplayAlert("Error", "No puede enviar vacio", "OK");
            }
            else
            {
                entCedula.Text = data;
            }


        }
        private void BtAgregar_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(entCedula.Text) || string.IsNullOrEmpty(entNombres.Text) || string.IsNullOrEmpty(entApellidos.Text) || entCedula.Text.Length != 11)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Registro",
                    "Introduzca los datos correctamente",
                    "Ok");


            }
            else
            {

                var padronRegistro = new PADRON();
                padronRegistro.CEDULA = entCedula.Text;
                padronRegistro.NOMBRES = entNombres.Text.ToUpper();
                padronRegistro.APELLIDO1 = entApellidos.Text.ToUpper();
                padronRegistro.APELLIDO2 = "";
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                try
                {
                    db.Insert(padronRegistro);
                    Application.Current.MainPage.DisplayAlert(
                   "Confirmado",
                   "Datos Ingresados Correctamente",
                   "accept");
                    Navigation.PushAsync(new CompanyPage(entCedula.Text, entNombres.Text, entApellidos.Text));
                    entApellidos.Text = "";
                    entCedula.Text = "";
                    entNombres.Text = "";
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