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
    public partial class EditTextViews : ContentPage
    {
        public Options options = new Options();
        public EditTextViews()
        {
            InitializeComponent();
            tog1.IsToggled = Preferences.Get("texto1", true);
            tog2.IsToggled = Preferences.Get("texto2", true);
            tog3.IsToggled = Preferences.Get("texto3", true);
            tog4.IsToggled = Preferences.Get("texto4", true);
            tog5.IsToggled = Preferences.Get("texto5", true);
            //tog6.IsToggled = Preferences.Get("texto6", true);

            //txt1.Text = Preferences.Get("title1salida", "");
            //txt2.Text = Preferences.Get("title1salida", "");
            //txt3.Text = Preferences.Get("title3salida", "");
            //txt4.Text = Preferences.Get("title4salida", "");
            //txt5.Text = Preferences.Get("title5salida", "");
            //txt6.Text = Preferences.Get("title6salida", "");

            var pp = Preferences.Get("title1salida", "");
            if (string.IsNullOrEmpty(pp))
            {
                txt1.Text = "Texto Salida 1";
                Preferences.Set("title1salida", "Texto Salida 1");
            }
            else
            {
                txt1.Text = pp;
            }

            var pp2 = Preferences.Get("title2salida", "");
            if (string.IsNullOrEmpty(pp2))
            {
                txt2.Text = "Texto Salida 2";
                Preferences.Set("title2salida", "Texto Salida 2");
            }
            else
            {
                txt2.Text = pp2;
            }

            var pp3 = Preferences.Get("title3salida", "");
            if (string.IsNullOrEmpty(pp3))
            {
                txt3.Text = "Texto Salida 3";
                Preferences.Set("title3salida", txt3.Text);
            }
            else
            {
                txt3.Text = pp3;
            }

            var pp4 = Preferences.Get("title4salida", "");
            if (string.IsNullOrEmpty(pp4))
            {
                txt4.Text = "Texto Salida 4";
                Preferences.Set("title4salida", txt4.Text);
            }
            else
            {
                txt4.Text = pp4;
            }

            var pp5 = Preferences.Get("title5salida", "");
            if (string.IsNullOrEmpty(pp5))
            {
                txt5.Text = "Texto Salida 5";
                Preferences.Set("title5salida", txt5.Text);
            }
            else
            {
                txt5.Text = pp5;
            }

            //var pp6 = Preferences.Get("title6salida", "");
            //if (string.IsNullOrEmpty(pp6))
            //{
            //    txt6.Text = "Texto Salida 6";
            //    Preferences.Set("title6salida", txt6.Text);
            //}
            //else
            //{
            //    txt6.Text = pp6;
            //}
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
                    Preferences.Set("title1salida", typeoption);
                    var p = Preferences.Get("title1salida", "");
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
                    Preferences.Set("title2salida", typeoption);
                    var p = Preferences.Get("title2salida", "");
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
                    Preferences.Set("title3salida", typeoption);
                    var p = Preferences.Get("title3salida", "");
                    txt3.Text = p;
                }
            };
            txt3.GestureRecognizers.Add(tile3);
            var tile4 = new TapGestureRecognizer();
            tile4.Tapped += async (s, e) =>
            {
                var typeoption = await DisplayPromptAsync(title: "Escriba una opcion", message: "", accept: "ok", cancel: "cancelar");
                if (string.IsNullOrEmpty(typeoption))
                {
                    await DisplayAlert("", "No puede dejar campos vacios", "ok");
                }
                else
                {
                    Preferences.Set("title4salida", typeoption);
                    var p = Preferences.Get("title4salida", "");
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
                    Preferences.Set("title5salida", typeoption);
                    var p = Preferences.Get("title5salida", "");
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
            //        Preferences.Set("title6salida", typeoption);
            //        var p = Preferences.Get("titl6salida", "");
            //        txt6.Text = p;
            //    }
            //};
            //txt6.GestureRecognizers.Add(tile6);
        }

        private async void cm1_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SalidaCombo1());
        }

        private async void cm2_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SalidaCombo2());
        }

        private async void cm3_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SalidaCombo3());
        }

        private void tog1_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("texto1", e.Value);
        }

        private void tog2_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("texto2", e.Value);
        }

        private void tog3_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("texto3", e.Value);
        }

        private void tog4_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("texto4", e.Value);
        }

        private void tog5_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("texto5", e.Value);
        }

        private void tog6_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("texto6", e.Value);
        }
    }
}