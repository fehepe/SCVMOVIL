﻿using System;
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
using Plugin.DeviceInfo;
using SCVMobil.Connections;
using System.Security.Cryptography;
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

                        var query = fireBird.hora();
                        
                        var query2 = fireBird.min();

                        foreach (var item in query)
                        {
                            foreach (var item2 in query2) //Evaluar la hora//
                            {
                                var tiempo = DateTime.Now.ToString("HH:mm:ss");
                                var src = DateTime.Now;
                                var hm = new DateTime(src.Year, src.Month, src.Day, src.Hour, src.Minute, 0);
                                if (item.fecha == Convert.ToString(hm.Hour) && item2.minuto == Convert.ToString(hm.Minute))
                                {
                                    break;
                                }
                                else
                                {
                                    Preferences.Set("SYNC_VSU", false);
                                    Current.MainPage.DisplayAlert("Error", "Hora incorrecta", "ok");                
                                    break;
                                }
                            }


                        }
                        ////implementar el metodo tryConnection();


                        fireBird.tryConnection();

                        //Implementar servicios Periodicos.
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
                        fireBird.DownloadGuests();

                        // Descargar las salidas.
                        fireBird.DownloadOuts();
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Error", "No tiene acceso a internet", "ok");
                        Preferences.Set("SYNC_VSU", false);
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

            bool isSet = Preferences.Get("IS_SET", false);

            if (isSet == false)
            {
                MainPage = new NavigationPage(new LicensePage());

            }
            else if (isSet == true)
            {
                MainPage = new NavigationPage(new MainPage());
            }
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