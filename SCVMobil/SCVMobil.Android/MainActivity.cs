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
using Java.Lang;
using System.Timers;

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
                       
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            showCurrentTime = FindViewById<TextView>(Resource.Id.toolbar);
            setCurrentTime();
            
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

        [Obsolete]
        private void setCurrentTime()
        {

            
            var query = fireBirdData.hora();
            var query2 = fireBirdData.min();
            if (query.Count < 0 || query2.Count < 0 || query.Count == 0 || query2.Count == 0)
            {
                Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                AlertDialog alert = dialog.Create();
                alert.SetTitle("Mensaje");
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
                    foreach (var item2 in query2)
                    {
                        var tiempo = DateTime.Now.ToString("HH:mm:ss");
                        var src = DateTime.Now;
                        var hm = new DateTime(src.Year, src.Month, src.Day, src.Hour, src.Minute, 0);
                        if (item.fecha == Convert.ToString(hm.Hour) && item2.minuto == Convert.ToString(hm.Minute))
                        {
                            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                            AlertDialog alert = dialog.Create();
                            alert.SetTitle("Conexion:");
                            alert.SetMessage("Conectado, hora: " + item.fecha + ":" + item2.minuto);
                            alert.SetButton("OK", (c, ev) =>
                            {

                            });
                            alert.Show();
                           
                        }
                        else
                        {
                            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                            AlertDialog alert = dialog.Create();
                            alert.SetTitle("Mensaje:");
                            alert.SetMessage("Arregle la hora de su dispositivo");
                            alert.SetButton("OK", (c, ev) =>
                            {

                                Xamarin.Forms.Forms.Context.StartActivity(new Android.Content.Intent(Android.Provider.Settings.ActionSettings));
                                JavaSystem.Exit(0);

                            });
                            alert.Show();
                            break;
                        }
                    }
                    
                    
                }

            }
        }




        
    }
}