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
using System.Net.NetworkInformation;
using Rg.Plugins.Popup.Services;
using FirebirdSql.Data.FirebirdClient;

namespace SCVMobil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public class numBoletas
    {
        public int countBoletas { get; set; }
    }

    public partial class SettingsPage : ContentPage
    {
        // This handles the Web data request
        private HttpClient _client = new HttpClient();
        private HttpClient _client2 = new HttpClient();
        private string Url = "";

        public SettingsPage()
        {
            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            setDefaults();
            eServerIP.Text = Preferences.Get("SERVER_IP", "192.168.1.103");
            registrosIP.Text = Preferences.Get("REGISTROS_IP", "192.168.1.103");
            eServerPort.Text = Preferences.Get("SERVER_PORT", "4441");
            port.Text = Preferences.Get("REGISTROS_PORT", "4440");
            eLector.Text = Preferences.Get("LECTOR", "1");
            entChunkSize.Text = Preferences.Get("CHUNK_SIZE", "50000");
            lbVersion.Text = "Ver: " + Preferences.Get("VERSION", "0.0.0.0.0");
            swAutoSync.IsToggled = Preferences.Get("AUTO_SYNC", "False") == "True";
            eCommitSize.Text = Preferences.Get("COMMIT_SIZE", "1000000");
           
            //Preferences.Get("LANG_SELECTED", pickerType.SelectedItem.ToString());



        }

        private void setDefaults()
        {
            if(Preferences.Get("SERVER_IP", "N/A") == "N/A")
            {
                Preferences.Set("SERVER_IP", "192.168.1.103");
            }
            if (Preferences.Get("REGISTROS_IP", "N/A") == "N/A")
            {
                Preferences.Set("REGISTROS_IP", "192.168.1.103");
            }
            if (Preferences.Get("SERVER_PORT", "N/A") == "N/A")
            {
                Preferences.Set("SERVER_PORT", "4441");
            }
            if (Preferences.Get("REGISTROS_PORT", "N/A") == "N/A")
            {
                Preferences.Set("REGISTROS_PORT", "4440");
            }

        }

       

        private void Lector_TextChanged(object sender, TextChangedEventArgs e)
        {
            Preferences.Set("LECTOR", e.NewTextValue);
            if (e.NewTextValue == "338")
            {
                Preferences.Set("DEV", "True");
            }
            else
            {
                Preferences.Set("DEV", "False");
            }
        }

        private async void BtSync_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Timer Stopped");
            App.syncTimer.Stop();
            App.syncTimer.Enabled = false;
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

                    var Url1 = "";

                    //Vamos a buscar el directorio de la base de datos
                    var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));

                    //Buscamos el Chunk Size
                    var iChunkSize = int.Parse(Preferences.Get("CHUNK_SIZE", "10000"));

                    //Vamos a poner el timeout del http en 20 min seg
                    _client.Timeout = new TimeSpan(0, 40, 0);

                    Debug.WriteLine("Setting timeout to 20 min");

                    string querry = "";

                    var Url = "http://" + Preferences.Get("SERVER_IP", "localhost") + ":" + Preferences.Get("SERVER_PORT", "4444")
                                    + "/?sql=";

                    //TODO: Buscar cuantas cedulas se van a descargar.

                    querry = "select count(p.cedula) as anyCount from padron p";
                    int tries = 0;
                    string contenido;
                    while (true)
                    {
                        try
                        {
                            contenido = await _client.GetStringAsync(Url + querry);
                            break;
                        }
                        catch
                        {
                            if (tries >= 5)
                            {
                                throw new Exception("No se pudo conectar con la base de datos.");
                            }
                            tries += 1;
                        } 
                    }
                    var maxRegistroList = JsonConvert.DeserializeObject<List<counterObj>>(contenido);
                    var maxRegistro = maxRegistroList.First();

                    //Descargar CEDULAS

                    querry = "select p.cedula, p.nombres, p.apellido1, p.apellido2 from padron p";

                    if (maxRegistro.anyCount > 0)
                    {
                        int loadedRegistros = 1;
                        int commitedRegistros = 0;
                        double progress = 0.0;
                        var listPadron = new List<PADRON>();
                        while (loadedRegistros < maxRegistro.anyCount)
                        {
                            progress = (double)loadedRegistros / maxRegistro.anyCount;
                            if (maxRegistro.anyCount - loadedRegistros < iChunkSize - 1)
                            {
                                Url1 = Url1 = Url + querry + " ROWS "
                                     + loadedRegistros.ToString() + " TO " + maxRegistro.anyCount.ToString();
                                progress = 1.0;
                            }
                            else
                            {
                                Url1 = Url + querry + " ROWS "
                                     + loadedRegistros.ToString() + " TO " + (loadedRegistros + iChunkSize - 1).ToString();
                            }
                            string contenidoTBL;
                            int triesTBL = 0;
                            while (true)
                            {
                                try
                                {
                                    contenidoTBL = await _client.GetStringAsync(Url1);
                                    break;
                                }
                                catch(Exception et)
                                {
                                    Debug.WriteLine("Error descargando padron: " + et.Message);
                                    if (triesTBL >= 5)
                                    {
                                        throw new Exception("No se pudo conectar con la base de datos: " + et.Message);
                                    }
                                    triesTBL += 1;
                                }
                            }
                            var listPadronTemp = JsonConvert.DeserializeObject<List<PADRON>>(contenidoTBL);
                            listPadron = listPadron.Concat(listPadronTemp).ToList();
                            if (listPadron.Count >= int.Parse(Preferences.Get("COMMIT_SIZE", "1000000")))
                            {
                                try
                                {
                                    lblLoadingText.Text = "Commited " + commitedRegistros.ToString() + "/" + maxRegistro.anyCount.ToString() + " BOLETAS";
                                    Debug.WriteLine("Se van a insertar: " + listPadron.Count.ToString() + " En la BBDD Local");
                                    await Task.Factory.StartNew(() =>
                                    {
                                        db.InsertAll(listPadron);
                                    });
                                    commitedRegistros = commitedRegistros + listPadron.Count;
                                    listPadron.Clear();
                                }
                                catch
                                {

                                }
                            }
                            lblLoadingText.Text = "Downloading BOLETAS: " + loadedRegistros.ToString() + "/" + maxRegistro.anyCount.ToString();
                            loadedRegistros = loadedRegistros + iChunkSize;
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

                        //Si hay boletas para cargar entramos

                    }
                }
                catch (Exception ey)
                {
                    Debug.WriteLine("Error syncronisando: " + ey);
                    popupLoadingView.IsVisible = false;
                    try
                    {
                        var options = new NotificationOptions()
                        {
                            Title = "Error syncronisando",
                            Description = "Ubo un error en la syncronización, intente nuevamente",
                            IsClickable = false // Set to true if you want the result Clicked to come back (if the user clicks it)
                        };
                        var notification = DependencyService.Get<IToastNotificator>();
                        var result = notification.Notify(options);
                    }
                    catch(Exception eo)
                    {
                        Debug.WriteLine("Error en notificacion: " + eo.Message);
                    }
                    
                }
                finally
                {

                    popupLoadingView.IsVisible = false;
                    try
                    {
                        _client.Timeout = new TimeSpan(0, 0, 100);
                    }
                    catch(Exception eu)
                    {
                        Debug.WriteLine("Error devolviendo el time out: " + eu.Message);
                    }
                    Debug.WriteLine("Setting timeout to 100 seconds");
                    Debug.WriteLine("Timer Start");
                    Preferences.Set("IS_SYNC", "false");
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
                });
            }
            catch
            {
                //TODO: Handle exeption
            }
        }     

        private async void Guardar_Clicked(object sender, EventArgs e)
        { 
            Preferences.Set("SERVER_PORT", eServerPort.Text);
            Preferences.Set("SERVER_IP", eServerIP.Text);
            Preferences.Set("REGISTROS_PORT", port.Text);
            Preferences.Set("REGISTROS_IP", registrosIP.Text);
            Preferences.Set("COMMIT_SIZE", eCommitSize.Text);
            var x = Preferences.Get("COMMIT_SIZE", "1000000");
            Preferences.Set("AUTO_SYNC", swAutoSync.ToString());
            Preferences.Set("LECTOR", eLector.Text);
            if (eLector.Text == "338")
            {
                Preferences.Set("DEV", "True");
            }
            else
            {
                Preferences.Set("DEV", "False");
            }

            await PopupNavigation.PushAsync(new PopUpGuardarConfig());

        }

        private async void pinbtn_Clicked(object sender, EventArgs e)  //PING INFERIOR//
        {
            try
            {
                Ping Pings = new Ping();
                int timeout = 1;
                if (Pings.Send(registrosIP.Text, timeout).Status == IPStatus.Success)
                {

                    try
                    {

                        string connectionString = "User ID=sysdba;Password=masterkey;" +
                               "Database=C:\\APP\\GAD\\datos_214.fdb;" +
                               $"DataSource={registrosIP.Text};Port=3050;Charset=NONE;Server Type=0;";

                        FbConnection fb = new FbConnection(connectionString);
                   
                        fb.Open();


                        fb.Close();
                        await PopupNavigation.PushAsync(new PopUpPing()); //popup conexion con exito//
                    }
                    catch (Exception)
                    {

                        await PopupNavigation.PushAsync(new PopUpPingIncorrecto());//popup conexion erronea//
                    }
                    
                }
                else
                {
                    await PopupNavigation.PushAsync(new PopUpPingIncorrecto());//popup conexion erronea//
                }
                
            }
            catch (Exception)
            {

                throw;
            }
            

        }

        private async void pinbutton_Clicked(object sender, EventArgs e)
        {
            try
            {              
                Ping Pings = new Ping();
                int timeout = 1;

                if (Pings.Send(eServerIP.Text, timeout).Status == IPStatus.Success)
                {

                    try
                    {
                        string connectionString = "User ID=sysdba;Password=masterkey;" +
                                           "Database=C:\\APP\\GAD\\registros.fdb;" +
                                           $"DataSource={eServerIP.Text};Port=3050;Charset=NONE;Server Type=0;";

                        FbConnection fb = new FbConnection(connectionString);
                        fb.Open();


                        fb.Close();
                        await PopupNavigation.PushAsync(new PopUpPing()); //popup conexion con exito//
                    }
                    catch (Exception)
                    {

                        await PopupNavigation.PushAsync(new PopUpPingIncorrecto()); //popup conexion erronea//
                    }
                }
                else
                {
                    await PopupNavigation.PushAsync(new PopUpPingIncorrecto());//popup conexion erronea//
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}