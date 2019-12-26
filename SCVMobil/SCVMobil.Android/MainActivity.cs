using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Content.Res;
using Xamarin.Forms;
using SCVMobil.Connections;

namespace SCVMobil.Droid
{
    [Activity(Label = "SCVMobil", Icon = "@drawable/visitor", Theme = "@style/MyTheme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]


    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        FireBirdData fireBirdData = new FireBirdData();
        private TextView showCurrentTime;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.MainTheme);
            showCurrentTime = FindViewById<TextView>(Resource.Id.time);
            setCurrentTime();            
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#D32F2F")); //Cambio de color del status bar//


            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void setCurrentTime()
        {

            //string time = string.Format("{0}",
            //DateTime.Now.ToString("HH:mm").PadLeft(2, '0'));
            //showCurrentTime.Text = time;
            var query = fireBirdData.Timee();
            if (query.Count < 0)
            {
                Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                AlertDialog alert = dialog.Create();
                alert.SetTitle("Title");
                alert.SetMessage("No se pudo conectar a la base de datos");
                alert.SetButton("OK", (c, ev) =>
                {

                });
                alert.Show();
            }
            else
            {
                foreach (var item in query)
                {
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Title");
                    alert.SetMessage("Se conecto, hora: " + item.fecha);
                    alert.SetButton("OK", (c, ev) =>
                    {
                       

                    });
                    alert.Show();
                }

            }
        }




        
    }
}