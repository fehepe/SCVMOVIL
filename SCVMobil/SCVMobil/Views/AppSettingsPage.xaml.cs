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
    public partial class AppSettingsPage : ContentPage
    {
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
            placa.IsToggled= Preferences.Get("PLACA_SELECTED", true);
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
                entTiempo.IsVisible = false;
            }
            else
            {
                verificacion.IsToggled = true;
                lblTiempo.IsVisible = true;
                entTiempo.IsVisible = true;
            }
            
            
        }
        private void Localidad_VSU()
        {


            var scompania_id = Preferences.Get("LOCALIDAD_VSU","1");
            var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
            var puertas = db.Query<COMPANIAS>("SELECT * FROM COMPANIAS where PUNTO_VSU = 1");
            List<string> listpuertas = new List<string>();
            if (puertas.Any())
            {
                entPuerta.IsVisible = true;
                foreach (var item in puertas)
                {
                    listpuertas.Add(item.NOMBRE);
                }
                entPuerta.ItemsSource = listpuertas;
                if (scompania_id != "1")
                {
                    entPuerta.SelectedItem = (puertas.Where(x => x.COMPANIA_ID == Convert.ToInt32(scompania_id)).First().NOMBRE);
                }
               

            }
            else
            {
                entPuerta.IsVisible = false;
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

            var response = await App.Current.MainPage.DisplayAlert("Limpiar Base de Datos Local","Desea que se elimine toda la informacion almacenada localmente?","Si","No");
            if (response)
            {
                Preferences.Set("MAX_RESERVA_ID", "0");
                Preferences.Set("MAX_COMPANIA_ID", "0");
                Preferences.Set("MAX_PERSONA_ID", "0");
                Preferences.Set("MAX_INVIDATO_ID", "0");
                Preferences.Set("PERSONAS_LIST", "");
                Preferences.Set("COMPANIAS_LIST", "");
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                db.DeleteAll<COMPANIAS>();
                db.DeleteAll<PERSONAS>();
                db.DeleteAll<VW_RESERVA_VISITA>();
                db.DeleteAll<Invitados>();
                db.DeleteAll<InvitadosReservas>();
                db.DeleteAll<SalidaOffline>();
                db.DeleteAll<PLACA>();
            }

            
        }

        private void entTiempo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

      
        private void entTiempo_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void verificacion_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("VERIFICA", e.Value);
            if (!Preferences.Get("VERIFICA", false))
            {
                Preferences.Set("VERIFICA", false);
                verificacion.IsToggled = false;
                lblTiempo.IsVisible = false;
                entTiempo.IsVisible = false;
            }
            else
            {
                verificacion.IsToggled = true;
                lblTiempo.IsVisible = true;
                entTiempo.IsVisible = true;
            }
        }

        private void entPuerta_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var nombreLocalidad = entPuerta.SelectedItem.ToString();
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                var puertas = db.Query<COMPANIAS>($"SELECT * FROM COMPANIAS where NOMBRE = '{nombreLocalidad}'");
                if (puertas.Any())
                {
                    Preferences.Set("LOCALIDAD_VSU", puertas.First().COMPANIA_ID.ToString());
                }
            }
            catch (Exception)
            {
                
            }
            
           
        }
    }
}