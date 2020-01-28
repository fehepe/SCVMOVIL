using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Net.Http;
using SQLite;
using Plugin.Toasts;
using System.Globalization;
using System.Threading;
using System.Timers;
using FirebirdSql.Data.FirebirdClient;
using Rg.Plugins.Popup.Services;
using System.Net.NetworkInformation;
using SCVMobil.Models;
using SCVMobil.Connections;
using Microsoft.AppCenter.Analytics;

namespace SCVMobil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public class numCedulas
    {
        public int countCedulas { get; set; }
    }

    public partial class SettingsPage : ContentPage
    {
        // This handles the Web data request
        private HttpClient _client = new HttpClient();
        private HttpClient _client2 = new HttpClient();
        private FireBirdData fireBird  = new FireBirdData();

        public SettingsPage()
        {
            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            setDefaults();
            DeviceDisplay.KeepScreenOn = true;
            eServerIP.Text = Preferences.Get("SERVER_IP", "192.168.1.170");            
            eLector.Text = Preferences.Get("LECTOR", "1");
            entChunkSize.Text = Preferences.Get("CHUNK_SIZE", "50000");
            lbVersion.Text = "Ver: " + Preferences.Get("VERSION", "3.0");
            var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));

            string totalCed = db.ExecuteScalar<string>("select count(*) from padron");
            db.Dispose();
            lblCantidadPadron.Text = $"Cantidad de cedulas guardadas: "+ totalCed;
            swAutoSync.IsToggled = Preferences.Get("AUTO_SYNC", "False") == "True";
            eCommitSize.Text = Preferences.Get("COMMIT_SIZE", "1000000");
           
            //Preferences.Get("LANG_SELECTED", pickerType.SelectedItem.ToString());
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DeviceDisplay.KeepScreenOn = false;

        }

        private void setDefaults()
        {
            if(Preferences.Get("SERVER_IP", "N/A") == "N/A")
            {
                Preferences.Set("SERVER_IP", "192.168.1.103");//
            }
        }

       

        

        private async void BtSync_Clicked(object sender, EventArgs e)
        {
            string Password = "cr52401";
            string TryPassword = await DisplayPromptAsync("verificacion", "Ingrese la contraseña maestra para continuar", "Aceptar", "Cancelar", "Contraseña", 11, null);
            if (Password == TryPassword)
            {
            
                Debug.WriteLine("Timer Stopped");
                App.syncTimer.Stop();
                App.syncTimer.Enabled = false;
                        var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                Preferences.Set("IS_SYNC", "true");
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    try
                    {
                        //Mostramos el popup
                        popupLoadingView.IsVisible = true;

                        pbLoading.IsVisible = true;
                        aiLoading.IsVisible = false;
                        aiLoading.IsRunning = false;
                        lblLoadingText.Text = "Loading...";

                        var querry1 = "";

                        //Vamos a buscar el directorio de la base de datos

                        //Buscamos el Chunk Size
                        var iChunkSize = int.Parse(Preferences.Get("CHUNK_SIZE", "10000"));

                        //Vamos a poner el timeout del http en 20 min seg
                        _client.Timeout = new TimeSpan(0, 40, 0);

                        Debug.WriteLine("Setting timeout to 20 min");

                        string querry = "";
                        string connectionString = fireBird.connectionString(false);


                        FbCommand cmd;
                        //var Url = "http://" + Preferences.Get("SERVER_IP", "localhost") + ":" + Preferences.Get("SERVER_PORT", "4444")
                        //+ "/?sql=";

                        //TODO: Buscar cuantas cedulas se van a descargar.

                        querry = "select count(p.cedula) as anyCount from padron p";
                        int tries = 0;
                        //string contenido;
                        int maxRegistro = 0;
                        while (true)
                        {
                            try
                            {
                                FbConnection fb = new FbConnection(connectionString);

                                fb.Open();
                                FbCommand command = new FbCommand(
                                    querry,
                                    fb);


                                FbDataReader reader = command.ExecuteReader();

                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        maxRegistro = Convert.ToInt32(reader[0]);
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("No rows found.");
                                }
                                reader.Close();

                                fb.Close();
                                fb.Dispose();

                                break;
                            }
                            catch (Exception ex)
                            {
                                Analytics.TrackEvent("Error al conectarse a base de datos " + ex.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
                                Debug.WriteLine("Error en Sync");
                                if (tries >= 5)
                                {

                                    throw new Exception("No se pudo conectar con la base de datos.");
                                }
                                var x = ex.Message;
                                tries += 1;
                            }
                        }

                        //var maxRegistroList = JsonConvert.DeserializeObject<List<counterObj>>(contenido);
                        //var maxRegistro = maxRegistroList.First();

                        //Descargar CEDULAS

                        querry = "select p.cedula, p.nombres, p.apellido1, p.apellido2 from padron p";

                        if (maxRegistro > 0)
                        {
                            int loadedRegistros = 1;
                            int commitedRegistros = 0;
                            double progress = 0.0;
                            var listPadron = new List<PADRON>();
                            while (loadedRegistros < maxRegistro)
                            {
                                progress = (double)loadedRegistros / maxRegistro;
                                if (maxRegistro - loadedRegistros < iChunkSize - 1)
                                {
                                    querry1 = querry1 = querry + " ROWS "
                                         + loadedRegistros.ToString() + " TO " + maxRegistro.ToString();
                                    progress = 1.0;
                                }
                                else
                                {
                                    querry1 = querry + " ROWS "
                                         + loadedRegistros.ToString() + " TO " + (loadedRegistros + iChunkSize - 1).ToString();
                                }

                                await Task.Factory.StartNew(() =>
                                {
                                    listPadron.AddRange(fireBird.DownloadPadron(querry1,false));

                                    //var listPadronTemp = JsonConvert.DeserializeObject<List<PADRON>>(contenidoTBL);
                                    //listPadron = listPadron.Concat(listPadronTemp).ToList();
                                    if (listPadron.Count >= int.Parse(Preferences.Get("COMMIT_SIZE", "1000000")) || maxRegistro - loadedRegistros < iChunkSize - 1)
                                    {
                                        try
                                        {
                                            Device.BeginInvokeOnMainThread(() =>
                                            {
                                                lblLoadingText.Text = "Commited " + commitedRegistros.ToString() + "/" + maxRegistro.ToString() + " CEDULAS";
                                            });
                                            Debug.WriteLine("Se van a insertar: " + listPadron.Count.ToString() + " En la BBDD Local");
                                        //await Task.Factory.StartNew(() =>
                                        //{
                                        db.InsertAll(listPadron);
                                        //});
                                        commitedRegistros = commitedRegistros + listPadron.Count;
                                            //Preferences.Set("TOTAL_CEDULAS_DESCARGADAS", commitedRegistros.ToString());
                                            Device.BeginInvokeOnMainThread(() =>
                                            {
                                                //lblCantidadPadron.Text = $"Cantidad de cedulas descargadas: {Preferences.Get("TOTAL_CEDULAS_DESCARGADAS", commitedRegistros.ToString())}";
                                                Debug.WriteLine($"Cantidad de cedulas descargadas: {Preferences.Get("TOTAL_CEDULAS_DESCARGADAS", commitedRegistros.ToString())}");
                                            });
                                            listPadron.Clear();
                                        }
                                        catch (Exception ea)
                                        {
                                            Debug.WriteLine("Se ha encontrado una excepcion, Error: " + ea.Message);
                                        }
                                    }

                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        lblLoadingText.Text = "Downloading CEDULAS: " + loadedRegistros.ToString() + "/" + maxRegistro.ToString();
                                    });
                                    Debug.WriteLine("Downloading CEDULAS: " + loadedRegistros.ToString() + "/" + maxRegistro.ToString());
                                    loadedRegistros = loadedRegistros + iChunkSize;
                                });

                                await pbLoading.ProgressTo(progress, 200, Easing.Linear);

                            }

                            pbLoading.Progress = 0.0;
                            lblLoadingText.Text = "Guardando Padron";
                            pbLoading.IsVisible = false;
                            aiLoading.IsVisible = true;
                            aiLoading.IsRunning = true;
                            Debug.WriteLine("Se van a insertar: " + listPadron.Count.ToString() + " En la BBDD Local");
                            await Task.Factory.StartNew(() =>
                            {
                                db.InsertAll(listPadron);
                            });
                            pbLoading.IsVisible = true;
                            aiLoading.IsVisible = false;
                            aiLoading.IsRunning = false;
                            lblLoadingText.Text = "Loading...";

                            //Si hay Cedulas para cargar entramos

                        }
                    }
                    catch (Exception ey)
                    {
                        Debug.WriteLine("Error syncronisando: " + ey.Message);
                        popupLoadingView.IsVisible = false;
                        try
                        {
                            var options = new NotificationOptions()
                            {
                                Title = "Error syncronisando",
                                Description = "Hubo un error en la syncronización, intente nuevamente",
                                IsClickable = false // Set to true if you want the result Clicked to come back (if the user clicks it)
                            };
                            var notification = DependencyService.Get<IToastNotificator>();
                            var result = notification.Notify(options);
                        }
                        catch (Exception eo)
                        {
                            Debug.WriteLine("Error en notificacion: " + eo.Message);
                        }

                    }
                    finally
                    {
                        db.Dispose();

                        popupLoadingView.IsVisible = false;
                        try
                        {
                            _client.Timeout = new TimeSpan(0, 0, 100);
                        }
                        catch (Exception eu)
                        {
                            Debug.WriteLine("Error devolviendo el time out: " + eu.Message);
                        }
                        Debug.WriteLine("Setting timeout to 100 seconds");
                        Debug.WriteLine("Timer Start");
                        //Preferences.Set("IS_SYNC", "false");
                        App.syncTimer.Enabled = true;
                        App.syncTimer.Start();
                    }
                    //"select b.boleta_id, b.boleta, b.fecha_lectura, null as SUBIDA from boletas b where b.fecha_lectura is NULL"
                }
                else
                {
                    //Si no hay connecion a internet avisar al usuario
                    Debug.WriteLine("NO NETWORK");
                    popupLoadingView.IsVisible = false;
                    var options = new NotificationOptions()
                    {
                        Title = "No hay conexión",
                        Description = "No se puede sincronizar sin una conexión a la red",
                        IsClickable = false // Set to true if you want the result Clicked to come back (if the user clicks it)
                    };
                    var notification = DependencyService.Get<IToastNotificator>();
                    var result = notification.Notify(options);
                    Debug.WriteLine("Timer Start");
                    Preferences.Set("IS_SYNC", "false");
                    App.syncTimer.Enabled = true;
                    App.syncTimer.Start();
                }
            }
            else
            {
                await DisplayAlert("Credencial incorrecta", "La credencial ingresada no es valida", "OK");
            }

        }

        private async void BtIni_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                    //db.Execute("DELETE FROM PADRON");
                    db.Execute("DELETE FROM COMPANIAS");
                    db.Execute("DELETE FROM PERSONAS");
                    db.Execute("DELETE FROM SalidaOffline");
                    db.Execute("DELETE FROM Invitados");
                    db.Execute("DELETE FROM InvitadosReservas");
                    db.Execute("DELETE FROM PLACA");
                    Preferences.Set("MAX_INVIDATO_ID", "0");
                    Preferences.Set("MAX_SALIDA_ID", "0");
                    Preferences.Set("MAX_PERSONA_ID", "0");
                    Preferences.Set("MAX_RESERVA_ID", "0");
                    Preferences.Set("MAX_COMPANIA_ID", "0");
                    Debug.WriteLine("DONE INI");
                    var options = new NotificationOptions()
                    {
                        Title = "Inicialización completa!",
                        Description = "La base de datos fue inicializada correctamente",
                        IsClickable = false // Set to true if you want the result Clicked to come back (if the user clicks it)
                    };
                    var notification = DependencyService.Get<IToastNotificator>();
                    var result = notification.Notify(options);
                    db.Close();
                    db.Dispose();
                });
            }
            catch(Exception ea)
            {
                Debug.WriteLine("Excepcion encontrada en el evento BtIni_Clicked: " + ea.Message);
                Analytics.TrackEvent("Error al inicializar  " + ea.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            }
        }     

        private async void Guardar_Clicked(object sender, EventArgs e) //Boton para guardar configuracion//
        {

            try
            {
                Preferences.Set("SERVER_IP", eServerIP.Text);

                Preferences.Set("COMMIT_SIZE", eCommitSize.Text);
                var x = Preferences.Get("COMMIT_SIZE", "1000000");
                Preferences.Set("AUTO_SYNC", swAutoSync.ToString());
                Preferences.Set("LECTOR", eLector.Text);
                Preferences.Set("CHUNK_SIZE", entChunkSize.Text);
                if (eLector.Text == "338")
                {
                    Preferences.Set("DEV", "True");
                }
                else
                {
                    Preferences.Set("DEV", "False");
                }
                await DisplayAlert("", "Datos guardados exitosamente", "continuar");
                await Navigation.PopToRootAsync();
                //await PopupNavigation.PushAsync(new PopUpGuardarConfig()); //PopUp para guardar la configuracion//
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en el metodo guardar: " + ea.Message);
               
                Analytics.TrackEvent("Error al guardar configuracion:  " + ea.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            }            

        }
        

        private async void pinbutton_Clicked(object sender, EventArgs e)
        {
            try
            {              
                Ping Pings = new Ping();
                int timeout = 50;

                if (Pings.Send(eServerIP.Text, timeout).Status == IPStatus.Success)
                {

                    try
                    {
                        string connectionString = fireBird.connectionString(true);

                        FbConnection fb = new FbConnection(connectionString);
                        fb.Open();


                        fb.Close();
                        Preferences.Set("SERVER_IP", eServerIP.Text);
                       

                        await DisplayAlert("Conectado","Se ha conectado","ok");
                        //await PopupNavigation.PushAsync(new PopUpPing()); //popup conexion con exito//
                        Debug.WriteLine("Ping exitoso a la ip: "+ eServerIP.Text);
                    }
                    catch (Exception ea)
                    {
                        Debug.WriteLine("Error en el metodo pingbtn" + ea.Message);
                        Analytics.TrackEvent("Exception al hacer ping:  " + ea.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
                        //await PopupNavigation.PushAsync(new PopUpPingIncorrecto()); //popup conexion erronea//
                    }
                }
                else
                {
                    Preferences.Set("SYNC_VSU",false);
                   
                    await DisplayAlert("Error", "No se pudo establecer conexion", "ok");
                    //await PopupNavigation.PushAsync(new PopUpPingIncorrecto());//popup conexion erronea//
                }

            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en el metodo pingbtn " + ea.Message);
                Analytics.TrackEvent("Exception al hacer ping:  " + ea.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            }

        }

        private void btnPadron_Clicked(object sender, EventArgs e)
        {
            ActualizarPadron();
        }

        private async void ActualizarPadron()
        {
            try
            {
                

                FireBirdData fireBirdData = new FireBirdData();
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
               
                string querry = "SELECT PV.documento AS CEDULA, PV.nombres, PV.apellidos,pv.padron_visitante_id " +
                                "FROM padron_visitantes PV " +
                                "WHERE CHAR_LENGTH(PV.documento) = 11 AND pv.padron_visitante_id > " + Preferences.Get("MAX_PERSONA_PADRON_ID", "0") +
                                " ORDER BY pv.padron_visitante_id desc ";
               
               
                var listPadronVisita = fireBirdData.DownloadPadron(querry, true);
                if (listPadronVisita.Count > 0)
                {
                   
                    db.InsertAll(listPadronVisita);
                    Preferences.Set("MAX_PERSONA_PADRON_ID", listPadronVisita.First().ID_PADRON.ToString());
                    

                    
                }
                OnAppearing();

            }
            catch (Exception ea)
            {
                await DisplayAlert("Error",ea.Message,"OK");
                Debug.WriteLine("Error en el metodo ActualizarPadron " + ea.Message);
                Analytics.TrackEvent("Exception al hacer ActualizarPadron:  " + ea.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));

            }


        }
    }
}