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

namespace SCVMobil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyPage : ContentPage
    {

        //----------------------------------------------------------------------------------------------
        //Variables
        private HttpClient _client = new HttpClient();
        //private string Url = "";
        //private Dictionary<string, BarcodeReader> mBarcodeReaders;
        Escaner scan;
        private string stNombre, stApellidos;

        //-----------------------------------------------------------------------------------------------       
        public CompanyPage(String cedula, String nombre, String apellidos)//Constructor

        {
            InitializeComponent();
            entCedula.Text = cedula;
            entNombre.Text = nombre + " " + apellidos;
            scan = new Escaner(entryScan);
            stNombre = nombre;
            stApellidos = apellidos;
           
        }
        //-------------------------------------------------------------------------------------------------------------------

        protected override void OnAppearing()
        {
            base.OnAppearing();
           //
            
            scan.GetScanner(true);

            if(Preferences.Get("PLACA_SELECTED", true))
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

            pickerVisitaA.ItemsSource = Preferences.Get("PERSONAS_LIST", "").Split(',').ToList<string>();

            if (!string.IsNullOrEmpty(pickerVisitaA.ItemsSource[0].ToString()) && Preferences.Get("VISITA_A_SELECTED", true))
            {
                FrameVisitaA.IsVisible = true;
                FrameVisitaA2.IsVisible = true;
                lblvisitaA.IsVisible = true;
                pickerVisitaA.IsVisible = true;
            }
            else
            {
                Preferences.Set("VISITA_A_SELECTED", false);
                FrameVisitaA.IsVisible = false;
                FrameVisitaA2.IsVisible = false;
                pickerVisitaA.IsVisible = false;
                pickerVisitaA.ItemsSource = new List<string>();
                lblvisitaA.IsVisible = false;
            }

            pickerDestino.ItemsSource = Preferences.Get("COMPANIAS_LIST", "").Split(',').ToList<string>();
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
                
                Debug.WriteLine("Error en onAppearing");
                Analytics.TrackEvent("Error al mostrar Invitados: " + ex.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            }
        }
        //----------------------------------------------------------------------------------------------------------------------
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            scan.GetScanner(false);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------
      /*  protected  void obtenerListaDestino()//Vamos a buscar las AREAS definidas en la base de datos
        {
           
                var stlRegistros = new List<string>();
                var stRegisros = "";
            try
            {
                var querry = "SELECT COMPANIA_ID, NOMBRE  FROM COMPANIAS ORDER BY NOMBRE ";
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
              
                var tr = db.Query<COMPANIAS>(querry);
                
                    foreach (COMPANIAS registro in tr)
                    {
                        //db.Insert(registro);
                        Debug.WriteLine("EMPRESAS: " + registro.NOMBRE);
                        stlRegistros.Add(registro.NOMBRE.ToString());
                        stRegisros = stRegisros + "," + registro.NOMBRE.ToString();
                    }
                    stRegisros = stRegisros.TrimStart(',');
                    Preferences.Set("COMPANIAS_LIST", stRegisros);

                
            }
            catch (Exception ey)
            {
                Debug.WriteLine("" + ey);
                Debug.WriteLine("With url: " + Url);
            }
            finally
            {

                pickerDestino.ItemsSource = stlRegistros;


                pickerDestino.SelectedItem = Preferences.Get("COMPANIA_SELECTED", "");



            }
        }*/
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------
       /* protected  void obtenerListaVisitA()//Vamos a buscar las AREAS definidas en la base de datos
        {

           
                
                    var stlRegistros = new List<string>();
                    var stRegisros = "";
            try
            {

                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                //Creamos el query para buscar el documento.
                var querry = "SELECT PERSONA_ID, NOMBRES_APELLIDOS FROM PERSONAS";

                var tr = db.Query<PERSONAS>(querry);
             
                    foreach (PERSONAS registro in tr)

                    {
                       // db.Insert(registro);
                        Debug.WriteLine("PERSONAS: " + registro.NOMBRES_APELLIDOS);
                        stlRegistros.Add(registro.NOMBRES_APELLIDOS.ToString());
                        stRegisros = stRegisros + "," + registro.NOMBRES_APELLIDOS.ToString();
                    }
                    stRegisros = stRegisros.TrimStart(',');
                    Preferences.Set("PERSONAS_LIST", stRegisros);
                    //pkParques.ItemsSource = stlRegistros;

                
            }
            catch (Exception ey)
            {
                Debug.WriteLine("" + ey);
                Debug.WriteLine("With url: " + Url);
            }
            finally
            {

                pickerVisitaA.SelectedItem = Preferences.Get("PERSONA_SELECTED", "");


            }
            
        }*/


        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------  
        public void refreshPage()
        {

        }
        //-----------------------------------------------------------------------------
        private void EntPlaca_Completed(object sender, EventArgs e)
        {
            // Preferences.Set("PAGE_ACTIVE", "CompanyPage");
        }


        //--------------------------------------------------------------------------------    

        private void EntCedula_Completed(object sender, EventArgs e)
        {

        }
        //----------------------------------------------------------------------------------------------------------
        public void entryScan(String scanneo)// Metodo para poder Scanear
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
                                DisplayAlert("Error", "Este Marbete no es Valido", "Ok");
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
                                    DisplayAlert("Error", "Ha ocurrido un error", "Ok");
                                }
                            }
                        }
                        else
                        {
                            if (scanneo.Contains("FOLIO"))// Este es una parte del string de la placas de del ano 2017-2018
                            {
                                DisplayAlert("Error", "Este Marbete no es Valido", "Ok");
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
                    DisplayAlert("Error", "Este Marbete no es Valido", "Ok");
                }
                    
                
                
            }catch(Exception ex)
            {
                Debug.WriteLine("Error en entryScan");
                Analytics.TrackEvent("Error al escanear: " + ex.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
                DependencyService.Get<IToastMessage>().DisplayMessage("Ha ocurrido un error: "+ex.Message);
            }
        }
     
        
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        private async void BtnImprimir_Clicked(object sender, EventArgs e)//Metodo del boton de Imprimir
        {

            var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));

            if (pickerVisitaA.IsVisible)
            {
                if (pickerDestino.SelectedItem != null  || pickerVisitaA.SelectedItem != null)
                {

                    var registroInvitados = new Invitados();

                     //Vamos a buscar la compania seleccionada 
                    try
                    {
                        var TBL_COMPANIAS = db.Query<COMPANIAS>("SELECT COMPANIA_ID FROM COMPANIAS WHERE NOMBRE = '" + pickerDestino.SelectedItem.ToString() + "'");

                     //Vamos a buscar la persona seleccionada
                    
                        if (pickerVisitaA.SelectedIndex != -1)
                        {
                            var TBL_PERSONAS = db.Query<PERSONAS>("SELECT PERSONA_ID FROM PERSONAS WHERE NOMBRES_APELLIDOS = '" + pickerVisitaA.SelectedItem.ToString() + "'");
                            int? visitaA;
                            if (TBL_PERSONAS.Any())
                            {
                                visitaA = TBL_PERSONAS.First().PERSONA_ID;
                            }
                            else
                            {
                                visitaA = null;
                            }
                            registroInvitados.Compania_ID = TBL_COMPANIAS.First().COMPANIA_ID;
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
                            registroInvitados.Origen_Entrada = "VISTA";
                            registroInvitados.Origen_Salida = "VISTA";
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

                            db.Insert(registroInvitados);
                            btnImprimir.IsEnabled = false;
                            await Navigation.PopToRootAsync();
                        }
                        else
                        {


                            registroInvitados.Compania_ID = TBL_COMPANIAS.First().COMPANIA_ID;
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
                            registroInvitados.Origen_Entrada = "VISTA";
                            registroInvitados.Origen_Salida = "VISTA";
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

                            db.Insert(registroInvitados);
                            btnImprimir.IsEnabled = false;
                            await Navigation.PopToRootAsync();

                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error en BtnImprimir");
                        Analytics.TrackEvent("Error al buscar compañias seleccionadas: " + ex.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
                        throw;
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

                    var registroInvitados = new Invitados();

                    //Vamos a buscar la compania seleccionada 

                    var TBL_COMPANIAS = db.Query<COMPANIAS>("SELECT COMPANIA_ID FROM COMPANIAS WHERE NOMBRE = '" + pickerDestino.SelectedItem.ToString() + "'");

                    //Vamos a buscar la persona seleccionada
                    registroInvitados.Compania_ID = TBL_COMPANIAS.First().COMPANIA_ID;
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
                    registroInvitados.Puerta_Entrada = Convert.ToInt32(Preferences.Get("LOCALIDAD_VSU", "1495"));
                    registroInvitados.Actualizada_La_Salida = 0;
                    registroInvitados.Horas_Caducidad = 12;
                    registroInvitados.Personas = 1;
                    registroInvitados.In_Out = 1;
                    registroInvitados.Origen_Entrada = "VISTA";
                    registroInvitados.Origen_Salida = "VISTA";
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

                    db.Insert(registroInvitados);
                    btnImprimir.IsEnabled = false;
                    await Navigation.PopToRootAsync();
                }
                else
                {
                    DependencyService.Get<IToastMessage>().DisplayMessage("Necesita seleccionar un destino.");
                }
            }

        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void PickerVisitaA_SelectedIndexChanged(object sender, EventArgs e)// Metodo del evento Change del picker, es decir si selecciono un item del picker este lo tomara
        {
            try
            {
                Preferences.Set("PERSONA_SELECTED", pickerVisitaA.SelectedItem.ToString()); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception catched while trying to set preference \"PERSONA_SELECTED\": " + ex);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------
        private void PickerDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            try

            {
                Preferences.Set("COMPANIA_SELECTED", pickerDestino.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception catched while trying to set preference \"COMPANIA_SELECTED\": " + ex);
            }
        }
       

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
    }
}
