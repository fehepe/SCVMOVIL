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
    public partial class Salida : ContentPage
    {
        String cedula;
       /* List<string> country = new List<string>
        {
            "India",
            "pakistan",
            "Srilanka",
            "Bangladesh",
            "Afghanistan"
        }; */
        public Salida(String cedula)
        {
            InitializeComponent();
            this.cedula = cedula;

        }

        //Este metodo sirve para dar salida a una cedula que ya esta dentro
        private async void BtnSalida_Clicked(object sender, EventArgs e)
        {
            var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
            var querry = "SELECT * FROM Invitados WHERE CARGO = " + cedula+ " AND FECHA_SALIDA is null";
            var registroInv = db.Query<Invitados>(querry);
            registroInv.First().Fecha_Salida = DateTime.Now;
            registroInv.First().Subida = null;
            db.UpdateAll(registroInv);
            DependencyService.Get<IToastMessage>().DisplayMessage("Se ha dado salida correctamente.");
            await Navigation.PopToRootAsync();

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
    }
}