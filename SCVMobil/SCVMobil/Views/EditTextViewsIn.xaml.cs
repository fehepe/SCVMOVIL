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
    public partial class EditTextViewsIn : ContentPage
    {
        public OptionsIn options = new OptionsIn();
        public EditTextViewsIn()
        {
            InitializeComponent();
            tog1.IsToggled = Preferences.Get("texto1entrada", true);
            tog2.IsToggled = Preferences.Get("texto2entrada", true);
            tog3.IsToggled = Preferences.Get("texto3entrada", true);
            tog4.IsToggled = Preferences.Get("texto4entrada", true);
            tog5.IsToggled = Preferences.Get("texto5entrada", true);
            //tog6.IsToggled = Preferences.Get("texto6entrada", true);


            var pp = Preferences.Get("title1", "");
            if (string.IsNullOrEmpty(pp))
            {
                txt1.Text = "Texto Entrada 1";
            }
            else
            {
                txt1.Text = pp;
            }

            var pp2 = Preferences.Get("title2", "");
            if (string.IsNullOrEmpty(pp2))
            {
                txt2.Text = "Texto Entrada 2";
            }
            else
            {
                txt2.Text = pp2;
            }

            var pp3 = Preferences.Get("title3", "");
            if (string.IsNullOrEmpty(pp3))
            {
                txt3.Text = "Texto Entrada 3";
            }
            else
            {
                txt3.Text = pp3;
            }

            var pp4 = Preferences.Get("title4", "");
            if (string.IsNullOrEmpty(pp4))
            {
                txt4.Text = "Texto Entrada 4";
            }
            else
            {
                txt4.Text = pp4;
            }

            var pp5 = Preferences.Get("title5", "");
            if (string.IsNullOrEmpty(pp5))
            {
                txt5.Text = "Texto Entrada 5";
            }
            else
            {
                txt5.Text = pp5;
            }

            //var pp6 = Preferences.Get("title6", "");
            //if (string.IsNullOrEmpty(pp6))
            //{
            //    txt6.Text = "Texto Entrada 6";
            //}
            //else
            //{
            //    txt6.Text = pp6;
            //}

            //txt1.Text = Preferences.Get("title1", "");
            //txt2.Text = Preferences.Get("title2", "");
            //txt3.Text = Preferences.Get("title3", "");
            //txt4.Text = Preferences.Get("title4", "");
            //txt5.Text = Preferences.Get("title5", "");
            //txt6.Text = Preferences.Get("title6", "");

            var tile = new TapGestureRecognizer();
            tile.Tapped += async (s, e) =>
            {
                var typeoption = await DisplayPromptAsync(title: "Escriba una opcion", message: "", accept: "ok", cancel: "cancelar");
                if (string.IsNullOrEmpty(typeoption))
                {
                    await DisplayAlert("", "No puede dejar campos vacios", "ok");
                }
                else
                {
                    Preferences.Set("title1", typeoption);
                    var p = Preferences.Get("title1", "");
                    txt1.Text = p;
                }
            };
            txt1.GestureRecognizers.Add(tile);
            var tile2 = new TapGestureRecognizer();
            tile2.Tapped += async (s, e) =>
            {
                var typeoption = await DisplayPromptAsync(title: "Escriba una opcion", message: "", accept: "ok", cancel: "cancelar");
                if (string.IsNullOrEmpty(typeoption))
                {
                    await DisplayAlert("", "No puede dejar campos vacios", "ok");
                }
                else
                {
                    Preferences.Set("title2", typeoption);
                    var p = Preferences.Get("title2", "");
                    txt2.Text = p;
                }
            };
            txt2.GestureRecognizers.Add(tile2);
            var tile3 = new TapGestureRecognizer();
            tile3.Tapped += async (s, e) =>
            {
                var typeoption = await DisplayPromptAsync(title: "Escriba una opcion", message: "", accept: "ok", cancel: "cancelar");
                if (string.IsNullOrEmpty(typeoption))
                {
                    await DisplayAlert("", "No puede dejar campos vacios", "ok");
                }
                else
                {
                    Preferences.Set("title3", typeoption);
                    var p = Preferences.Get("title3", "");
                    txt3.Text = p;
                }
            };
            txt3.GestureRecognizers.Add(tile3);
            var tile4 = new TapGestureRecognizer();
            tile3.Tapped += async (s, e) =>
            {
                var typeoption = await DisplayPromptAsync(title: "Escriba una opcion", message: "", accept: "ok", cancel: "cancelar");
                if (string.IsNullOrEmpty(typeoption))
                {
                    await DisplayAlert("", "No puede dejar campos vacios", "ok");
                }
                else
                {
                    Preferences.Set("title4", typeoption);
                    var p = Preferences.Get("title4", "");
                    txt4.Text = p;
                }
            };
            txt4.GestureRecognizers.Add(tile4);
            var tile5 = new TapGestureRecognizer();
            tile5.Tapped += async (s, e) =>
            {
                var typeoption = await DisplayPromptAsync(title: "Escriba una opcion", message: "", accept: "ok", cancel: "cancelar");
                if (string.IsNullOrEmpty(typeoption))
                {
                    await DisplayAlert("", "No puede dejar campos vacios", "ok");
                }
                else
                {
                    Preferences.Set("title5", typeoption);
                    var p = Preferences.Get("title5", "");
                    txt5.Text = p;
                }
            };
            txt5.GestureRecognizers.Add(tile5);
            //var tile6 = new TapGestureRecognizer();
            //tile6.Tapped += async (s, e) =>
            //{
            //    var typeoption = await DisplayPromptAsync(title: "Escriba una opcion", message: "", accept: "ok", cancel: "cancelar");
            //    if (string.IsNullOrEmpty(typeoption))
            //    {
            //        await DisplayAlert("", "No puede dejar campos vacios", "ok");
            //    }
            //    else
            //    {
            //        Preferences.Set("title6", typeoption);
            //        var p = Preferences.Get("titl6", "");
            //        txt6.Text = p;
            //    }
            //};
            //txt6.GestureRecognizers.Add(tile6);
        }

        private async void cm1_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EntradaCombo1());
        }

        private async void cm2_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EntradaCombo2());
        }

        private async void cm3_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EntradaCombo3());
        }

        private void tog1_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("texto1entrada", e.Value);
        }

        private void tog2_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("texto2entrada", e.Value);
        }

        private void tog3_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("texto3entrada", e.Value);
        }

        private void tog4_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("texto4entrada", e.Value);
        }

        private void tog5_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("texto5entrada", e.Value);
        }

        //private void tog6_Toggled(object sender, ToggledEventArgs e)
        //{
        //    Preferences.Set("texto6entrada", e.Value);
        //}
    }
}