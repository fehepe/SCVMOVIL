using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.IO;
using Xamarin.Essentials;
using Plugin.Toasts;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Globalization;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using SCVMobil.Models;
using SCVMobil.Connections;

namespace SCVMobil
{

    public class counterObj
    {
        public int anyCount { get; set; }
    }

    public partial class App : Application
    {
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

       public async static void OnTimedEvent(object sender, ElapsedEventArgs e)
       {
            
            syncTimer.Enabled = false;
            Debug.WriteLine("New Timer Running");
            HttpClient _client = new HttpClient();
            _client.Timeout = new TimeSpan(0, 0, 100);
            await Task.Factory.StartNew(() => //Crear un nuevo thread!
            {
                try
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        var fireBird = new FireBirdData();

                        //implementar el metodo tryConnection();
                       
                       // fireBird.tryConnection();
   
                        // Implementar servicios Periodicos.
                        fireBird.PublicServices();

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

                        // Descargar las reservas.
                        fireBird.DownloadReservations();

                        // Descargar las companies.
                        fireBird.DownloadCompanies();

                        // Descargar las personas(destinos).
                        fireBird.DownloadPeople_Destination();

                        // Descargar los Invitados.

                        /**string querryDownInv = "SELECT FIRST " + Preferences.Get("CHUNK_SIZE", "10000") + " INVIDATO_ID, 1 as SUBIDA, IIF(FECHA_SALIDA is null, 0,1) " +
                        //"as SALIDASUBIDA, COMPANIA_ID, NOMBRES, APELLIDOS, FECHA_REGISTRO, FECHA_SALIDA, TIPO, CARGO, TIENE_ACTIVO, ESTATUS_ID, MODULO, EMPRESA_ID, " +
                        //"PLACA, TIPO_VISITANTE, ES_GRUPO, GRUPO_ID, PUERTA_ENTRADA, ACTUALIZADA_LA_SALIDA, HORAS_CADUCIDAD, PERSONAS, IN_OUT, ORIGEN_ENTRADA, " +
                        //"ORIGEN_SALIDA, COMENTARIO, ORIGEN_IO, ACTUALIZADO, CPOST, TEXTO1_ENTRADA, TEXTO2_ENTRADA, TEXTO3_ENTRADA, SECUENCIA_DIA, NO_APLICA_INDUCCION, " +
                        //"VISITADO, COALESCE(LECTOR, 0) AS LECTOR FROM INVITADOS WHERE INVIDATO_ID > " + Preferences.Get("MAX_INVIDATO_ID", "0") +
                        //" AND COALESCE(LECTOR, 0) <> " + Preferences.Get("LECTOR", "1") + " ORDER BY INVIDATO_ID DESC";**/
                        fireBird.DownloadGuests();

                        // Descargar las salidas.
                        fireBird.DownloadOuts();
                    }
                }
                catch (Exception ey)
                {
                    Preferences.Set("SYNC_VSU", false);

                    Debug.WriteLine("Exeption in timer: " + ey.ToString());
                }
                finally
                {
                    if(Preferences.Get("IS_SYNC", "false") == "false")
                    {
                        syncTimer.Enabled = true;
                    }
                }

            });
        }

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage())
            {
                //Aqui se puede cambiar el color de la barra de navegacion.
                //BarBackgroundColor = Color.FromHex("#0000FF"),
                //BarTextColor = Color.White
            };
        }

        protected override void OnStart()
        {
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

            //db.DropTable<PADRON>();
            //db.DropTable<COMPANIAS>();
            //db.DropTable<PERSONAS>();
            //db.DropTable<VW_RESERVA_VISITA>();
            //db.DropTable<Invitados>();
            //Preferences.Set("MAX_INVIDATO_ID","0");
            //Preferences.Set("MAX_SALIDA_ID", "0");
            //Creamos las tablas necesarias.
            //db.CreateTable<Puertas>();
            db.CreateTable<PADRON>();
            db.CreateTable<COMPANIAS>();
            db.CreateTable<PERSONAS>();
            db.CreateTable<VW_RESERVA_VISITA>();
            db.CreateTable<Invitados>();
            db.CreateTable<InvitadosReservas>();
            db.CreateTable<SalidaOffline>();
            db.CreateTable<PLACA>(); 

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
            Preferences.Set("PAGE_ACTIVE", "NONE");
        }



    }
}