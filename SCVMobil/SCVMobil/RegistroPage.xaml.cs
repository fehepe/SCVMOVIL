﻿using Microsoft.AppCenter.Crashes;
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
    public partial class RegistroPage : ContentPage
    {
        public RegistroPage()
        {
            InitializeComponent();
        }

        private void Agregar_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EntryCedula.Text) || string.IsNullOrEmpty(EntryNombre.Text) || string.IsNullOrEmpty(EntryApellido.Text) || EntryCedula.Text.Length != 11)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Registro",
                    "Introduzca los datos correctamente",
                    "Ok");


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
                    Application.Current.MainPage.DisplayAlert(
                   "Confirmado",
                   "Datos Ingresados Correctamente",
                   "accept");
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