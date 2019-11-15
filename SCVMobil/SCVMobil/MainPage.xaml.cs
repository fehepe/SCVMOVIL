using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Essentials;
using SQLite;
using System.Diagnostics;
using Honeywell.AIDC.CrossPlatform;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;
using System.Net.Http;
using Newtonsoft.Json;
using FirebirdSql.Data.FirebirdClient;
using System.Threading.Tasks;
using System.Data;
using Plugin.Toasts;

namespace SCVMobil
{
    public partial class MainPage : ContentPage
    {
        //Variables
        //--------------------------------------------------------------------------
        private HttpClient _client = new HttpClient();
        private string Url = "";
        Escaner scanner;


        //---------------------------------------------------------------------------

        public MainPage()//Constructor
        {

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (Preferences.Get("AUTO_SYNC", "False") == "True")
                {
                    Device.BeginInvokeOnMainThread(() => refreshPage());
                    Debug.WriteLine("Refreshed");

                }
                return true;
            });
          
            InitializeComponent();
            scanner = new Escaner(cedulaScanned);
           
            
        }


        //--------------------------------------------------------------------------------------
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            fmPassConf.VerticalOptions = LayoutOptions.CenterAndExpand;
            ppPasswordConfig.IsVisible = true;
            lbWrongPass.IsVisible = false;
        }


        //Refrescar la pagina
        //---------------------------------------------------------------------------------------
        public void refreshPage()
        {
            if (Preferences.Get("SYNC_VSU", false))
            {
                imgNoSync.IsVisible = false;
                imgSync.IsVisible = true;
            }
            else
            {
                imgNoSync.IsVisible = true;
                imgSync.IsVisible = false;
            }
        }
        //-----------------------------------------------------------------------------------------
        protected override void OnAppearing() //Cuando aparezca la pagina, refrescamos.
        {
            // OnGetList();
            Debug.WriteLine("Appeared");
            refreshPage();
            scanner.GetScanner(true);
            entCedula.Text = string.Empty;
            entApellidos.Text = string.Empty;
            entNombres.Text = string.Empty;
            lblVersion.Text = Preferences.Get("VERSION", "0.0.0.0.0");

        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        protected override void OnDisappearing()//Cuando la pagina desaparezca
        {
            scanner.GetScanner(false);// Se desactiva el Scaner

            ppCedulaNoExiste.IsVisible = false;// Ocultamos el Popup de documento no existe
            Preferences.Set("PAGE_ACTIVE", "MainPage");
           

        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public async void cedulaScanned(string inString)// Metodo que se invoca cuando se scanea un documento
        {
            if (inString.Contains("|"))// Si lo scaneado contiene un | me elimina todo lo que sigue despues, esto sirve para poder scanear las licencias
            {
                inString = inString.Substring(0, inString.IndexOf("|") + 0);
                entrada(inString);
            }
            else// Este else se utiliza para que en caso de que no sea una licencia, continue haciendo lo mismo.
            {
                entrada(inString);
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private async void EntCedula_Completed(object sender, EventArgs e)// Metodo Enter del Entry de la Cedula
        {
            entrada(entCedula.Text);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //Cancelar Ventana de Password
        private void BtCancelPass_Clicked(object sender, EventArgs e)
        {
            ppPasswordConfig.IsVisible = false;
            entPassword.Text = string.Empty;
            lbWrongPass.IsVisible = false;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //OK Ventana de Password
        private void BtOKPass_Clicked(object sender, EventArgs e)
        {
            if (entPassword.Text == "1")
            {
                ppPasswordConfig.IsVisible = false;
                entPassword.Text = string.Empty;
                Navigation.PushAsync(new AppSettingsPage());
            }
            else
            {
                lbWrongPass.IsVisible = true;
                try
                {
                    var duration = TimeSpan.FromSeconds(0.5);
                    Vibration.Vibrate(duration);
                }
                catch
                {
                    //TODO: HANDLE NO VIBRATE
                }
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Revisar Password
        private void EntPassword_Completed(object sender, EventArgs e)
        {
            if (entPassword.Text == "cr52401")
            {
                ppPasswordConfig.IsVisible = false;
                ((Entry)sender).Text = string.Empty;
                Navigation.PushAsync(new SettingsPage());
            }
            else
            {
                lbWrongPass.IsVisible = true;
                ((Entry)sender).Text = string.Empty;
                Analytics.TrackEvent("Password de config invalido en el escaner" + Preferences.Get("LECTOR", "N/A"));
                try
                {
                    var duration = TimeSpan.FromSeconds(0.5);
                    Vibration.Vibrate(duration);
                }
                catch
                {
                    //TODO: HANDLE NO VIBRATE
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Mover la ventana de password cuando salga el teclado
        private void EntPassword_Focused(object sender, FocusEventArgs e)
        {
            fmPassConf.VerticalOptions = LayoutOptions.StartAndExpand;
        }

        //Mover la ventna de password cuando se esconda el teclado
        private void EntPassword_Unfocused(object sender, FocusEventArgs e)
        {
            fmPassConf.VerticalOptions = LayoutOptions.CenterAndExpand;
        }

        //Ok Ventana de Cedula no existe.
        private void BtOKCedulaNoExiste_Clicked(object sender, EventArgs e)
        {

            Navigation.PushAsync(new RegisterPage(entCedula.Text));
        }

        //Boton de agregar cedula.


        //Boton de Limpiar los campos
        private void BtClear_Clicked(object sender, EventArgs e)
        {
            entApellidos.Text = "";
            entCedula.Text = "";
            entNombres.Text = "";
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public async void entrada(String inString)
        {
            if (inString != "")
            {
                if (inString.Length == 11 || inString.StartsWith("ID"))
                {

                
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                //Vamos a ver si es un ID de salida.
                if (inString.StartsWith("ID"))
                {
                    var querry = "SELECT * FROM Invitados WHERE INVIDATO_ID = " + inString.Replace("ID", "") + " AND FECHA_SALIDA is null AND Fecha_Verificacion is not null";
                    var querrys = "SELECT * FROM Invitados WHERE INVIDATO_ID= " + inString.Replace("ID", "") + " AND FECHA_SALIDA is null AND Fecha_Verificacion is null";
                    var registroInv = db.Query<Invitados>(querry);
                    var registroVer = db.Query<Invitados>(querrys);

                        if (registroVer.Any() && Preferences.Get("VERIFICA", false))
                        {
                            if (registroVer.First().Fecha_Verificacion is null)
                            {
                                registroVer.First().Fecha_Verificacion = DateTime.Now;
                                registroVer.First().Puerta_Registro = Convert.ToInt32(Preferences.Get("PUERTA", "1459").ToString());
                                registroVer.First().verificacionSubida = null;
                                db.UpdateAll(registroVer);
                                DependencyService.Get<IToastMessage>().DisplayMessage("Se ha verificado correctamente.");
                                await Navigation.PushAsync(new Verificacion(registroVer.First()));
                            }
                            else if((registroVer.First().Fecha_Verificacion.Value.Date-DateTime.Now).TotalMinutes < 1)
                            {
                                DependencyService.Get<IToastMessage>().DisplayMessage("Se ha acaba de verificar este id.");
                            }
                            
                        }else if (registroInv.Any())
                        {
                            if (registroInv.First().Fecha_Salida is null)
                            {
                                registroInv.First().Fecha_Salida = DateTime.Now;
                                registroInv.First().salidaSubida = null;
                                db.UpdateAll(registroInv);
                                DependencyService.Get<IToastMessage>().DisplayMessage("Se ha dado salida correctamente.");
                            }
                            else
                            {
                                DependencyService.Get<IToastMessage>().DisplayMessage("Esta persona ya salió.");
                            }
                        }


                        else
                        {
                        DependencyService.Get<IToastMessage>().DisplayMessage("No existe este ID de salida");

                        var querrySal = "SELECT * FROM SalidaOffline WHERE INVIDATO_ID = " + inString.Replace("ID", "");
                        var registroSal = db.Query<SalidaOffline>(querrySal);
                        if (!registroSal.Any())
                        {
                            //Hacer el insert a la base de datos local para subirlo mas tarde
                            var SalOF = new SalidaOffline();
                            SalOF.INVIDATO_ID = int.Parse(inString.Replace("ID", ""));
                            SalOF.Fecha_Salida = DateTime.Now;
                            SalOF.Subida = null;
                            db.Insert(SalOF);
                        }
                        else
                        {
                            DependencyService.Get<IToastMessage>().DisplayMessage("Este ID de Salida ya se encuentra en la base de datos");
                        }
                    }
                }
                else
                {

                    //Vamos a verificar si la persona ya entro.
                    var querry = "SELECT * FROM INVITADOS WHERE CARGO = '" + inString + "' AND FECHA_SALIDA is NULL";

                    var registroVerifInv = db.Query<Invitados>(querry);

                    if (registroVerifInv.Any())
                    {
                        
                        querry = "SELECT * FROM PADRON WHERE CEDULA = '" + inString + "'";
                        var registro = db.Query<PADRON>(querry);

                        entCedula.Text = registro.First().CEDULA;
                        entNombres.Text = registro.First().NOMBRES;
                        entApellidos.Text = registro.First().APELLIDO1 + " " + registro.First().APELLIDO2;
                        //Esto quiere decir que aun la persona no ha salido.
                        await Navigation.PushAsync(new SalidaPage(inString, entNombres.Text, entApellidos.Text));
                    }
                    else
                    {
                        //Vamos a ver si la persona tiene una reserva.

                        querry = "SELECT * FROM VW_RESERVA_VISITA WHERE VISITAS_DOCUMENTO = '" + inString + "' and visitas_Fecha_reserva="+DateTime.Today.Ticks;


                        var registroRes = db.Query<VW_RESERVA_VISITA>(querry);
                        if (registroRes.Count > 0)
                        {
                            await Navigation.PushAsync(new ReservasPage(registroRes));
                        }
                        else
                        {
                            // Como no tiene reserva entramos.
                            //Buscamos la cedula en el padron.
                            querry = "SELECT * FROM PADRON WHERE CEDULA = '" + inString + "'";

                            try
                            {
                                //Ejecutamos el query.
                                var registro = db.Query<PADRON>(querry);

                                entCedula.Text = registro.First().CEDULA;
                                entNombres.Text = registro.First().NOMBRES;
                                entApellidos.Text = registro.First().APELLIDO1 + " " + registro.First().APELLIDO2;
                                await Navigation.PushAsync(new CompanyPage(entCedula.Text, entNombres.Text, entApellidos.Text));

                            }
                            catch (Exception ey)
                            {
                                entApellidos.Text = string.Empty;
                                entNombres.Text = string.Empty;
                                entCedula.Text = inString;
                                var properties = new Dictionary<string, string> {
                                    { "Category", "Documento NO EXISTE?" },
                                    { "Code", "MainPage.xaml.cs Line: 179" },
                                    { "Lector", Preferences.Get("LECTOR", "0")}
                                };
                                Crashes.TrackError(ey, properties);
                                ppCedulaNoExiste.IsVisible = true;
                                Analytics.TrackEvent("Documento No-Existe: " + inString + " en el escaner " + Preferences.Get("LECTOR", "N/A"));
                                await TextToSpeech.SpeakAsync("Documento No Existe");
                                try
                                {
                                    var duration = TimeSpan.FromSeconds(1);
                                    Vibration.Vibrate(duration);
                                }
                                catch
                                {
                                    //TODO: HANDLE NO VIBRATE
                                }
                                Debug.WriteLine(ey);
                            }
                        }
                    }

                }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().DisplayMessage("Escaneo un codigo que no es la cedula");
                }
            }
        }

        private async void ToolbarItem_Clicked_Test(object sender, EventArgs e)
        {
            /*
            string connectionString = "User ID=sysdba;Password=masterkey;" +
                           "Database=C:\\APP\\GAD\\datos_214.fdb; " +
                           $"DataSource={Preferences.Get("SERVER_IP", "localhost")};Port=3050;Charset=NONE;Server Type=0;";
            string querrys = "select p.cedula, p.nombres, p.apellido1, p.apellido2 from padron p";
            FbConnection fb = new FbConnection(connectionString);
            FbDataReader myReader = null;
            DataTable dataTable = new DataTable();
            FbCommand myCommand = new FbCommand(querrys, fb);
            var listPadron = new List<PADRON>();
            var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
            try
            {
                fb.Open();
                myCommand.CommandTimeout = 0;
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    PADRON persona = new PADRON();
                    persona.CEDULA = myReader[0].ToString();
                    persona.NOMBRES = myReader[1].ToString();
                    persona.APELLIDO1 = myReader[2].ToString();
                    persona.APELLIDO2 = myReader[3].ToString();
                    listPadron.Add(persona);
                }











                fb.Close();

                await Task.Factory.StartNew(() =>
                {
                    db.InsertAll(listPadron);
                });
            }
            catch (Exception ea)
            {
                DependencyService.Get<IToastMessage>().DisplayMessage("No se Conecto." + ea.Message);
            }

            //string connectionString = "User ID=sysdba;Password=masterkey;" +
            //               "Database=C:\\APP\\GAD\\registros.fdb; " +
            //               $"DataSource={Preferences.Get("SERVER_IP", "localhost")};Port=3050;Charset=NONE;Server Type=0;";
            ////"Server=192.168.1.170;User ID=sysdba;Password=masterkey;Charser=NONE;Database=192.168.1.70:C:\\APP\\GAD\\registros.fdb";
            //try
            //{
            //    FbConnection fb = new FbConnection(connectionString);
            //    fb.Open();
            //    DependencyService.Get<IToastMessage>().DisplayMessage("Se Conecto.");
            //}
            //catch (Exception ea)
            //{
            //    DependencyService.Get<IToastMessage>().DisplayMessage("No se Conecto." + ea.Message);
            //}
            */
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

                    var querry1 = "";

                    //Vamos a buscar el directorio de la base de datos
                    var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));

                    //Buscamos el Chunk Size
                    var iChunkSize = int.Parse(Preferences.Get("CHUNK_SIZE", "10000"));

                    //Vamos a poner el timeout del http en 20 min seg
                    _client.Timeout = new TimeSpan(0, 40, 0);

                    Debug.WriteLine("Setting timeout to 20 min");

                    string querry = "";
                    string connectionString = "User ID=sysdba;Password=masterkey;" +
                           "Database=C:\\APP\\GAD\\datos_214.fdb;" +
                           $"DataSource={Preferences.Get("SERVER_IP", "localhost")};Port=3050;Charset=NONE;Server Type=0;";
                    
                    
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

                            break;
                        }
                        catch(Exception ex)
                        {
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
                                querry1 = querry1 =  querry + " ROWS "
                                     + loadedRegistros.ToString() + " TO " + maxRegistro.ToString();
                                progress = 1.0;
                            }
                            else
                            {
                                querry1 = querry + " ROWS "
                                     + loadedRegistros.ToString() + " TO " + (loadedRegistros + iChunkSize - 1).ToString();
                            }
                            string contenidoTBL;
                            int triesTBL = 0;
                            while (true)
                            {
                                try
                                {
                                    FbConnection fb = new FbConnection(connectionString);

                                    fb.Open();
                                    FbCommand command = new FbCommand(
                                        querry1,
                                        fb);


                                    FbDataReader reader = command.ExecuteReader();

                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            PADRON persona = new PADRON();
                                            persona.CEDULA = reader[0].ToString();
                                            persona.NOMBRES = reader[1].ToString();
                                            persona.APELLIDO1 = reader[2].ToString();
                                            persona.APELLIDO2 = reader[3].ToString();

                                            listPadron.Add(persona);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("No rows found.");
                                    }
                                    reader.Close();

                                    fb.Close();
                                    //contenidoTBL = await _client.GetStringAsync(Url1);
                                    break;
                                }
                                catch (Exception et)
                                {
                                    Debug.WriteLine("Error descargando padron: " + et.Message);
                                    if (triesTBL >= 5)
                                    {
                                        throw new Exception("No se pudo conectar con la base de datos: " + et.Message);
                                    }
                                    triesTBL += 1;
                                }
                            }
                            //var listPadronTemp = JsonConvert.DeserializeObject<List<PADRON>>(contenidoTBL);
                            //listPadron = listPadron.Concat(listPadronTemp).ToList();
                            if (listPadron.Count >= int.Parse(Preferences.Get("COMMIT_SIZE", "1000000")))
                            {
                                try
                                {
                                    lblLoadingText.Text = "Commited " + commitedRegistros.ToString() + "/" + maxRegistro.ToString() + " BOLETAS";
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
                            lblLoadingText.Text = "Downloading BOLETAS: " + loadedRegistros.ToString() + "/" + maxRegistro.ToString();
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

        
    }
}



