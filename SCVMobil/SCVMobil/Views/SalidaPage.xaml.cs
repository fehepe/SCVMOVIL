using Microsoft.AppCenter.Analytics;
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
    public partial class SalidaPage : ContentPage
    {
        String cedula, apellido, nombres;
     
        public SalidaPage(String cedula, String nombres, string apellidos)
        {
            InitializeComponent();
            this.cedula = cedula;
            this.nombres = nombres;
            this.apellido = apellidos;

        }

        //Este metodo sirve para dar salida a una cedula que ya esta dentro
        private async void BtnSalida_Clicked(object sender, EventArgs e)
        {
            Preferences.Get("BUSY", false);
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                var querry = "SELECT * FROM Invitados WHERE CARGO = '" + cedula + "' AND FECHA_SALIDA is null";
                var registroInv = db.Query<Invitados>(querry);
                registroInv.First().Fecha_Salida = DateTime.Now;
                registroInv.First().salidaSubida = null;
                db.UpdateAll(registroInv);
                DependencyService.Get<IToastMessage>().DisplayMessage("Se ha dado salida correctamente.");
                await Navigation.PopToRootAsync();
                //await Navigation.PushAsync(new CompanyPage(cedula, nombres, apellido));
                btnSalida.IsEnabled = false;

            }
            catch (Exception ee)
            {
                
                Analytics.TrackEvent("Error al dar salida " + ee.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
                throw;
            }
        }





        /* private void CountryList_ItemTapped(object sender, ItemTappedEventArgs e)
         {
             if (e.Item as string == null)
             {
                 return;
             }
             else
             {
                 CountryList.ItemsSource = country.Where(c => c.Equals(e.Item as string));
                 CountryList.IsVisible = true;
                 SearchContent.Text = e.Item as string;

             }


     }

         private void SearchContent_TextChanged(object sender, TextChangedEventArgs e)
         {
             var keyword = SearchContent.Text;
             if (keyword.Length >= 1)
             {
                 var suggestion = country.Where(c => c.ToLower().Contains(keyword.ToLower()));
                 CountryList.ItemsSource = suggestion;
                 CountryList.IsVisible = true;
             }
             else
             {
                 CountryList.IsVisible = false;
             }
         } */

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
          
        }

    }
}
