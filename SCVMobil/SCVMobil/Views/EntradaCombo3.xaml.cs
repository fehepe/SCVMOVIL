using SCVMobil.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCVMobil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EntradaCombo3 : ContentPage
    {
        public Options options = new Options();
        public EntradaCombo3()
        {
            InitializeComponent();
            GetOptions();
        }

        public void GetOptions()
        {
            var cs = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
            var retrievelist = cs.Query<Options>($"SELECT * FROM OPTIONS WHERE COMBO = 'COMBO ENTRADA 3'");
            if (retrievelist.Count < 1)
            {
                ListOptions.IsVisible = false;
            }
            else
            {
                ListOptions.ItemsSource = retrievelist;
            }
        }

        private void Deletebtn_Clicked(object sender, EventArgs e)
        {
            var sel = sender as Xamarin.Forms.MenuItem;

            var cs = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
            var obtainkey = cs.Query<Options>($"SELECT * FROM OPTIONS WHERE OptionDesc = '{sel.CommandParameter}' AND COMBO = 'COMBO ENTRADA 3'");
            cs.Query<Options>($"DELETE FROM Options WHERE ID_Option = {obtainkey.First().ID_Option}");
            var retrievelist = cs.Query<Options>($"SELECT * FROM OPTIONS WHERE COMBO = 'COMBO ENTRADA 3'");
            if (retrievelist.Count < 1)
            {
                ListOptions.IsVisible = false;
            }
            else
            {
                ListOptions.ItemsSource = retrievelist;
            }
        }

        private async void Addbtn_Clicked(object sender, EventArgs e)
        {
            var typeoption = await DisplayPromptAsync(title: "Escriba una opcion", message: "", accept: "ok", cancel: "cancelar");
            var cs = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
            options.OptionDesc = typeoption;
            options.combo = "COMBO ENTRADA 3";
            if (!string.IsNullOrEmpty(options.OptionDesc))
            {
                var obtainkey = cs.Query<Options>($"SELECT * FROM OPTIONS WHERE OptionDesc = '{options.OptionDesc}' AND COMBO = 'COMBO ENTRADA 3'");
                if (obtainkey.Count > 0)
                {
                    await DisplayAlert("", "No puede agregar la misma opcion 2 veces", "ok");
                }
                else
                {
                    cs.Insert(options);
                    ListOptions.ItemsSource = cs.Query<Options>($"SELECT * FROM OPTIONS WHERE COMBO = 'COMBO ENTRADA 3'");
                }

            }
            else
            {
                await DisplayAlert("", "No puede dejar el campo vacio", "Ok");
            }
        }
    }
}