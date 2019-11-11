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

namespace SCVMobil
{
    public class PADRON
    {
        [PrimaryKey]//Seteamos la columna CEDULA como primarykey
        public string CEDULA { get; set; }

        public string NOMBRES { get; set; }

        public string APELLIDO1 { get; set; }

        public string APELLIDO2 { get; set; }

        public PADRON(string sCedula, string sNombres, string sApellido1, string sApellido2) //Constructor para la tabla
        {

            CEDULA = sCedula;
            NOMBRES = sNombres;
            APELLIDO1 = sApellido1;
            APELLIDO2 = sApellido2;

        }

        public PADRON()//Constructor en blanco necesario para hacer querrys
        {

        }
    }
    public class COMPANIAS
    {
        [PrimaryKey]
        public int COMPANIA_ID { get; set; }

        public string NOMBRE { get; set; }


        public COMPANIAS(int COMPANIA_ID, string NOMBRE)
        {
            this.COMPANIA_ID = COMPANIA_ID;
            this.NOMBRE = NOMBRE;

        }

        public COMPANIAS()//Constructor en blanco necesario para hacer querrys
        {

        }
    }
    public class PLACA
    {
        [PrimaryKey]
        public string CODIGO { get; set; }

        public string PLACA_CODE { get; set; }


        public PLACA(string CODIGO, string PLACA_CODE)
        {
            this.CODIGO = CODIGO;
            this.PLACA_CODE = CODIGO;

        }

        public PLACA()//Constructor en blanco necesario para hacer querrys
        {

        }
    }
    public class PERSONAS
    {
        [PrimaryKey]
        public int PERSONA_ID { get; set; }
        public string NOMBRES_APELLIDOS { get; set; }

        public PERSONAS(int PERSONA_ID, string NOMBRES_APELLIDOS)
        {
            this.PERSONA_ID = PERSONA_ID;
            this.NOMBRES_APELLIDOS = NOMBRES_APELLIDOS;
        }

        public PERSONAS()//Constructor en blanco necesario para hacer querrys
        {

        }
    }
    public class VW_RESERVA_VISITA
    {
        // [PrimaryKey]
        public int VISITA_ID { get; set; }
        public int? VISITAS_VISITA_A_ID { get; set; }
        public string VISITAS_DOCUMENTO { get; set; }
        public string VISITAS_NOMBRE_COMPLETO { get; set; }
        public DateTime VISITAS_FECHA_VISITA_DESDE { get; set; }
        public DateTime VISITAS_FECHA_VISITA_HASTA { get; set; }
        public string EMPRESAS_NOMBRE { get; set; }
        public DateTime VISITAS_FECHA_RESERVA { get; set; }
        public int VISITAS_DEPARTAMENTO_ID { get; set; }
        public string DEPARTAMENTO_NOMBRE { get; set; }

        public VW_RESERVA_VISITA(int VISITA_ID, int VISITAS_VISITA_A_ID, string VISITAS_DOCUMENTO, string VISITAS_NOMBRE_COMPLETO, DateTime VISITAS_FECHA_VISITA_DESDE, DateTime VISITAS_FECHA_VISITA_HASTA, string EMPRESAS_NOMBRE, DateTime VISITAS_FECHA_RESERVA, int VISITAS_DEPARTAMENTO_ID, string DEPARTAMENTO_NOMBRE)
        {
            this.VISITA_ID = VISITA_ID;
            this.VISITAS_VISITA_A_ID = VISITAS_VISITA_A_ID;
            this.VISITAS_DOCUMENTO = VISITAS_DOCUMENTO;
            this.VISITAS_NOMBRE_COMPLETO = VISITAS_NOMBRE_COMPLETO;
            this.VISITAS_FECHA_VISITA_DESDE = VISITAS_FECHA_VISITA_DESDE;
            this.VISITAS_FECHA_VISITA_HASTA = VISITAS_FECHA_VISITA_HASTA;
            this.EMPRESAS_NOMBRE = EMPRESAS_NOMBRE;
            this.VISITAS_FECHA_RESERVA = VISITAS_FECHA_RESERVA;
            this.VISITAS_DEPARTAMENTO_ID = VISITAS_DEPARTAMENTO_ID;
            this.DEPARTAMENTO_NOMBRE = DEPARTAMENTO_NOMBRE;
        }

        public VW_RESERVA_VISITA()//Constructor en blanco necesario para hacer querrys
        {

        }
    }

    public class counterObj
    {
        public int anyCount { get; set; }
    }

    public partial class App : Application
    {
        public static System.Timers.Timer syncTimer;

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
                        string url = $"http://{Preferences.Get("REGISTROS_IP", "192.168.1.158") }:{Preferences.Get("REGISTROS_PORT", "4441")}/?sql=";
                        string querrySYNC = "SELECT FIRST 2 * FROM COMPANIAS";
                        var contentSync = _client.GetStringAsync(url + querrySYNC);
                        contentSync.Wait();
                        Preferences.Set("SYNC_VSU", contentSync.IsCompleted);
                        
                        
                      
                        //Servicios Periodicos
                        CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                        culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                        culture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
                        Thread.CurrentThread.CurrentCulture = culture;

                        var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));

                        //Vamos a subir las visitas que se agregaron.
                        var visitasASubir = db.Query<Invitados>("SELECT * FROM Invitados where SUBIDA is null");

                        //string url = $"http://{Preferences.Get("REGISTROS_IP", "192.168.1.158") }:{Preferences.Get("REGISTROS_PORT", "4441")}/?sql=";

                        Debug.WriteLine("There are: " + visitasASubir.Count.ToString() + " To upload");
                        foreach (Invitados registro in visitasASubir)
                        {
                            string querryInv;
                            string visitado;
                            string fechaSalida;
                            string placa;

                            if (registro.Visitado is null)
                            {
                                visitado = "null";
                            }
                            else
                            {
                                visitado = registro.Visitado.ToString();
                            }
                            if (registro.Fecha_Salida is null)
                            {
                                fechaSalida = "null";
                            }
                            else
                            {
                                fechaSalida = $"'{registro.Fecha_Salida.ToString()}'";
                            }
                            if (registro.Placa is null)
                            {
                                placa = "null";
                            }
                            else
                            {
                                placa = $"'{registro.Placa.ToString()}'";
                            }

                            querryInv = "SELECT IN_INVIDATO_ID as anyCount FROM INSERTAR_VISITAS(" +
                                $"{registro.Compania_ID.ToString()}, " +
                                $"'{registro.Nombres}', " +
                                $"'{ registro.Apellidos}', " +
                                $"'{registro.Fecha_Registro.ToString()}', " +
                                $"'{registro.Cargo}'," +
                                "0," +
                                "100," +
                                "1," +
                                $"{Util.CoalesceStr(registro.Empresa_ID, "null")}, " +
                                $"{Util.CoalesceStr(placa, "null")}," +
                                $"'{registro.Tipo_Visitante}'," +
                                "0," +
                                "0," +
                                $" {Util.CoalesceStr(registro.Puerta_Entrada, "1495")}," +
                                "0," +
                                "12," +
                                "1," +
                                "1," +
                                $"'{registro.Origen_Entrada}'," +
                                $"'{registro.Origen_Salida}'," +
                                "''," +
                                "0," +
                                "'I'," +
                                "0," +
                                "''," +
                                "''," +
                                "''," +
                                "1," +
                                "0," +
                                $" {Util.CoalesceStr(registro.Visitado, "null")}" +
                                $", {registro.Lector.ToString()}" +
                                $", {Util.CoalesceStr(fechaSalida, "null")})";

                            var content = _client.GetStringAsync(url + querryInv);
                            Debug.WriteLine("Waiting for: " + url + querryInv);
                            content.Wait();
                            if (content.IsCompleted & content.Result.Contains("ANYCOUNT"))
                            {
                                var respuesta = JsonConvert.DeserializeObject<List<counterObj>>(content.Result);
                                registro.INVIDATO_ID = respuesta.First().anyCount;
                                registro.Subida = true;
                                if (!(registro.Fecha_Salida is null))
                                {
                                    registro.salidaSubida = true;
                                }
                                Debug.WriteLine("Invitado subido: " + registro.INVIDATO_ID.ToString());
                            }
                            else
                            {
                                registro.Subida = null;
                                if (!(registro.Fecha_Salida is null))
                                {
                                    registro.salidaSubida = null;
                                }
                                try
                                {
                                    Analytics.TrackEvent("Error de SQL en el escaner: " + Preferences.Get("LECTOR", "N/A") + " Error: " + content.Result);
                                    Debug.WriteLine("Error de SQL: " + content.Result);
                                }
                                catch
                                {
                                    //No Hacer Nada
                                }
                            }


                        }
                        db.UpdateAll(visitasASubir);
                        //
                        var visitasASubir2 = db.Query<InvitadosReservas>("SELECT * FROM InvitadosReservas where SUBIDA is null");

                         url = $"http://{Preferences.Get("REGISTROS_IP", "192.168.1.103")}:{Preferences.Get("REGISTROS_PORT", "4440")}/?sql=";

                        Debug.WriteLine("There are: " + visitasASubir2.Count.ToString() + " To upload");
                        foreach (InvitadosReservas registro2 in visitasASubir2)
                        {
                            string querryInv2;
                            string visitado2;
                            string fechaSalida2;
                            string placa2;

                            if (registro2.Visitado is null)
                            {
                                visitado2 = "null";
                            }
                            else
                            {
                                visitado2 = registro2.Visitado.ToString();
                            }
                            if (registro2.Fecha_Salida is null)
                            {
                                fechaSalida2 = "null";
                            }
                            else
                            {
                                fechaSalida2 = "'" + registro2.Fecha_Salida.ToString() + "'";
                            }
                            if (registro2.Placa is null)
                            {
                                placa2 = "0000";
                            }
                            else
                            {
                                placa2 = "'" + registro2.Placa.ToString() + "'";
                            }

                            querryInv2 = "SELECT IN_INVIDATO_ID as anyCount FROM INSERTAR_VISITAS(" +
                                "" + registro2.Compania_ID.ToString() + ", " +
                                "'" + registro2.Nombres + "', " +
                                "'" + registro2.Apellidos + "', " +
                                "'" + registro2.Fecha_Registro.ToString() + "', " +
                                "'" + registro2.Cargo + "'," +
                                "0," +
                                "100, " +
                                "1, " +
                                $"{Util.Coalesce(registro2.Empresa_ID,"null")}, " +
                                $"{placa2}," +
                                $"'{registro2.Tipo_Visitante}'," +
                                "0, " +
                                "0, " +
                                $"{registro2.Puerta_Entrada}," +
                                "0, " +
                                "12, " +
                                "1, " +
                                "1, " +
                                "'MANUAL', " +
                                "'MANUAL', " +
                                "'PPOOII', " +
                                "0, " +
                                "'I', " +
                                "0, " +
                                "'', " +
                                "'', " +
                                "'', " +
                                "1, " +
                                "0, " +
                                $"{visitado2}, " +
                                $"{registro2.Lector.ToString()}, " +
                                $"{fechaSalida2})";


                            var content = _client.GetStringAsync(url + querryInv2);
                            Debug.WriteLine("Waiting for: " + url + querryInv2);
                            content.Wait();
                            if (content.IsCompleted & content.Result.Contains("ANYCOUNT"))
                            {
                                var respuesta = JsonConvert.DeserializeObject<List<counterObj>>(content.Result);
                                registro2.INVIDATO_ID = respuesta.First().anyCount;
                                registro2.Subida = true;
                                if (!(registro2.Fecha_Salida is null))
                                {
                                    registro2.salidaSubida = true;
                                }
                                Debug.WriteLine("Invitado subido Reserva: " + registro2.INVIDATO_ID.ToString());
                            }
                            else
                            {
                                registro2.Subida = null;
                                if (!(registro2.Fecha_Salida is null))
                                {
                                    registro2.salidaSubida = null;
                                }
                                try
                                {
                                    Analytics.TrackEvent("Error de SQL en el escaner: " + Preferences.Get("LECTOR", "N/A") + " Error: " + content.Result);
                                    Debug.WriteLine("Error de SQL: " + content.Result);
                                }
                                catch
                                {
                                    //No Hacer Nada
                                }
                            }


                        }
                        db.UpdateAll(visitasASubir2);
                        //
                        //Vamos a subir las Verificacion.
                        var verificarASubir = db.Query<Invitados>("SELECT * FROM Invitados where VERIFICACIONSUBIDA is null and Fecha_Verificacion is not null and SUBIDA is not null");
                        foreach (Invitados registro in verificarASubir)
                        {
                            var querryVer = "SELECT * FROM SP_DAR_VERIFICACION(" + registro.INVIDATO_ID.ToString() + ", " + registro.Puerta_Registro.ToString() + ", '" +
                            registro.Fecha_Verificacion.ToString() + "')";

                            var content = _client.GetStringAsync(url + querryVer);
                            Debug.WriteLine("Waiting for: " + url + querryVer);
                            content.Wait();
                            if (content.IsCompleted & content.Result.Contains("OK_ACK"))
                            {
                                registro.verificacionSubida = true;
                                Debug.WriteLine("Verificacion subida: ID" + registro.INVIDATO_ID.ToString() + " " + registro.Fecha_Verificacion.ToString());
                            }
                            else
                            {
                                registro.verificacionSubida = null;
                                try
                                {
                                    Analytics.TrackEvent("Error de SQL en el escaner: " + Preferences.Get("LECTOR", "N/A") + " Error: " + content.Result);
                                    Debug.WriteLine("Error de SQL: " + content.Result);
                                }
                                catch
                                {
                                    //No Hacer Nada
                                }
                            }
                        }
                        db.UpdateAll(verificarASubir);

                        //Vamos a subir las salidas.
                        var salidasASubir = db.Query<Invitados>("SELECT * FROM Invitados where SALIDASUBIDA is null and FECHA_SALIDA is not null and SUBIDA is not null");
                        foreach (Invitados registro in salidasASubir)
                        {
                            var querrySal = "SELECT * FROM SP_DAR_SALIDA(" + registro.INVIDATO_ID.ToString() + ", '" + registro.Fecha_Salida.ToString() + "', " +
                            Preferences.Get("LECTOR", "1") + ")";

                            var content = _client.GetStringAsync(url + querrySal);
                            Debug.WriteLine("Waiting for: " + url + querrySal);
                            content.Wait();
                            if (content.IsCompleted & content.Result.Contains("OK_ACK"))
                            {
                                registro.salidaSubida = true;
                                Debug.WriteLine("Salida subida: ID" + registro.INVIDATO_ID.ToString() + " " + registro.Fecha_Salida.ToString());
                            }
                            else
                            {
                                registro.salidaSubida = null;
                                try
                                {
                                    Analytics.TrackEvent("Error de SQL en el escaner: " + Preferences.Get("LECTOR", "N/A") + " Error: " + content.Result);
                                    Debug.WriteLine("Error de SQL: " + content.Result);
                                }
                                catch
                                {
                                    //No Hacer Nada
                                }
                            }
                        }
                        db.UpdateAll(salidasASubir);
                        //Vamos a subir las salidasDesconocidas.
                        var salidasOFF = db.Query<SalidaOffline>("SELECT * FROM SalidaOffline where SUBIDA is null");
                        foreach (SalidaOffline registros in salidasOFF)
                        {
                            var querrySalOFF = "SELECT * FROM SP_DAR_SALIDA(" + registros.INVIDATO_ID + ", '" + registros.Fecha_Salida + "', " +
                            Preferences.Get("LECTOR", "1") + ")";

                            var content = _client.GetStringAsync(url + querrySalOFF);
                            Debug.WriteLine("Waiting for: " + url + querrySalOFF);
                            content.Wait();
                            if (content.IsCompleted & content.Result.Contains("OK_ACK"))
                            {
                                registros.Subida = true;
                                Debug.WriteLine("Salida subida: ID" + registros.INVIDATO_ID.ToString() + " " + registros.Fecha_Salida.ToString());
                            }
                            else
                            {
                                registros.Subida = null;
                                try
                                {
                                    Analytics.TrackEvent("Error de SQL en el escaner: " + Preferences.Get("LECTOR", "N/A") + " Error: " + content.Result);
                                    Debug.WriteLine("Error de SQL: " + content.Result);
                                }
                                catch
                                {
                                    //No Hacer Nada
                                }
                            }
                        }
                        db.UpdateAll(salidasOFF);
                        //Vamos a descargar las reservas.

                        string querry = "SELECT FIRST " + Preferences.Get("CHUNK_SIZE", "10000") + " VISITA_ID,VISITAS_VISITA_A_ID, VISITAS_DOCUMENTO" +
                        ", VISITAS_NOMBRE_COMPLETO, CAST(VISITAS_FECHA_VISITA_DESDE as VARCHAR(25)) as VISITAS_FECHA_VISITA_DESDE" +
                        ", CAST(VISITAS_FECHA_VISITA_HASTA as VARCHAR(25)) as VISITAS_FECHA_VISITA_HASTA, EMPRESAS_NOMBRE" +
                        ", CAST(VISITAS_FECHA_RESERVA as VARCHAR(25)) as VISITAS_FECHA_RESERVA, " + " VISITAS_DEPARTAMENTO_ID, " + "DEPARTAMENTO_NOMBRE " +
                        "FROM vw_reserva_visita where VISITA_ID > " + Preferences.Get("MAX_RESERVA_ID", "0") + " ORDER BY VISITA_ID desc";
                        var contentRes = _client.GetStringAsync(url + querry);
                        contentRes.Wait();
                        if (contentRes.IsCompleted)
                        {
                            var listReserva = JsonConvert.DeserializeObject<List<VW_RESERVA_VISITA>>(contentRes.Result).ToList();

                            try
                            {
                                if (listReserva.Any())
                                {
                                    db.InsertAll(listReserva);
                                    Debug.WriteLine("MAX_RESERVA_ID: " + listReserva.First().VISITA_ID.ToString());
                                    Preferences.Set("MAX_RESERVA_ID", listReserva.First().VISITA_ID.ToString());
                                    Debug.WriteLine("Reservas Descargadas: " + DateTime.Now);
                                }
                            }
                            catch (Exception ex)
                            {
                                var properties = new Dictionary<string, string> {
                                            { "Category", "Error insertando reservas" },
                                            { "Code", "App.xaml.cs Line: 472" },
                                            { "Lector", Preferences.Get("LECTOR", "N/A")}
                                        };
                                Debug.WriteLine("Excepcion insertando reservas: " + ex.ToString());
                                Crashes.TrackError(ex, properties);
                            }

                        }
                        else
                        {
                            try
                            {
                                Analytics.TrackEvent("Error de SQL en el escaner: " + Preferences.Get("LECTOR", "N/A") + " Error: " + contentRes.Result);
                                Debug.WriteLine("Error de SQL: " + contentRes.Result);
                            }
                            catch
                            {
                                //No Hacer Nada
                            }
                        }
                        //Vamos a descargar las companias
                        string querryCompanias = " SELECT FIRST " 
                                               + Preferences.Get("CHUNK_SIZE", "10000") 
                                               + " COMPANIA_ID, NOMBRE  FROM COMPANIAS where COMPANIA_ID > " 
                                               + Preferences.Get("MAX_COMPANIA_ID", "0")
                                               + " ORDER BY COMPANIA_ID desc";
                        var contentCompanias = _client.GetStringAsync(url + querryCompanias);
                        contentCompanias.Wait();
                        var stlRegistros = new List<string>();
                        var stRegisros = "";
                        if (contentCompanias.IsCompleted)
                        {
                            var listCompanias = JsonConvert.DeserializeObject<List<COMPANIAS>>(contentCompanias.Result).ToList();

                            try
                            {
                                if (listCompanias.Any())
                                {
                                    db.InsertAll(listCompanias);
                                    Debug.WriteLine("MAX_COMPANIA_ID: " + listCompanias.First().COMPANIA_ID.ToString());
                                    Preferences.Set("MAX_COMPANIA_ID", listCompanias.First().COMPANIA_ID.ToString());
                                    Debug.WriteLine("Companias Descargadas: " + DateTime.Now);
                                    string sortNames = "select nombre from companias order by nombre";
                                    var Sorting = db.Query<COMPANIAS>(sortNames);
                                    foreach (COMPANIAS registro in Sorting)
                                    {
                                        //db.Insert(registro);
                                        Debug.WriteLine("EMPRESAS: " + registro.NOMBRE);
                                        stlRegistros.Add(registro.NOMBRE.ToString());
                                        stRegisros = stRegisros + "," + registro.NOMBRE.ToString();
                                    }
                                    stRegisros = stRegisros.TrimStart(',');
                                    Preferences.Set("COMPANIAS_LIST", stRegisros);
                                }
                            }
                            catch (Exception ex)
                            {
                                var properties = new Dictionary<string, string> {
                                            { "Category", "Error insertando Companias" },
                                            { "Code", "App.xaml.cs Line: 516" },
                                            { "Lector", Preferences.Get("LECTOR", "N/A")}
                                        };
                                Debug.WriteLine("Excepcion insertando Companias: " + ex.ToString());
                                Crashes.TrackError(ex, properties);
                            }

                        }
                        else
                        {
                            try
                            {
                                Analytics.TrackEvent("Error de SQL en el escaner: " + Preferences.Get("LECTOR", "N/A") + " Error: " + contentCompanias.Result);
                                Debug.WriteLine("Error de SQL: " + contentCompanias.Result);
                            }
                            catch
                            {
                                //No Hacer Nada
                            }
                        }
                        //Vamos a descargar las personas(destinos)
                        var stlRegistros2 = new List<string>();
                         var stRegisros2 = "";
                        string querryPersonas = " SELECT FIRST " + Preferences.Get("CHUNK_SIZE", "10000") + " PERSONA_ID, NOMBRES_APELLIDOS FROM PERSONAS where TIPO=1 AND PERSONA_ID > " + Preferences.Get("MAX_PERSONA_ID", "0") + " ORDER BY PERSONA_ID desc";
                        var contentPersonas= _client.GetStringAsync(url + querryPersonas);
                        contentPersonas.Wait();
                        if (contentPersonas.IsCompleted)
                        {
                            var listPersonas = JsonConvert.DeserializeObject<List<PERSONAS>>(contentPersonas.Result).ToList();

                            try
                            {
                                if (listPersonas.Any())
                                {
                                    db.InsertAll(listPersonas);
                                    Debug.WriteLine("MAX_PERSONA_ID: " + listPersonas.First().PERSONA_ID.ToString());
                                    Preferences.Set("MAX_PERSONA_ID", listPersonas.First().PERSONA_ID.ToString());
                                    Debug.WriteLine("Personas Descargadas: " + DateTime.Now);

                                    string sortPersons = "select nombres_apellidos from personas order by nombres_apellidos";
                                    var Sorting = db.Query<PERSONAS>(sortPersons);
                                    foreach (var registro in Sorting)
                                    {
                                        if (registro.NOMBRES_APELLIDOS != null )
                                        {
                                            //db.Insert(registro);
                                            Debug.WriteLine("Personas: " + registro.NOMBRES_APELLIDOS);
                                            stlRegistros2.Add(registro.NOMBRES_APELLIDOS.ToString());
                                            stRegisros2 = stRegisros2 + "," + registro.NOMBRES_APELLIDOS.ToString();
                                        }
                                        
                                    }
                                    stRegisros2 = stRegisros2.TrimStart(',');
                                    Preferences.Set("PERSONAS_LIST", stRegisros2);

                                }
                            }
                            catch (Exception ex)
                            {
                                var properties = new Dictionary<string, string> {
                                            { "Category", "Error insertando Personas" },
                                            { "Code", "App.xaml.cs Line: 243" },
                                            { "Lector", Preferences.Get("LECTOR", "N/A")}
                                        };
                                Debug.WriteLine("Excepcion insertando Personas: " + ex.ToString());
                                Crashes.TrackError(ex, properties);
                            }

                        }
                        else
                        {
                            try
                            {
                                Analytics.TrackEvent("Error de SQL en el escaner: " + Preferences.Get("LECTOR", "N/A") + " Error: " + contentPersonas.Result);
                                Debug.WriteLine("Error de SQL: " + contentPersonas.Result);
                            }
                            catch
                            {
                                //No Hacer Nada
                            }
                        }
                        //Vamos a descargar los Invitados.
                        //string querryDownInv = "SELECT FIRST " + Preferences.Get("CHUNK_SIZE", "10000") + " INVIDATO_ID, 1 as SUBIDA, IIF(FECHA_SALIDA is null, 0,1) " +
                        //"as SALIDASUBIDA, COMPANIA_ID, NOMBRES, APELLIDOS, FECHA_REGISTRO, FECHA_SALIDA, TIPO, CARGO, TIENE_ACTIVO, ESTATUS_ID, MODULO, EMPRESA_ID, " +
                        //"PLACA, TIPO_VISITANTE, ES_GRUPO, GRUPO_ID, PUERTA_ENTRADA, ACTUALIZADA_LA_SALIDA, HORAS_CADUCIDAD, PERSONAS, IN_OUT, ORIGEN_ENTRADA, " +
                        //"ORIGEN_SALIDA, COMENTARIO, ORIGEN_IO, ACTUALIZADO, CPOST, TEXTO1_ENTRADA, TEXTO2_ENTRADA, TEXTO3_ENTRADA, SECUENCIA_DIA, NO_APLICA_INDUCCION, " +
                        //"VISITADO, COALESCE(LECTOR, 0) AS LECTOR FROM INVITADOS WHERE INVIDATO_ID > " + Preferences.Get("MAX_INVIDATO_ID", "0") +
                        //" AND COALESCE(LECTOR, 0) <> " + Preferences.Get("LECTOR", "1") + " ORDER BY INVIDATO_ID DESC";

                        string querryDownInv = $"SELECT FIRST {Preferences.Get("CHUNK_SIZE", "10000")} I1.INVIDATO_ID, 1 AS SUBIDA, IIF(I1.FECHA_SALIDA IS NULL, 0, 1) AS SALIDASUBIDA, I1.COMPANIA_ID" +
                        ", I1.NOMBRES, I1.APELLIDOS, I1.FECHA_REGISTRO, I1.FECHA_SALIDA, I1.TIPO, I1.CARGO, I1.TIENE_ACTIVO, I1.ESTATUS_ID, I1.MODULO" +
                        ", I1.EMPRESA_ID, I1.PLACA, I1.TIPO_VISITANTE, I1.ES_GRUPO, I1.GRUPO_ID, I1.PUERTA_ENTRADA, I1.ACTUALIZADA_LA_SALIDA" +
                        ", COALESCE(I1.HORAS_CADUCIDAD,0) AS HORAS_CADUCIDAD, I1.PERSONAS, I1.IN_OUT, I1.ORIGEN_ENTRADA, I1.ORIGEN_SALIDA, I1.COMENTARIO, COALESCE(I1.ORIGEN_IO,0) AS ORIGEN_IO, I1.ACTUALIZADO" +
                        ", I1.CPOST, I1.TEXTO1_ENTRADA, I1.TEXTO2_ENTRADA, I1.TEXTO3_ENTRADA, I1.SECUENCIA_DIA, I1.NO_APLICA_INDUCCION, I1.VISITADO" +
                        ", COALESCE(I1.LECTOR, 0) AS LECTOR FROM INVITADOS I1 INNER JOIN(SELECT MAX(I2.INVIDATO_ID) AS INVIDATO_ID FROM INVITADOS I2" +
                        $" WHERE INVIDATO_ID > {Preferences.Get("MAX_INVIDATO_ID", "0")} AND COALESCE(LECTOR, 0) <> {Preferences.Get("LECTOR", "1")} " + 
                        "GROUP BY I2.CARGO) I3 ON I1.invidato_id = I3.INVIDATO_ID ORDER BY I1.INVIDATO_ID DESC";
                        var contentDownInv = _client.GetStringAsync(url + querryDownInv);
                        contentDownInv.Wait();
                        if (contentDownInv.IsCompleted)
                        {
                            var listInvitados = JsonConvert.DeserializeObject<List<Invitados>>(contentDownInv.Result).ToList();

                            try
                            {
                                if (listInvitados.Any())
                                {
                                    Debug.WriteLine("Se va a descargar: " + listInvitados.Count().ToString() + " Visitas");
                                    db.InsertAll(listInvitados);
                                    Debug.WriteLine("MAX_INVIDATO_ID: " + listInvitados.First().INVIDATO_ID.ToString());
                                    Preferences.Set("MAX_INVIDATO_ID", listInvitados.First().INVIDATO_ID.ToString());
                                    Debug.WriteLine("Visitas Descargadas: " + DateTime.Now);
                                }
                            }
                            catch (Exception ex)
                            {
                                var properties = new Dictionary<string, string> {
                                            { "Category", "Error insertando reservas" },
                                            { "Code", "App.xaml.cs Line: 631" },
                                            { "Lector", Preferences.Get("LECTOR", "N/A")}
                                        };
                                Debug.WriteLine("Excepcion insetando reservas: " + ex.ToString());
                                Crashes.TrackError(ex, properties);
                            }

                        }
                        else
                        {
                            try
                            {
                                Analytics.TrackEvent("Error de SQL en el escaner: " + Preferences.Get("LECTOR", "N/A") + " Error: " + contentRes.Result);
                                Debug.WriteLine("Error de SQL: " + contentRes.Result);
                            }
                            catch
                            {
                                //No Hacer Nada
                            }
                        }

                        var maxInvidatoIdLocal = db.Query<counterObj>("SELECT MAX(INVIDATO_ID) as anycount FROM Invitados");

                        //Vamos a descargar las salidas.
                        var querryDownSal = "SELECT FIRST " + Preferences.Get("CHUNK_SIZE", "10000") + " INVIDATO_ID, 1 as SUBIDA, 1 as SALIDASUBIDA, COMPANIA_ID, NOMBRES, APELLIDOS, " +
                        "FECHA_REGISTRO, FECHA_SALIDA, TIPO, CARGO, TIENE_ACTIVO, ESTATUS_ID, MODULO, EMPRESA_ID, PLACA, TIPO_VISITANTE, ES_GRUPO, GRUPO_ID, " +
                        "PUERTA_ENTRADA, ACTUALIZADA_LA_SALIDA, HORAS_CADUCIDAD, PERSONAS, IN_OUT, ORIGEN_ENTRADA, ORIGEN_SALIDA, COMENTARIO, ORIGEN_IO, " +
                        "ACTUALIZADO, CPOST, TEXTO1_ENTRADA, TEXTO2_ENTRADA, TEXTO3_ENTRADA, SECUENCIA_DIA, NO_APLICA_INDUCCION, VISITADO, COALESCE(LECTOR, 0) AS LECTOR, SALIDA_ID " +
                        "FROM INVITADOS WHERE SALIDA_ID > " + Preferences.Get("MAX_SALIDA_ID", "0") + " AND INVIDATO_ID <= " + maxInvidatoIdLocal.First().anyCount.ToString() +
                        " AND SALIDA_ID IS NOT NULL AND COALESCE(LECTOR_SALIDA,0) <> " + Preferences.Get("LECTOR", "1") +
                        " ORDER BY SALIDA_ID DESC";
                        var contentDownSal = _client.GetStringAsync(url + querryDownSal);
                        contentDownSal.Wait();
                        if (contentDownSal.IsCompleted)
                        {
                            var listSalidas = JsonConvert.DeserializeObject<List<Invitados>>(contentDownSal.Result);

                            try
                            {
                                if (listSalidas.Any())
                                {
                                    foreach (Invitados registro in listSalidas)
                                    {
                                        var invitadoId = db.Query<counterObj>("SELECT INVITADO_ID as anycount FROM Invitados WHERE INVIDATO_ID = " + registro.INVIDATO_ID.ToString());
                                        if (invitadoId.Any())
                                        {
                                            registro.INVITADO_ID = invitadoId.First().anyCount;
                                        }
                                    }
                                    Debug.WriteLine("Se va a descargar: " + listSalidas.Count().ToString() + " Salidas");
                                    db.UpdateAll(listSalidas);
                                    Debug.WriteLine("MAX_SALIDA_ID: " + listSalidas.First().SALIDA_ID.ToString());
                                    Preferences.Set("MAX_SALIDA_ID", listSalidas.First().SALIDA_ID.ToString());
                                    Debug.WriteLine("Salidas Descargadas: " + DateTime.Now);
                                }
                            }
                            catch (Exception ex)
                            {
                                var properties = new Dictionary<string, string> {
                                            { "Category", "Error descargando Salidas" },
                                            { "Code", "App.xaml.cs Line: 683" },
                                            { "Lector", Preferences.Get("LECTOR", "N/A")}
                                        };
                                Debug.WriteLine("Excepcion descargando Salidas: " + ex.ToString());
                                Crashes.TrackError(ex, properties);
                            }

                        }
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