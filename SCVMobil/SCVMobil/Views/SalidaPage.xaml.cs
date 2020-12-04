using Microsoft.AppCenter.Analytics;
using Rg.Plugins.Popup.Services;
using SCVMobil.Models;
using SCVMobil.Views;
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
        string comment, comment2, comment3, comment4, comment5, comment6;
        public Options Options { get; set; } = new Options();
     
        public SalidaPage()
        {
            InitializeComponent();
            lbl4.IsVisible = false;
            lbl5.IsVisible = false;
            lbl6.IsVisible = false;
            lbl7.IsVisible = false;
            lbl8.IsVisible = false;
            GetCombos();

            var p1 = Preferences.Get("texto1", true);
            var p2 = Preferences.Get("texto2", true);
            var p3 = Preferences.Get("texto3", true);
            var p4 = Preferences.Get("texto4", true);
            var p5 = Preferences.Get("texto5", true);
            var p6 = Preferences.Get("texto6", true);

            if (p1 == true)
            {
                lbl4.Text = Preferences.Get("title1salida", "");
                lbl4.IsVisible = true;
            }
            else
            {
                lbl4.IsVisible = false;

            }

            if (p2 == true)
            {
                lbl5.Text = Preferences.Get("title2salida", "");
                lbl5.IsVisible = true;
            }
            else
            {
                lbl5.IsVisible = false;
            }

            if (p3 == true)
            {
                lbl6.Text = Preferences.Get("title3salida", "");
                lbl6.IsVisible = true;
            }
            else
            {
                lbl6.IsVisible = false;

            }

            if (p4 == true)
            {
                lbl7.Text = Preferences.Get("title4salida", "");
                lbl7.IsVisible = true;
            }
            else
            {
                lbl7.IsVisible = false;

            }

            if (p5 == true)
            {
                lbl8.Text = Preferences.Get("title5salida", "");
                lbl8.IsVisible = true;
            }
            else
            {
                lbl8.IsVisible = false;

            }
        }

        public SalidaPage(String cedula, String nombres, string apellidos)
        {
            InitializeComponent();
            this.cedula = cedula;
            this.nombres = nombres;
            this.apellido = apellidos;
            frame4.IsVisible = false;
            frame5.IsVisible = false;
            lblcm.IsVisible = false;
            //chx1.IsVisible = false;
            //chx2.IsVisible = false;
            //chx3.IsVisible = false;
            //chx4.IsVisible = false;
            //chx5.IsVisible = false;
            //lbl1.IsVisible = false;
            //lbl2.IsVisible = false;
            //lbl3.IsVisible = false;
            lbl4.IsVisible = false;
            lbl5.IsVisible = false;
            lbl6.IsVisible = false;
            lbl7.IsVisible = false;
            lbl8.IsVisible = false;

            var p1 = Preferences.Get("texto1", true);
            var p2 = Preferences.Get("texto2", true);
            var p3 = Preferences.Get("texto3", true);
            var p4 = Preferences.Get("texto4", true);
            var p5 = Preferences.Get("texto5", true);
            var p6 = Preferences.Get("texto6", true);

            if (p1 == true)
            {
                lbl4.Text = Preferences.Get("title1salida", "");
                lbl4.IsVisible = true;
            }
            else
            {
                lbl4.IsVisible = false;

            }

            if (p2 == true)
            {
                lbl4.Text = Preferences.Get("title2salida", "");
                lbl4.IsVisible = true;
            }
            else
            {
                lbl4.IsVisible = false;
            }

            if (p3 == true)
            {
                lbl6.Text = Preferences.Get("title3salida", "");
                lbl6.IsVisible = true;
            }
            else
            {
                lbl4.IsVisible = false;

            }

            if (p4 == true)
            {
                lbl7.Text = Preferences.Get("title4salida", "");
                lbl7.IsVisible = true;
            }
            else
            {
                lbl7.IsVisible = false;

            }

            if (p5 == true)
            {
                lbl8.Text = Preferences.Get("title5salida", "");
                lbl8.IsVisible = true;
            }
            else
            {
                lbl8.IsVisible = false;

            }
            //lbl4.Text = Preferences.Get("title1salida", "");
            //lbl5.Text = Preferences.Get("title2salida", "");
            //lbl6.Text = Preferences.Get("title3salida", "");

            //fcombo1.IsVisible = false;
            //fcombo2.IsVisible = false;
            //fcombo3.IsVisible = false;
            //var tile = new TapGestureRecognizer();
            //tile.Tapped += async (s, e) =>
            //{
            //    var typeoption = await DisplayPromptAsync(title: "Escriba una opcion", message: "", accept: "ok", cancel: "cancelar");
            //    if (string.IsNullOrEmpty(typeoption))
            //    {
            //        await DisplayAlert("", "No puede dejar campos vacios", "ok");
            //    }
            //    else
            //    {
            //        Preferences.Set("title1", typeoption);
            //        var p = Preferences.Get("title1", "");
            //        lbl4.Text = p;
            //    }
            //};
            //lbl4.GestureRecognizers.Add(tile);

            //var tile2 = new TapGestureRecognizer();
            //tile2.Tapped += async (s, e) =>
            //{
            //    var typeoption = await DisplayPromptAsync(title: "Escriba una opcion", message: "", accept: "ok", cancel: "cancelar");
            //    if (string.IsNullOrEmpty(typeoption))
            //    {
            //        await DisplayAlert("", "No puede dejar campos vacios", "ok");
            //    }
            //    else
            //    {
            //        Preferences.Set("title2", typeoption);
            //        var p = Preferences.Get("title2", "");
            //        lbl5.Text = p;
            //    }
            //};
            //lbl5.GestureRecognizers.Add(tile2);

            //var tile4 = new TapGestureRecognizer();
            //tile4.Tapped += async (s, e) =>
            //{
            //    var typeoption = await DisplayPromptAsync(title: "Escriba una opcion", message: "", accept: "ok", cancel: "cancelar");
            //    if (string.IsNullOrEmpty(typeoption))
            //    {
            //        await DisplayAlert("", "No puede dejar campos vacios", "ok");
            //    }
            //    else
            //    {
            //        Preferences.Set("title3", typeoption);
            //        var p = Preferences.Get("title3", "");
            //        lbl6.Text = p;

            //    }
            //};
            //lbl6.GestureRecognizers.Add(tile4);

            GetCombos();

        }

       

        private void desc_Clicked(object sender, EventArgs e)
        {
            desc.IsEnabled = false;
            Commentbtn.IsEnabled = true;
            lblcm.IsVisible = false;
            frame4.IsVisible = false;
            frame5.IsVisible = false;
            //chx1.IsVisible = false;
            //chx2.IsVisible = false;
            //chx3.IsVisible = false;
            //chx4.IsVisible = false;
            //chx5.IsVisible = false;
            //if (!string.IsNullOrEmpty(lbl1.Text))
            //{
            //    lbl1.IsVisible = false;
            //    fcombo1.IsVisible = false;
            //    //Campos.Text = "Desactivar";
            //}

            //if (!string.IsNullOrEmpty(lbl2.Text))
            //{
            //    lbl2.IsVisible = false;
            //    fcombo2.IsVisible = false;
            //    //Campos.Text = "Desactivar";
            //}
            //if (!string.IsNullOrEmpty(lbl3.Text))
            //{
            //    lbl3.IsVisible = false;
            //    fcombo3.IsVisible = false;
            //    //Campos.Text = "Desactivar";
            //}
            //var list1 = Comboone.Where(elem => elem != "" && elem != null).ToList();
            if (cmbbox1.ItemsSource.Count > 0)
            {
                lbl4.IsVisible = false;
                frame1.IsVisible = false;
               // chx1.IsVisible = false;
                // Campos.Text = "Desactivar";
            }
           // var list2 = Comboscn.Where(elem => elem != "" && elem != null).ToList();
            if (cmbbox2.ItemsSource.Count > 0)
            {
                lbl5.IsVisible = false;
                frame2.IsVisible = false;
              //  chx2.IsVisible = false;
                //Campos.Text = "Desactivar";
            }
           // var list3 = Combothr.Where(elem => elem != "" && elem != null).ToList();
            if (cmbbox3.ItemsSource.Count > 0)
            {
                lbl6.IsVisible = false;
                frame3.IsVisible = false;
                
                //Campos.Text = "Desactivar";
            }

        }

        //private void chx1_CheckChanged(object sender, EventArgs e)
        //{
        //    if (frame1.IsVisible == false)
        //    {
        //        frame1.IsVisible = true;
        //        lbl4.IsVisible = true;
        //        chx1.Text = "Desactivar campo ↓";
        //    }
        //    else
        //    {
        //        frame1.IsVisible = false;
        //        lbl4.IsVisible = false;
        //        chx1.Text = "Activar campo ↓";
        //    }
        //}

        //private void chx2_CheckChanged(object sender, EventArgs e)
        //{
        //    if (frame2.IsVisible == false)
        //    {
        //        frame2.IsVisible = true;
        //        chx2.Text = "Desactivar campo ↓";
        //        lbl5.IsVisible = true;
        //        chx2.Type = Plugin.InputKit.Shared.Controls.CheckBox.CheckType.Cross;
        //    }
        //    else
        //    {
        //        frame2.IsVisible = false;
        //        lbl5.IsVisible = false;
        //        chx2.Text = "Activar campo ↓";
        //    }
        //}

        //private void chx3_CheckChanged(object sender, EventArgs e)
        //{
        //    if (frame3.IsVisible == false)
        //    {
        //        frame3.IsVisible = true;
        //        chx3.Text = "Desactivar campo ↓";
        //        lbl6.IsVisible = true;
        //        chx3.Type = Plugin.InputKit.Shared.Controls.CheckBox.CheckType.Cross;
        //    }
        //    else
        //    {
        //        frame3.IsVisible = false;
        //        lbl6.IsVisible = false;
        //        chx3.Text = "Activar campo ↓";
        //    }
        //}

        //private void chx4_CheckChanged(object sender, EventArgs e)
        //{
        //    if (frame4.IsVisible == false)
        //    {
        //        frame4.IsVisible = true;
        //        chx4.Text = "Desactivar campo ↓";
        //        chx4.Type = Plugin.InputKit.Shared.Controls.CheckBox.CheckType.Cross;
        //    }
        //    else
        //    {
        //        frame4.IsVisible = false;
        //        chx4.Text = "Activar campo ↓";
        //    }
        //}

        //private void chx5_CheckChanged(object sender, EventArgs e)
        //{
        //    if (frame5.IsVisible == false)
        //    {
        //        frame5.IsVisible = true;
        //        chx5.Text = "Desactivar campo ↓";
        //        chx5.Type = Plugin.InputKit.Shared.Controls.CheckBox.CheckType.Cross;
        //    }
        //    else
        //    {
        //        frame5.IsVisible = false;
        //        chx5.Text = "Activar campo ↓";
        //    }
        //}

        public void GetCombos()
        {
            //lbl1.Text = Preferences.Get("texto1", "");
            //lbl2.Text = Preferences.Get("texto2", "");
            //lbl3.Text = Preferences.Get("texto3", "");


            //var textc1 = Preferences.Get("sal1", "");
            //var textc2 = Preferences.Get("sal2", "");
            //var textc3 = Preferences.Get("sal3", "");

            //Comboone.Add(textc1);
            //Comboone.Add(textc2);
            //Comboone.Add(textc3);


            //var textc4 = Preferences.Get("sal4", "");
            //var textc5 = Preferences.Get("sal5", "");
            //var textc6 = Preferences.Get("sal6", "");

            //Comboscn.Add(textc4);
            //Comboscn.Add(textc5);
            //Comboscn.Add(textc6);

            //var textc7 = Preferences.Get("sal7", "");
            //var textc8 = Preferences.Get("sal8", "");
            //var textc9 = Preferences.Get("sal9", "");


            //Combothr.Add(textc7);
            //Combothr.Add(textc8);
            //Combothr.Add(textc9);

            var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));

            cmbbox1.ItemsSource = db.Query<Options>("SELECT * FROM OPTIONS WHERE COMBO = 'COMBO SALIDA 1'");
            //cmbbox1.ItemDisplayBinding. = Options.OptionDesc;
            cmbbox2.ItemsSource = db.Query<Options>("SELECT * FROM OPTIONS WHERE COMBO = 'COMBO SALIDA 2'");
            //cmbbox2.ItemDisplayBinding = Options.OptionDesc;
            cmbbox3.ItemsSource = db.Query<Options>("SELECT * FROM OPTIONS WHERE COMBO = 'COMBO SALIDA 3'");
            //cmbbox3.ItemDisplayBinding = Options.OptionDesc;
        }

        private async void Commentbtn_Clicked(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(lbl1.Text) && string.IsNullOrEmpty(lbl2.Text) && string.IsNullOrEmpty(lbl3.Text))
            //{
            //    await DisplayAlert("Info", "Para ver los campos debe de llenarlos en configuracion", "OK");

            //}
            //else
            //{
            desc.IsEnabled = true;
            Commentbtn.IsEnabled = false; 
            lblcm.IsVisible = true;
            frame4.IsVisible = true;
            frame5.IsVisible = true;
            //chx1.IsVisible = true;
            //chx2.IsVisible = true;
            //chx3.IsVisible = true;
            //chx4.IsVisible = true;
            //chx5.IsVisible = true;
            //if (!string.IsNullOrEmpty(lbl1.Text))
            //{
            //    lbl1.IsVisible = true;
            //    fcombo1.IsVisible = true;
            //    //Campos.Text = "Desactivar";
            //}

            //if (!string.IsNullOrEmpty(lbl2.Text))
            //{
            //    lbl2.IsVisible = true;
            //    fcombo2.IsVisible = true;
            //    //Campos.Text = "Desactivar";
            //}

            //if (!string.IsNullOrEmpty(lbl3.Text))
            //{
            //    lbl3.IsVisible = true;
            //    fcombo3.IsVisible = true;
            //    //Campos.Text = "Desactivar";
            //}
            //var list1 = Comboone.Where(elem => elem != "" && elem != null).ToList();
            if (cmbbox1.ItemsSource.Count > 0)
                {
                    lbl4.IsVisible = true;
                    frame1.IsVisible = true;
                    // Campos.Text = "Desactivar";
                }
                //var list2 = Comboscn.Where(elem => elem != "" && elem != null).ToList();
               if (cmbbox2.ItemsSource.Count > 0)
                {
                    lbl5.IsVisible = true;
                    frame2.IsVisible = true;
                    //Campos.Text = "Desactivar";
                }
                //var list3 = Combothr.Where(elem => elem != "" && elem != null).ToList();
                if (cmbbox3.ItemsSource.Count > 0)
                {
                    lbl6.IsVisible = true;
                    frame3.IsVisible = true;
                    //Campos.Text = "Desactivar";
                }
                
           // }
        }

        private void cmbbox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comment = cmbbox1.Items[cmbbox1.SelectedIndex]; 
        }

        private void cmbbox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comment2 = cmbbox2.Items[cmbbox2.SelectedIndex];
        }

        private void cmbbox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comment3 = cmbbox3.Items[cmbbox3.SelectedIndex];
        }

        //Este metodo sirve para dar salida a una cedula que ya esta dentro
        [Obsolete]
        private async void BtnSalida_Clicked(object sender, EventArgs e)
        {
            comment4 = comentariotxt.Text;
            comment5 = comentariotxt2.Text;
            
           
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));                
                var querry = "SELECT * FROM Invitados WHERE CARGO = '" + cedula + "' AND FECHA_SALIDA is null";
                var registroInv = db.Query<Invitados>(querry);
               // var Request = db.Query<COMPANIAS>($"select * from companias where compania_id = {registroInv.First().Compania_ID}");
                registroInv.First().Fecha_Salida = DateTime.Now;
                registroInv.First().salidaSubida = null;

                //This is a test//
                registroInv.First().Comentario = comment;
                registroInv.First().SALIDA1 = comment2;
                registroInv.First().SALIDA2 = comment3;
                registroInv.First().SALIDA3 = comment4;
                registroInv.First().SALIDA4 = comment5;
                
                

                //await PopupNavigation.PushAsync(new VisitInfo(registroInv.First().Nombres, registroInv.First().Cargo, Request.First().NOMBRE, registroInv.First().Fecha_Registro.ToString(), registroInv.First().Fecha_Salida.ToString()));
                db.UpdateAll(registroInv);                
                DependencyService.Get<IToastMessage>().DisplayMessage("Se ha dado salida correctamente.");
                await Navigation.PopToRootAsync();
                
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
