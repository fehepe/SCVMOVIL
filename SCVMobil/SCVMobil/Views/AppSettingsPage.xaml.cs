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
using Microsoft.AppCenter.Analytics;
using SCVMobil.Connections;

namespace SCVMobil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppSettingsPage : ContentPage
    {

        List<int> num = new List<int>();
        FireBirdData fire = new FireBirdData();
        public AppSettingsPage()
        {
            InitializeComponent();
            
        }
        

        private void BtConfigAvan_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage());
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            setdefaults();
            Localidad_VSU();
            visitaA.IsToggled = Preferences.Get("VISITA_A_SELECTED", true);
            entPuerta.SelectedIndex = Preferences.Get("PUERTA_SELECTED_INDEX", 0);
            placa.IsToggled= Preferences.Get("PLACA_SELECTED", true);
            TiempoVerif.SelectedItem = Preferences.Get("TIEMPOS", "1");
        }
        public void setdefaults()
        {
            if (Preferences.Get("TIEMPO_VER", "N/A") == "N/A")
            {
                Preferences.Set("TIEMPO_VER", "1");
            }
            
            if (!Preferences.Get("VERIFICA", false))
            {
                Preferences.Set("VERIFICA", false);
                verificacion.IsToggled = false;
                lblTiempo.IsVisible = false;
                TiempoVerif.IsVisible = false;
            }
            else
            {
                verificacion.IsToggled = true;
                lblTiempo.IsVisible = true;
                TiempoVerif.IsVisible = true;
            }
            
            
        }

        private void Localidad_VSU()
        {
            try
            {
                var scompania_id = Preferences.Get("LOCALIDAD_VSU", "1");
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                var puertas = db.Query<COMPANIAS>("SELECT * FROM COMPANIAS where PUNTO_VSU = 1");
                List<string> listpuertas = new List<string>();
                if (puertas.Any())
                {
                    entPuerta.ItemsSource = puertas;
                    entPuerta.SelectedItem = Preferences.Get("PUERTA_SELECTED_INDEX", 0);
                }
                else
                {
                    entPuerta.IsVisible = false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error en el metodo LOCALIDAD _VSU");
                Analytics.TrackEvent("Error al mostrar compañias: " + e.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
                
            }
        }


        private void VisitaA_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                Preferences.Set("VISITA_A_SELECTED", e.Value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception catched while trying to set preference \"VISITA_A_SELECTED\": " + ex);
            }
        }

      

        private void UltimaVisitaA_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                Preferences.Set("ULTIMA_VISITA_A_SELECTED", e.Value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception catched while trying to set preference \"ULTIMA_VISITA_A_SELECTED\": " + ex);
            }
        }

        private void Placa_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                Preferences.Set("PLACA_SELECTED", e.Value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception catched while trying to set preference \"PLACA_SELECTED\": " + ex);
            }
        }

        private async void BtnClearDB_Clicked(object sender, EventArgs e)
        {

            try
            {
                string Password = "cr52401";
                string TryPassword = await DisplayPromptAsync("verificacion","Ingrese la contraseña maestra para continuar","Eliminar","Cancelar","Contraseña",11,null);
                if (Password == TryPassword)
                {
                    var response = await App.Current.MainPage.DisplayAlert("Limpiar Base de Datos Local", "Desea que se elimine toda la informacion almacenada localmente?", "Si", "No");
                    if (response)
                    {
                        Preferences.Set("MAX_RESERVA_ID", "0");
                        Preferences.Set("MAX_COMPANIA_ID", "0");
                        Preferences.Set("MAX_PERSONA_ID", "0");
                        Preferences.Set("MAX_INVIDATO_ID", "0");
                        Preferences.Set("PERSONAS_LIST", "");
                        Preferences.Set("COMPANIAS_LIST", "");
                        Preferences.Set("CHUNK_SIZE", "50000");
                        Preferences.Set("MAX_DEPTO_LOCALIDAD", "0");
                        Preferences.Set("DESTINO_SELECTED", 0);

                        var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                        db.DeleteAll<COMPANIAS>();
                        db.DeleteAll<PERSONAS>();
                        db.DeleteAll<VW_RESERVA_VISITA>();
                        db.DeleteAll<Invitados>();
                        db.DeleteAll<InvitadosReservas>();
                        db.DeleteAll<SalidaOffline>();
                        db.DeleteAll<PLACA>();
                        db.DeleteAll<DEPTO_LOCALIDAD>();
                    } 
                }
                else
                {
                    await DisplayAlert("Credencial incorrecta","La credencial ingresada no es valida", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en BtnClearDB");
                Analytics.TrackEvent("Error al limpiar base de datos " + ex.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
               
            }

            
        }

       

      
       
        private void verificacion_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("VERIFICA", e.Value);
            if (!Preferences.Get("VERIFICA", false))
            {
                Preferences.Set("VERIFICA", false);
                verificacion.IsToggled = false;
                lblTiempo.IsVisible = false;
                TiempoVerif.IsVisible = false;
            }
            else
            {
                verificacion.IsToggled = true;
                lblTiempo.IsVisible = true;
                TiempoVerif.IsVisible = true;
            }
        }

        private void entPuerta_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var fireBird = new FireBirdData();
                var selected = entPuerta.SelectedItem as COMPANIAS;
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                Preferences.Set("LOCALIDAD_VSU", selected.COMPANIA_ID.ToString());
                Preferences.Set("PUERTA_SELECTED_INDEX", selected.COMPANIA_ID);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en entPuerta_SelectedIndexChanged");
                Analytics.TrackEvent("Error al mostrar compañias: " + ex.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            }


        }

        private void TiempoVerif_SelectedIndexChanged(object sender, EventArgs e)
        {
             Preferences.Set("TIEMPOS", TiempoVerif.SelectedItem.ToString());                        

        }

        private void verif_Clicked(object sender, EventArgs e)
        {
           

        }
    }
}