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
                //lbl4.IsVisible = true;
            }
            else
            {
                //lbl4.IsVisible = false;

            }

            if (p2 == true)
            {
                lbl5.Text = Preferences.Get("title2salida", "");
                //lbl5.IsVisible = true;
            }
            else
            {
                //lbl5.IsVisible = false;
            }

            if (p3 == true)
            {
                lbl6.Text = Preferences.Get("title3salida", "");
                //lbl6.IsVisible = true;
            }
            else
            {
                //lbl6.IsVisible = false;

            }

            if (p4 == true)
            {
                lbl7.Text = Preferences.Get("title4salida", "");
                //lbl7.IsVisible = true;
            }
            else
            {
                //lbl7.IsVisible = false;

            }

            if (p5 == true)
            {
                lbl8.Text = Preferences.Get("title5salida", "");
                //lbl8.IsVisible = true;
            }
            else
            {
                //lbl8.IsVisible = false;

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
                //lbl4.IsVisible = true;
            }
            else
            {
                lbl4.IsVisible = false;

            }

            if (p2 == true)
            {
                lbl4.Text = Preferences.Get("title2salida", "");
                //lbl4.IsVisible = true;
            }
            else
            {
                lbl4.IsVisible = false;
            }

            if (p3 == true)
            {
                lbl6.Text = Preferences.Get("title3salida", "");
                //lbl6.IsVisible = true;
            }
            else
            {
                lbl4.IsVisible = false;

            }

            if (p4 == true)
            {
                lbl7.Text = Preferences.Get("title4salida", "");
                ///lbl7.IsVisible = true;
            }
            else
            {
                //lbl7.IsVisible = false;

            }

            if (p5 == true)
            {
                lbl8.Text = Preferences.Get("title5salida", "");
                //lbl8.IsVisible = true;
            }
            else
            {
                //lbl8.IsVisible = false;

            }
 

            GetCombos();

        }

       

        private void desc_Clicked(object sender, EventArgs e)
        {
            desc.IsEnabled = false;
            Commentbtn.IsEnabled = true;
        
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
            if (lbl7.IsVisible == true)
            {
                lbl7.IsVisible = false;
                frame4.IsVisible = false;
            }

            if (lbl8.IsVisible == true)
            {
                lbl8.IsVisible = false;
                frame4.IsVisible = false;
            }
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

       

        public void GetCombos()
        {
         

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
       
            desc.IsEnabled = true;
            Commentbtn.IsEnabled = false; 
         
           
           
            var p1 = Preferences.Get("texto1", true);
            var p2 = Preferences.Get("texto2", true);
            var p3 = Preferences.Get("texto3", true);
            var p4 = Preferences.Get("texto4", true);
            var p5 = Preferences.Get("texto5", true);
            var p6 = Preferences.Get("texto6", true);
            
            if (!string.IsNullOrEmpty(lbl7.Text) && p4 == true)
            {
                lbl7.IsVisible = true;
                frame4.IsVisible = true;
                //lbl4.Text = Preferences.Get("title4salida", "");
            }

            if (!string.IsNullOrEmpty(lbl8.Text) && p5 == true)
            {
                lbl8.IsVisible = true;
                frame5.IsVisible = true;
                //lbl4.Text = Preferences.Get("title5salida", "");

            }


            if (cmbbox1.ItemsSource.Count > 0 && p1 == true)
                {
                    lbl4.IsVisible = true;
                    frame1.IsVisible = true;
                //lbl4.Text = Preferences.Get("title1salida", "");
                // Campos.Text = "Desactivar";
            }
                //var list2 = Comboscn.Where(elem => elem != "" && elem != null).ToList();
                if (cmbbox2.ItemsSource.Count > 0 && p2 == true)
                {
                    lbl5.IsVisible = true;
                    frame2.IsVisible = true;
                //lbl5.Text = Preferences.Get("title2salida", "");
                //Campos.Text = "Desactivar";
            }
                //var list3 = Combothr.Where(elem => elem != "" && elem != null).ToList();
                if (cmbbox3.ItemsSource.Count > 0 && p3 == true)
                {
                    lbl6.IsVisible = true;
                    frame3.IsVisible = true;
               // lbl6.Text = Preferences.Get("title3salida", "");
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
