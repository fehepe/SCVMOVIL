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
using Honeywell.AIDC.CrossPlatform;
using System.Net;
using System.Web;
using SCVMobil.Models;
using Microsoft.AppCenter.Analytics;
using SCVMobil.Connections;
using System.Data;
using Plugin.BluetoothLE;
using System.Reactive.Linq;
using XF.Bluetooth.Printer.Plugin.Abstractions;
using XLabs.Platform.Device;

namespace SCVMobil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyPage : ContentPage
    {

        //----------------------------------------------------------------------------------------------
        //Variables
        //private HttpClient _client = new HttpClient();
        public FireBirdData fireBird = new FireBirdData(); //NEW//
        //private string Url = "";
        //private Dictionary<string, BarcodeReader> mBarcodeReaders;
        Escaner scan;
        private string stNombre, stApellidos;
        COMPANIAS cc;
        DEPTO_LOCALIDAD depto_localidad;
        PERSONAS persona;
        public List<PERSONAS> PeopleList { get; set; } = new List<PERSONAS>();
        List<string> EstadoCarga = new List<string>()
        {
            "si",
            "No",
            "No Aplica"
        };

        List<string> Comboone = new List<string>();
        List<string> Comboscn = new List<string>();
        List<string> Combothr = new List<string>();

        
        private readonly PRINT _blueToothService;
       // private readonly PRINT prints;
        private readonly IPrint prints;
        private readonly BleScanType bleScan;

        private IList<string> _deviceList;
        public IList<string> DeviceList
        {
            get
            {
                if (_deviceList == null)
                    _deviceList = new ObservableCollection<string>();
                return _deviceList;
            }
            set
            {
                _deviceList = value;
            }
        }

        private string _printMessage;
        public string PrintMessage
        {
            get
            {
                return _printMessage;
            }
            set
            {
                _printMessage = value;
            }
        }

        private string _selectedDevice;
        public string SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                _selectedDevice = value;
            }
        }


        //-----------------------------------------------------------------------------------------------       
        public CompanyPage(String cedula, String nombre, String apellidos)//Constructor
        {
            InitializeComponent();                  
            //lbl1.IsVisible = false;
            //lbl2.IsVisible = false;
            //lbl3.IsVisible = false;
            //lbl4.IsVisible = false;
            //lbl5.IsVisible = false;
            //lbl6.IsVisible = false;
            //lbl1.Text = Preferences.Get("texto1", "");
            //lbl2.Text = Preferences.Get("texto2", "");
            //lbl3.Text = Preferences.Get("texto3", "");
            //fcombo1.IsVisible = false;
            //fcombo2.IsVisible = false;
            //fcombo3.IsVisible = false;

            if (Preferences.Get("VISITA_A_SELECTED", true))
            {
                FrameVisitaA.IsVisible = true;
                FrameVisitaA2.IsVisible = true;
                pickerVisitaA.IsVisible = true;
                lblvisitaA.IsVisible = true;
            }
            else
            {
                FrameVisitaA.IsVisible = false;
                FrameVisitaA2.IsVisible = false;
                pickerVisitaA.IsVisible = false;
                lblvisitaA.IsVisible = false;
            }
           
           
            

            entCedula.Text = cedula;
            entNombre.Text = nombre + " " + apellidos;
            scan = new Escaner(entryScan);
            stNombre = nombre;
            stApellidos = apellidos;
            _blueToothService = DependencyService.Get<PRINT>();
            prints = DependencyService.Get<IPrint>();
           

            //BindDeviceList();
        }

        public void GetCombos()
        {
          

            //var textc1 = Preferences.Get("combo1", "");
            //var textc2 = Preferences.Get("combo2", "");
            //var textc3 = Preferences.Get("combo3", "");

            //Comboone.Add(textc1);
            //Comboone.Add(textc2);
            //Comboone.Add(textc3);


            //var textc4 = Preferences.Get("ccombo1", "");
            //var textc5 = Preferences.Get("ccombo2", "");
            //var textc6 = Preferences.Get("ccombo3", "");

            //Comboscn.Add(textc4);
            //Comboscn.Add(textc5);
            //Comboscn.Add(textc6);

            //var textc7 = Preferences.Get("combofirst", "");
            //var textc8 = Preferences.Get("combosecond", "");
            //var textc9 = Preferences.Get("combothird", "");


            //Combothr.Add(textc7);
            //Combothr.Add(textc8);
            //Combothr.Add(textc9);

            //cmbbox1.ItemsSource = Comboone;
            //cmbbox2.ItemsSource = Comboscn;
            //cmbbox3.ItemsSource = Combothr;
        }
        

        //public void BindDeviceList()
        //{
        //    var list = _blueToothService.GetDeviceList();
        //    DeviceList.Clear();
        //    foreach (var item in list)
        //        DeviceList.Add(item);
        //}
        //-------------------------------------------------------------------------------------------------------------------

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //
            scan.GetScanner(true);
            lbl1.IsVisible = false;
            lbl2.IsVisible = false;
            lbl3.IsVisible = false;
            lbl4.IsVisible = false;
            lbl5.IsVisible = false;
            lbl6.IsVisible = false;
            

            //if (Preferences.Get("RTT", true))
            //{
            //    lblCodigoCarnet.IsVisible = true;
            //    CodeRTT.IsVisible = true;
            //}
            //else
            //{
            //    lblCodigoCarnet.IsVisible = false;
            //    CodeRTT.IsVisible = false;
            //}

            //lbl1.Text = Preferences.Get("texto1", "");
            //lbl2.Text = Preferences.Get("texto2", "");
            //lbl3.Text = Preferences.Get("texto3", "");
            fcombo1.IsVisible = false;
            fcombo2.IsVisible = false;
            fcombo3.IsVisible = false;
            GetCombos();
            if (Preferences.Get("PLACA_SELECTED", true))
            {
                FramaPlaca.IsVisible = true;
                FramePlaca2.IsVisible = true;
                lbPlaca.IsVisible = true;
                entPlaca.IsVisible = true;
            }
            else
            {
                FramaPlaca.IsVisible = false;
                FramePlaca2.IsVisible = false;
                lbPlaca.IsVisible = false;
                entPlaca.IsVisible = false;
            }

            //entPlaca.Text = Preferences.Get("LETRA", "");

            AsignarDepartamentos();

            //if (pickerDestino.ItemsSource.Count > 0 && Preferences.Get("VISITA_A_SELECTED", true))
            //if (pickerDestino.ItemsSource.Count > 0 ) //THIS COMMENTED
            //{
            //    FrameVisitaA.IsVisible = true;
            //    FrameVisitaA2.IsVisible = true;
            //    //lblvisitaA.IsVisible = true;
            //    //pickerVisitaA.IsVisible = true;
            //}
            //else
            //{
            //    Preferences.Set("VISITA_A_SELECTED", false);
            //    FrameVisitaA.IsVisible = false;
            //    FrameVisitaA2.IsVisible = false;
            //    pickerVisitaA.IsVisible = false;
            //    pickerVisitaA.ItemsSource = new List<string>();
            //    lblvisitaA.IsVisible = false;
            //}
            var x = Preferences.Get("COMPANIAS_LIST", "");
            //pickerDestino.ItemsSource = Preferences.Get("COMPANIAS_LIST", "").Split(',').ToList<string>();
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                var TB = db.Query<Invitados>("SELECT * from invitados WHERE cargo ='" + entCedula.Text + "' order by Fecha_registro desc");
                //

                if (TB.Any())
                {
                    var TBL_PERSONAS = db.Query<PERSONAS>("SELECT NOMBRES_APELLIDOS FROM PERSONAS WHERE PERSONA_ID = " + TB.First().Visitado);
                    pickerVisitaA.SelectedItem = TBL_PERSONAS.First().NOMBRES_APELLIDOS;
                    var TBL_COMPANIAS = db.Query<COMPANIAS>("SELECT NOMBRE FROM COMPANIAS WHERE COMPANIA_ID = " + TB.First().Compania_ID);
                    pickerDestino.SelectedItem = TBL_COMPANIAS.First().NOMBRE;
                }
                else
                {
                    pickerDestino.SelectedIndex = -1;
                    pickerVisitaA.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.ToString());
            }
        }
        //----------------------------------------------------------------------------------------------------------------------
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            scan.GetScanner(false);
        }

        //-----------------------------------------------------------------------------
        public void refreshPage()
        {

        }

        //-----------------------------------------------------------------------------
        private void EntPlaca_Completed(object sender, EventArgs e)
        {
           
        }

        //------------------------------------------------------------------------------
        private void EntCedula_Completed(object sender, EventArgs e)
        {
            
        }

        //------------------------------------------------------------------------------
        public async void entryScan(String scanneo)// Metodo para poder Scanear
        {
            try
            {
                string Strings = scanneo;
                var scaneo = scanneo.Length;
                if (scanneo.Contains("http"))
                {
                    var x = Strings.IndexOf("http");
                    Strings = Strings.Substring(x, 44);
                    scanneo = Strings;
                    Uri uri = new Uri(Strings);
                    string p = HttpUtility.ParseQueryString(uri.Query).Get("dm");//Obteniendo el paramentro dm que es el codigo unico de cada persona con marbete
                    Debug.WriteLine("Data: " + p);

                    var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                    var TB = db.Query<PLACA>("SELECT PLACA_CODE from PLACA WHERE codigo ='" + p + "'");// Buscando en la base de datos placa la placa que concuerde con ese codigo sin importar que persona este ingresando
                    if (TB.Any())
                    {
                        entPlaca.Text = TB.First().PLACA_CODE;
                    }
                    else
                    {
                        if (Connectivity.NetworkAccess == NetworkAccess.Internet)// En caso de que no encuentre a ninguna placa y haya conexion a internet buscara en la pagina de la DGII
                        {
                            if (scanneo.Contains("FOLIO"))// Este es una parte del string de la placas de del ano 2017-2018
                            {
                                await DisplayAlert("Error", "Este Marbete no es Valido", "Ok");
                            }
                            else
                            {
                                try
                                {
                                    WebClient webClient = new WebClient();
                                    string page = webClient.DownloadString(scanneo);

                                    HtmlAgilityPack.HtmlDocument docu = new HtmlAgilityPack.HtmlDocument();
                                    docu.LoadHtml(page);

                                    List<List<string>> table = docu.DocumentNode.SelectSingleNode("//table[@class='data_table']")
                                                .Descendants("tr")
                                                .Skip(1)
                                                .Where(tr => tr.Elements("td").Count() > 1)
                                                .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                                                .ToList();
                                    var placa = table[5][1].ToString();
                                    entPlaca.Text = placa;
                                    Debug.WriteLine("Data: " + p);
                                    var registroPlaca = new PLACA();
                                    registroPlaca.CODIGO = p;
                                    registroPlaca.PLACA_CODE = placa;
                                    db.Insert(registroPlaca);
                                    Console.WriteLine("Insertando: " + registroPlaca);
                                }
                                catch (Exception ex)
                                {
                                    await DisplayAlert("Error", "Ha ocurrido un error", "Ok");
                                }
                            }
                        }
                        else
                        {
                            if (scanneo.Contains("FOLIO"))// Este es una parte del string de la placas de del ano 2017-2018
                            {
                                await DisplayAlert("Error", "Este Marbete no es Valido", "Ok");
                            }
                            else
                            {
                                DependencyService.Get<IToastMessage>().DisplayMessage("No posee internet digite manualmente.");
                            }
                        }
                    }
                }
                else
                {
                   await DisplayAlert("Error", "Este Marbete no es Valido", "Ok");
                }



            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en entryScan");
                Analytics.TrackEvent("Error al escanear: " + ex.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
                DependencyService.Get<IToastMessage>().DisplayMessage("Ha ocurrido un error: " + ex.Message);
            }
        }

        //------------------------------------------------------------------------------
        private async void BtnImprimir_Clicked(object sender, EventArgs e)//Metodo del boton de Imprimir
        {
            try
            {
                Preferences.Set("BUSY", false);
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                var registroInvitados = new Invitados();
                string text = string.Empty;

                if (pickerVisitaA.IsVisible)
                {
                    if (pickerDestino.SelectedItem != null || pickerVisitaA.SelectedItem != null)
                    {

                        //Vamos a buscar la compania seleccionada 
                        try
                        {
                            //Vamos a buscar la persona seleccionada
                            if (pickerVisitaA.SelectedIndex != -1)
                            {
                                int? visitaA;
                                if (persona.PERSONA_ID != null)
                                {
                                    visitaA = persona.PERSONA_ID;
                                }
                                else
                                {
                                    visitaA = null;
                                }
                                var fdate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

                                //fdate.ToString("MM/dd/yyyy HH:mm:ss");

                                registroInvitados.Compania_ID = Preferences.Get("DESTINO_SELECTED",0);
                                registroInvitados.Nombres = stNombre;
                                registroInvitados.Apellidos = stApellidos;
                                registroInvitados.Fecha_Registro = DateTime.Now;
                                registroInvitados.Cargo = entCedula.Text;
                                registroInvitados.Tiene_Activo = 0;
                                registroInvitados.Estatus_ID = 100;
                                registroInvitados.Modulo = 1;
                                registroInvitados.Empresa_ID = null;
                                registroInvitados.Placa = entPlaca.Text;
                                registroInvitados.Tipo_Visitante = "VISITANTE";
                                registroInvitados.Es_Grupo = 0;
                                registroInvitados.Grupo_ID = 0;
                                registroInvitados.Puerta_Entrada = Convert.ToInt32(Preferences.Get("LOCALIDAD_VSU", "1495"));//TODO: Cambiar a parametro!
                                registroInvitados.Actualizada_La_Salida = 0;
                                registroInvitados.Horas_Caducidad = 12;
                                registroInvitados.Personas = 1;
                                registroInvitados.In_Out = 1;
                                registroInvitados.Origen_Entrada = "MANUAL";
                                registroInvitados.Origen_Salida = "MANUAL";
                                registroInvitados.Comentario = "";
                                registroInvitados.Origen_IO = 0;
                                registroInvitados.Cpost = "I";
                                registroInvitados.Actualizado = 0;
                                registroInvitados.Secuencia_Dia = "1";
                                registroInvitados.No_Aplica_Induccion = "0";
                                registroInvitados.Subida = null;
                                registroInvitados.salidaSubida = null;
                                registroInvitados.Visitado = visitaA;
                                registroInvitados.Lector = int.Parse(Preferences.Get("LECTOR", "1"));
                                if (!string.IsNullOrWhiteSpace(entCodigoCarnet.Text)) //CODIGO CARNET COMENTADO//
                                {
                                    registroInvitados.Codigo_carnet = entCodigoCarnet.Text.ToUpper();
                                }

                                db.Insert(registroInvitados);
                                fireBird.UploadVisits();
                                fireBird.DownloadGuests();
                                var inserted = db.Query<Invitados>($"SELECT * FROM INVITADOS WHERE CARGO = '{registroInvitados.Cargo}' ORDER BY Fecha_Registro DESC").FirstOrDefault();

                                var empresa = Preferences.Get("Empresa", "").ToUpper();
                                //var textempresa = empresa.ToUpper();
                                var depar = Preferences.Get("Departamento", "").ToUpper();
                                //var departamento = depar.ToUpper();
                                var piso = Preferences.Get("Piso", "").ToUpper();
                                //piso = piso.ToUpper();

                                var st = "^XA" +
                                       "^FX" +
                                       "^CF0,55" +
                                       $"^FO70,30^FD{empresa}^FS" +
                                       $"^CF0,45" +
                                       $"^FO70,100^FD{registroInvitados.Nombres + " " + registroInvitados.Apellidos}^FS" +
                                       $"^FO70,170^FD{depar}^FS" +
                                       $"^FO70,230^FD{piso}^FS" +
                                       $"^FO70,290^FD{registroInvitados.Fecha_Registro}^FS" +
                                       $"^FO70,360^FDFavor Devolver Ticket en la salida^FS^FS" +
                                       "^FS" +
                                        "^MMT" +
                                        "^PW2400" +
                                        "^LL1200" +
                                        "^LS5" +
                                        "^BY2,3,73" +
                                        "^FO750,20" +
                                        "^BCR,,Y,N" +
                                        $"^FD{"ID" + inserted.INVIDATO_ID}" +
                                        "^FS" +
                                        "^PQ1,0,1,Y" +
                                        "^FO10,85^GB650,1,3^FS" +
                                        "^FO10,150 ^ GB650,1,3 ^ FS" +
                                        "^XZ"; ;

                                var searchemp = db.Query<PERSONAS>($"SELECT DOCUMENTO FROM PERSONAS WHERE DOCUMENTO = '{entCedula.Text}'");
                                if (searchemp.Count > 0)
                                {
                                    await DisplayAlert("", "Este es un empleado, por lo cual no se imprimira ticket", "ok");
                                    await Navigation.PopToRootAsync();
                                }
                                else
                                {
                                    //await prints.PrintText($"{st}", "RP4-18145B4DE7");
                                    await prints.PrintText($"{st}", "Printer - RP4");
                                    await Navigation.PopToRootAsync();
                                }
                                //var concatname = registroInvitados.Nombres + " " + registroInvitados.Apellidos;
                                //if (PeopleList.First().NOMBRES_APELLIDOS == $"{registroInvitados.Nombres + " " + registroInvitados.Apellidos}")
                                //{
                                //    await DisplayAlert("",$"{concatname} es un empleado","Ok");
                                //    //await prints.PrintText($"{st}", "RP4-18145B4DE7");

                                //}
                                //await Navigation.PopToRootAsync();


                            }
                            else
                            {

                                var fdate = new DateTime();

                                fdate.ToString("MM/dd/yyyy HH:mm:ss");

                                registroInvitados.Compania_ID = Preferences.Get("DESTINO_SELECTED", 0);
                                registroInvitados.Nombres = stNombre;
                                registroInvitados.Apellidos = stApellidos;
                                registroInvitados.Fecha_Registro = DateTime.Now;
                                registroInvitados.Cargo = entCedula.Text;
                                registroInvitados.Tiene_Activo = 0;
                                registroInvitados.Estatus_ID = 100;
                                registroInvitados.Modulo = 1;
                                registroInvitados.Empresa_ID = null;
                                registroInvitados.Placa = entPlaca.Text;
                                registroInvitados.Tipo_Visitante = "VISITANTE";
                                registroInvitados.Es_Grupo = 0;
                                //registroInvitados.Grupo_ID = 0;

                                registroInvitados.Puerta_Entrada = Convert.ToInt32(Preferences.Get("LOCALIDAD_VSU", "1495"));
                                registroInvitados.Actualizada_La_Salida = 0;
                                registroInvitados.Horas_Caducidad = 12;
                                registroInvitados.Personas = 1;
                                registroInvitados.In_Out = 1;
                                registroInvitados.Origen_Entrada = "MANUAL";
                                registroInvitados.Origen_Salida = "MANUAL";
                                registroInvitados.Comentario = "";
                                registroInvitados.Origen_IO = 0;
                                registroInvitados.Cpost = "I";
                                registroInvitados.Actualizado = 0;
                                registroInvitados.Secuencia_Dia = "1";
                                registroInvitados.No_Aplica_Induccion = "0";
                                registroInvitados.Subida = null;
                                registroInvitados.salidaSubida = null;
                                registroInvitados.Visitado = null;
                                registroInvitados.Lector = int.Parse(Preferences.Get("LECTOR", "1"));
                                if (!string.IsNullOrWhiteSpace(entCodigoCarnet.Text)) //CODIGO CARNET COMENTADO//
                                {
                                    registroInvitados.Codigo_carnet = entCodigoCarnet.Text.ToUpper();
                                }

                                var inserted = db.Insert(registroInvitados);
                                fireBird.UploadVisits();
                                fireBird.DownloadGuests();
                                var inserted_ = db.Query<Invitados>($"SELECT * FROM INVITADOS WHERE CARGO = '{registroInvitados.Cargo}' ORDER BY Fecha_Registro DESC").FirstOrDefault();

                                var empresa = Preferences.Get("Empresa", "").ToUpper();
                                //var textempresa = empresa.ToUpper();
                                var depar = Preferences.Get("Departamento", "").ToUpper();
                                //var departamento = depar.ToUpper();
                                var piso = Preferences.Get("Piso", "").ToUpper();
                                //piso = piso.ToUpper();

                                var st = "^XA" +
                                       "^FX" +
                                       "^CF0,55" +
                                       $"^FO70,30^FD{empresa}^FS" +
                                       $"^CF0,45" +
                                       $"^FO70,100^FD{registroInvitados.Nombres + " " + registroInvitados.Apellidos}^FS" +
                                       $"^FO70,170^FD{depar}^FS" +
                                       $"^FO70,230^FD{piso}^FS" +
                                       $"^FO70,290^FD{registroInvitados.Fecha_Registro}^FS" +
                                       $"^FO70,360^FDFavor Devolver Ticket en la salida^FS^FS" +
                                       "^FS"+
                                        "^MMT"+
                                        "^PW2400"+
                                        "^LL1200"+
                                        "^LS5"+
                                        "^BY2,3,73"+
                                        "^FO750,20"+
                                        "^BCR,,Y,N"+
                                        $"^FD{"ID" + inserted_.INVIDATO_ID}" +
                                        "^FS"+
                                        "^PQ1,0,1,Y"+
                                        "^FO10,85^GB650,1,3^FS"+
                                        "^FO10,150 ^ GB650,1,3 ^ FS"+
                                        "^XZ";

                                var searchemp = db.Query<PERSONAS>($"SELECT DOCUMENTO FROM PERSONAS WHERE DOCUMENTO = '{entCedula.Text}'");
                                if (searchemp.Count > 0)
                                {
                                    await DisplayAlert("", "Este es un empleado, por lo cual no se imprimira ticket", "ok");
                                    await Navigation.PopToRootAsync();
                                }
                                else
                                {
                                    //await prints.PrintText($"{st}", "RP4-18145B4DE7");
                                    await prints.PrintText($"{st}", "Printer - RP4");
                                    await Navigation.PopToRootAsync();
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error en BtnImprimir: "+ex.Message);
                            Analytics.TrackEvent("Error al buscar compañias seleccionadas: " + ex.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
                            await DisplayAlert("", ex.Message, "ok");
                            //throw;
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().DisplayMessage("Necesita seleccionar un destino y/o a quien visita.");
                    }
                }
                else
                {
                    if (pickerDestino.SelectedItem != null)
                    {

                        var fdate = new DateTime();
                        
                        fdate.ToString("MM/dd/yyyy HH:mm:ss");
                        //Vamos a buscar la persona seleccionada
                        registroInvitados.Compania_ID = Preferences.Get("DESTINO_SELECTED",0);
                        registroInvitados.Nombres = stNombre;
                        registroInvitados.Apellidos = stApellidos;
                        registroInvitados.Fecha_Registro = DateTime.Now; //FORMAT DATE//
                        registroInvitados.Cargo = entCedula.Text;
                        registroInvitados.Tiene_Activo = 0;
                        registroInvitados.Estatus_ID = 100;
                        registroInvitados.Modulo = 1;
                        registroInvitados.Empresa_ID = null;
                        if (string.IsNullOrEmpty(entPlaca.Text))
                        {
                           registroInvitados.Placa = null;
                        }
                        registroInvitados.Tipo_Visitante = "VISITANTE";
                        registroInvitados.Es_Grupo = 0;
                        registroInvitados.Grupo_ID = 0;
                        registroInvitados.Puerta_Entrada = Convert.ToInt32(Preferences.Get("LOCALIDAD_VSU", "1495"));
                        registroInvitados.Actualizada_La_Salida = 0;
                        registroInvitados.Horas_Caducidad = 12;
                        registroInvitados.Personas = 1;
                        registroInvitados.In_Out = 1;
                        registroInvitados.Origen_Entrada = "MANUAL";
                        registroInvitados.Origen_Salida = "MANUAL";
                        registroInvitados.Comentario = "";
                        registroInvitados.Origen_IO = 0;
                        registroInvitados.Cpost = "I";
                        registroInvitados.Actualizado = 0;
                        registroInvitados.Secuencia_Dia = "1";
                        registroInvitados.No_Aplica_Induccion = "0";
                        registroInvitados.Subida = null;
                        registroInvitados.salidaSubida = null;
                        registroInvitados.Visitado = null;
                        registroInvitados.Lector = int.Parse(Preferences.Get("LECTOR", "1"));
                        registroInvitados.Fecha_Salida = null;
                        if (!string.IsNullOrWhiteSpace(entCodigoCarnet.Text))  //CODIGO CARNET COMENTADO//
                        {
                            registroInvitados.Codigo_carnet = entCodigoCarnet.Text.ToUpper();
                        }

                        //var inserted = db.Insert(registroInvitados); COMMENTED BY ME.//
                        db.Insert(registroInvitados);
                        fireBird.UploadVisits();
                        fireBird.DownloadGuests();
                        var inserted__ = db.Query<Invitados>($"SELECT * FROM INVITADOS WHERE CARGO = '{registroInvitados.Cargo}' ORDER BY Fecha_Registro DESC").FirstOrDefault();

                        var empresa = Preferences.Get("Empresa", "").ToUpper();
                        //var textempresa = empresa.ToUpper();
                        var depar = Preferences.Get("Departamento", "").ToUpper();
                        //var departamento = depar.ToUpper();
                        var piso = Preferences.Get("Piso", "").ToUpper();
                        //piso = piso.ToUpper();

                        var st = "^XA" +
                                       "^FX" +
                                       "^CF0,55" +
                                       $"^FO70,30^FD{empresa}^FS" +
                                       $"^CF0,45" +
                                       $"^FO70,100^FD{registroInvitados.Nombres + " " + registroInvitados.Apellidos}^FS" +
                                       $"^FO70,170^FD{depar}^FS" +
                                       $"^FO70,230^FD{piso}^FS" +
                                       $"^FO70,290^FD{registroInvitados.Fecha_Registro}^FS" +
                                       $"^FO70,360^FDFavor Devolver Ticket en la salida^FS^FS" +
                                       "^FS" +
                                        "^MMT" +
                                        "^PW2400" +
                                        "^LL1200" +
                                        "^LS5" +
                                        "^BY2,3,73" +
                                        "^FO750,20" +
                                        "^BCR,,Y,N" +
                                        $"^FD{"ID" + inserted__.INVIDATO_ID}" +
                                        "^FS" +
                                        "^PQ1,0,1,Y" +
                                        "^FO10,85^GB650,1,3^FS" +
                                        "^FO10,150 ^ GB650,1,3 ^ FS" +
                                        "^XZ"; ;
                        var searchemp = db.Query<PERSONAS>($"SELECT DOCUMENTO FROM PERSONAS WHERE DOCUMENTO = '{entCedula.Text}'");
                        if (searchemp.Count > 0)
                        {
                            await DisplayAlert("", "Este es un empleado, por lo cual no se imprimira ticket", "ok");
                            await Navigation.PopToRootAsync();
                        }
                        else
                        {
                            await prints.PrintText($"{st}", "Printer - RP4");
                            await Navigation.PopToRootAsync();
                        }


                       
                       
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().DisplayMessage("Necesita seleccionar un destino.");
                    }
                }

                this.persona = null;

            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en BtnImprimir_Clicked " + ea.Message);
                await DisplayAlert("Error", "Error en BtnImprimir_Clicked: " + ea.Message, "OK");
                //throw;
            }
            finally
            {
                Preferences.Set("BUSY", true);
            }


        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void PickerDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                var selected = pickerDestino.SelectedItem as DEPTO_LOCALIDAD;
                this.depto_localidad = selected;
                Preferences.Set("DESTINO_SELECTED", selected.ID_DEPARTAMENTO);
                //pickerVisitaA.ItemsSource = Preferences.Get("PERSONAS_LIST", "").Split(',').ToList<string>();//
                var Request = db.Query<PERSONAS>($"SELECT * FROM PERSONAS WHERE DEPARTAMENTO_ID = {selected.ID_DEPARTAMENTO}");
                PeopleList = Request;

                if (!Request.Any())
                {
                    FrameVisitaA.IsVisible = false;
                    FrameVisitaA2.IsVisible = false;
                    pickerVisitaA.IsVisible = false;
                    pickerVisitaA.ItemsSource = new List<string>();
                    lblvisitaA.IsVisible = false;
                }
                else
                {
                    //FrameVisitaA.IsVisible = true;
                    //FrameVisitaA2.IsVisible = true;
                    //pickerVisitaA.IsVisible = true;
                    //lblvisitaA.IsVisible = true;
                    AsignarDestinos(Request);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception catched while trying to set preference \"PERSONA_SELECTED\": " + ex);
            }
        }

        private void PickerVisitaA_SelectedIndexChanged(object sender, EventArgs e)// Metodo del evento Change del picker, es decir si selecciono un item del picker este lo tomara
        {
            try
            {
               var selected = pickerVisitaA.SelectedItem as PERSONAS;
               //Preferences.Set("PERSONA_SELECTED", selected.NOMBRES_APELLIDOS.ToString());
               //Preferences.Set("ID_PERSONA_SELECTED", selected.PERSONA_ID.ToString());
               this.persona = selected;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception catched while trying to set preference \"COMPANIA_SELECTED\": " + ex);
            }
        }

        private void entCodigoCarnet_Completed(object sender, EventArgs e)
        {
            entPlaca.Focus();
        }
        private void PickerEstadoCarga_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //------------------------------------------------------------------------------------------------------------------------------


        //------------------------------------------------------------------------------------------------------------------------------------
        private async void Grupo_Clicked(object sender, EventArgs e)// Evento del item del ActionBar llamado Es un grupo, hacemos varias validaciones antes de pasar a la GrupoPage
        {
            if (string.IsNullOrEmpty(entNombre.Text))// Debe haber por lo menos un nombre
            {
                await DisplayAlert("Error", "Debe Ingresar Un Nombre", "Aceptar");
                entNombre.Focus();
                entNombre.IsReadOnly = false;
                return;
            }
            if (string.IsNullOrEmpty(entCedula.Text))// Por lo menos una cedula
            {
                await DisplayAlert("Error", "Debe Ingresar su identificacion", "Aceptar");
                entCedula.Focus();
                return;
            }
            if (pickerDestino.SelectedIndex == -1)// Debe tener un destino
            {
                await DisplayAlert("Error", "Debe Elegir un destino", "Aceptar");
                pickerDestino.Focus();
                return;
            }
            if (Preferences.Get("VISITA_A_SELECTED", true))
            {
                if (pickerVisitaA.SelectedIndex == -1)// Debe saber a quien visita
                {
                    await DisplayAlert("Error", "Debe Ingresar a quien va a visitar", "Aceptar");
                    pickerVisitaA.Focus();
                    return;
                }
            }
            else
            {
                pickerVisitaA.SelectedItem = " ";
            }

            Visitas visita = new Visitas// asignamos los campos
            {
                Cedula = entCedula.Text,
                Nombres = entNombre.Text,
                Empresa = pickerDestino.SelectedItem.ToString(),
                VisitaA = pickerVisitaA.SelectedItem.ToString(),

            };
            using (var datos = new DataAccess())
            {
                datos.DeleteAllVISITAS();// Limpiamos la base de datos
                datos.InsertVisita(visita);// Insertamos en la base de datos

            }
            using (var datos = new DataAccess())
            {
                await Navigation.PushAsync(new GrupoPage(pickerDestino.SelectedItem.ToString(), pickerVisitaA.SelectedItem.ToString(), entCedula.Text, entNombre.Text));// Enviamos todo la informacion al GrupoPage
            }
        }

        private void AsignarDepartamentos()
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                var query = $"SELECT * FROM DEPTO_LOCALIDAD WHERE ID_LOCALIDAD = {Preferences.Get("LOCALIDAD_VSU", "0")}";
                var Departamentos = db.Query<DEPTO_LOCALIDAD>(query);
                pickerDestino.ItemsSource = Departamentos;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Excepcion en el metodo AsignarDepartamentos: "+ex.Message);
            } 
        }

        private async void Campos_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lbl1.Text) && string.IsNullOrEmpty(lbl2.Text) && string.IsNullOrEmpty(lbl3.Text))
            {
                await DisplayAlert("Info", "Para ver los campos debe de llenarlos en configuracion", "OK");

            }
            else
            {
                if (!string.IsNullOrEmpty(lbl1.Text))
                {
                    lbl1.IsVisible = true;
                    fcombo1.IsVisible = true;
                    //Campos.Text = "Desactivar";
                }

                if (!string.IsNullOrEmpty(lbl2.Text))
                {
                    lbl2.IsVisible = true;
                    fcombo2.IsVisible = true;
                    //Campos.Text = "Desactivar";
                }

                if (!string.IsNullOrEmpty(lbl3.Text))
                {
                    lbl3.IsVisible = true;
                    fcombo3.IsVisible = true;
                    //Campos.Text = "Desactivar";
                }
                var list1 = Comboone.Where(elem => elem != "" && elem != null).ToList();
                if (list1.Count > 0)
                {
                    lbl4.IsVisible = true;
                    frame1.IsVisible = true;
                   // Campos.Text = "Desactivar";
                }
                var list2 = Comboscn.Where(elem => elem != "" && elem != null).ToList();
                if (list2.Count > 0)
                {
                    lbl5.IsVisible = true;
                    frame2.IsVisible = true;
                    //Campos.Text = "Desactivar";
                }
                var list3 = Combothr.Where(elem => elem != "" && elem != null).ToList();
                if (list3.Count > 0)
                {
                    lbl6.IsVisible = true;
                    frame3.IsVisible = true;
                    //Campos.Text = "Desactivar";
                }
                Desc.IsEnabled = true;
                Campos.IsEnabled = false;
            }

            


            //if (fcombo1.IsVisible == true)
            //{
            //    Campos.Text = "Activar";
            //    lbl1.IsVisible = false;
            //    lbl2.IsVisible = false;
            //    lbl3.IsVisible = false;
            //    fcombo1.IsVisible = false;
            //    fcombo2.IsVisible = false;
            //    fcombo3.IsVisible = false;
            //    lbl4.IsVisible = false;
            //    lbl5.IsVisible = false;
            //    lbl6.IsVisible = false;
            //    frame1.IsVisible = false;
            //    frame2.IsVisible = false;
            //    frame3.IsVisible = false;


            //}
            //else
            //{
            //    Campos.Text = "Desactivar";
            //    lbl1.IsVisible = true;
            //    lbl2.IsVisible = true;
            //    lbl3.IsVisible = true;
            //    fcombo1.IsVisible = true;
            //    fcombo2.IsVisible = true;
            //    fcombo3.IsVisible = true;
            //    lbl4.IsVisible = true;
            //    lbl5.IsVisible = true;
            //    lbl6.IsVisible = true;
            //    frame1.IsVisible = true;
            //    frame2.IsVisible = true;
            //    frame3.IsVisible = true;


            //} //THIS CODE ARE RIGHT//

            //if (lbl4.IsVisible == true && lbl5.IsVisible == true && lbl6.IsVisible == true)
            //{
            //    Campos.Text = "Activar";
            //    lbl4.IsVisible = false;
            //    lbl5.IsVisible = false;
            //    lbl6.IsVisible = false;
            //    frame1.IsVisible = false;
            //    frame2.IsVisible = false;
            //    frame3.IsVisible = false;
            //}
            //else
            //{
            //    Campos.Text = "Desactivar";
            //    lbl4.IsVisible = true;
            //    lbl5.IsVisible = true;
            //    lbl6.IsVisible = true;
            //    frame1.IsVisible = true;
            //    frame2.IsVisible = true;
            //    frame3.IsVisible = true;                  
            //}
            //}
        }
       
        private void combo3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void combo2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void combo1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Desc_Clicked(object sender, EventArgs e)
        {
            //Campos.Text = "Activar";
            lbl1.IsVisible = false;
            lbl2.IsVisible = false;
            lbl3.IsVisible = false;
            fcombo1.IsVisible = false;
            fcombo2.IsVisible = false;
            fcombo3.IsVisible = false;
            lbl4.IsVisible = false;
            lbl5.IsVisible = false;
            lbl6.IsVisible = false;
            frame1.IsVisible = false;
            frame2.IsVisible = false;
            frame3.IsVisible = false;
            Desc.IsEnabled = false;
            Campos.IsEnabled = true;
        }

        private void AsignarDestinos(List<PERSONAS>lista)
        {
            try
            {
                var Personas = lista;
                if (Personas.Any())
                {
                    pickerVisitaA.ItemsSource = Personas; 
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Excepcion en el metodo AsignarDestino: "+ex.Message);
            }
        }
    }
}