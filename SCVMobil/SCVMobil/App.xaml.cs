using System;
using Xamarin.Forms;
using SQLite;
using System.IO;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;
using System.Net.Http;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using SCVMobil.Models;
using SCVMobil.Connections;
using SCVMobil.Views;

namespace SCVMobil
{

    public partial class App : Application
    {
        const int counter = 130;
        VerifyHash verification = new VerifyHash(counter);
        public static System.Timers.Timer syncTimer;
        //private static object preferences;

        private static void SetTimer()
        {
            // Crear un timer con un intervalo de 2 segundos
            syncTimer = new System.Timers.Timer(2000);
            // Asignar el evento.
            
            syncTimer.Elapsed += OnTimedEvent;
            syncTimer.AutoReset = true;
            syncTimer.Enabled = true;
            
            
        }
        public async void checkDateTime()
        {


            try
            {
                var fireBird = new FireBirdData(); //NEW//
                var src = DateTime.Now; //NEW//
                var fechactual = Convert.ToDateTime(fireBird.obtenerfecha());
                var fechamaxima = fechactual.AddMinutes(2);
                var fechaminima = fechactual.AddMinutes(-2);
                Preferences.Set("MINUTOMINIMO", fechaminima);
                int i = DateTime.Compare(src, fechamaxima);
                int k = DateTime.Compare(src, fechaminima);

                if (Convert.ToInt32(src) <= i && Convert.ToInt32(src) >= k)
                {
                    Preferences.Set("nowifi", false);
                    Preferences.Set("ENTCEDULA", true);
                    Preferences.Set("wifi", true);
                    Preferences.Set("aviso", false);
                    Preferences.Set("CONFIG", true);
                }
                else
                {
                    await MainPage.DisplayAlert("Error", "Fecha Incorrecta", "ok");
                    await MainPage.DisplayAlert("Mensaje", "La fecha actual es: " + fechactual, "ok");
                    Preferences.Set("nowifi", true);
                    Preferences.Set("ENTCEDULA", false);
                    Preferences.Set("wifi", false);
                    Preferences.Set("aviso", true);
                    Preferences.Set("CONFIG", false);
                }
            }
            catch (Exception)
            {


            }

        }


        public async static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {

            
            syncTimer.Enabled = false;
            Debug.WriteLine("New Timer Running");
            HttpClient _client = new HttpClient();
            _client.Timeout = new TimeSpan(0, 0, 100);
            var fireBird = new FireBirdData();
            
            await Task.Factory.StartNew(() => //Crear un nuevo thread!
            {
                try
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet) //
                    {
                        //checkDateTime();
                        fireBird.tryConnection();

                        //Implementar servicios Periodicos.
                        fireBird.PublicServices();

                        
                        // Descargar las reservas.
                        fireBird.DownloadReservations();

                        // Descargar las companies.
                        fireBird.DownloadCompanies();

                        // Descargar Verificaciones
                       // fireBird.DownloadVerifications();

                        // Descargar las personas(destinos).
                        fireBird.DownloadPeople_Destination();

                        // Descargar los Invitados.
                        fireBird.DownloadGuests();

                        // Descargar las salidas.
                        fireBird.DownloadOuts();

                        // Descargar DEPTO_LOCALIDAD
                        fireBird.DownloadDeptoLocalidad();

                        // Subir visitantes.
                        fireBird.UploadVisits();

                        // Cargar Visitantes con reservas.
                        fireBird.UploadVisitsReservation();

                        // Subir las Verificacion.
                        fireBird.UploadVerifications();

                        // Subir las salidas.
                        fireBird.UploadOut();

                        // Subir las salidasDesconocidas.
                        fireBird.UploadUnknownOuts();


                    }
                    else
                    {                        
                        
                        Preferences.Set("SYNC_VSU", false); //
                    }
                    
                }
                catch (Exception ey)
                {
                    Preferences.Set("SYNC_VSU", false);
                    Debug.WriteLine("Error en la conexion de internet" + ey);

                    Debug.WriteLine("Exeption in timer: " + ey.ToString());
                }
                finally
                {
                   
                        syncTimer.Enabled = true;
                    
                }

            });
       }

        public App()
        {
            InitializeComponent();
        
            

            bool isSet = Preferences.Get("IS_SET", false);
            MainPage = new NavigationPage(new MainPage());
            //if (isSet == false)
            //{
            //    MainPage = new NavigationPage(new LicensePage());

            //}
            //else if (isSet == true)
            //{
                
            //}
        }

        

        protected override void OnStart()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet) //
            {
                //checkDateTime();
            }
                //Configuracion del App Center
                AppCenter.Start("android=364e9032-e9db-4d3a-a76f-c2095b3293d1;" +
                        "uwp={Your UWP App secret here};" +
                        "ios={Your iOS App secret here}",
                        typeof(Analytics), typeof(Crashes));
                Distribute.SetEnabledAsync(true);

                //Crear string para la base de datos
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dbSCV.db");

                //Guardamos el string de la base de datos en una preferencia.
                Preferences.Set("DB_PATH", dbPath);

                //Setiamos la Version
                Preferences.Set("VERSION", "1.28.5.19.1");

                //Conectar con la base de datos
                var db = new SQLiteConnection(dbPath);

               
                db.CreateTable<PADRON>();
                db.CreateTable<COMPANIAS>();
                db.CreateTable<COMPANIASLOC>();
                db.CreateTable<PERSONAS>();
                db.CreateTable<VW_RESERVA_VISITA>();
                db.CreateTable<Invitados>();
                db.CreateTable<InvitadosReservas>();
                db.CreateTable<SalidaOffline>();
                db.CreateTable<PLACA>();
                db.CreateTable<DEPTO_LOCALIDAD>();

                //Crear los indices de la tabla
                Task.Factory.StartNew(() =>
                {
                   
                    Debug.WriteLine("Creating indexes");
                    db.CreateIndex("PADRON", "CEDULA", true);
                    Debug.WriteLine("DONE! creating indexes");
                });
                ;
                //Iniciar servicio de subida en el background
                SetTimer();              
               
            

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
          
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            //Preferences.Set("PAGE_ACTIVE", "NONE");
            //Preferences.Set("PAGE_ACTIVE", "NONE");
            //checkDateTime();

        }
        
        

    }
}