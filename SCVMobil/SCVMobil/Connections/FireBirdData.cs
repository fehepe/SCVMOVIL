using FirebirdSql.Data.FirebirdClient;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SCVMobil.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms.PlatformConfiguration;

namespace SCVMobil.Connections
{
    public class FireBirdData
    {
        //HttpClient _client = new HttpClient();

        //// Constructor
        public FireBirdData()
        {

        }


        //// Retornar el Connection String
        public string connectionString(bool db)
        {
            if (db)
            {
                string connectionString = "User ID = sysdba; Password = masterkey; Database = C:\\APP\\GAD\\registros.fdb; " +
                                          $"DataSource={Preferences.Get("SERVER_IP", "192.168.1.103")};Port=3050;Charset=NONE;Server Type=0;";

                //string connectionString = "User ID=sysdba;Password=masterkey;Database=C:\\Users\\Abraham\\Desktop\\Codes\\registros\\registros.fdb;" +
                //                           $"DataSource={Preferences.Get("SERVER_IP", "192.168.2.120")};Port=3050;Charset=NONE;Server Type=0; Timeout=5;"; //Connectionstring//

                //string connectionString = "User ID=sysdba;Password=masterkey;Database=C:\\APP\\registros\\registros.fdb;" +
                //                          $"DataSource={Preferences.Get("SERVER_IP", "192.168.2.120")};Port=3050;Charset=NONE;Server Type=0; Timeout=5;"; //Connectionstring//

                return connectionString;
            }
            else
            {

                string connectionString = "User ID=sysdba;Password=masterkey;" +
                          "Database=C:\\APP\\GAD\\datos_214.fdb;" +
                          $"DataSource={Preferences.Get("SERVER_IP", "localhost")};Port=3050;Charset=NONE;Server Type=0;";
                return connectionString;
            }
        }


        //// Ejecutar query Scalar luego de abrir una conexion con la base de datos
        public string ExecuteScalar(string query)
        {
            try
            {
                string _dtResult = "";
                FbConnection fb = new FbConnection(connectionString(true));
                using (FbCommand command = new FbCommand(query, fb))
                {
                    fb.Open();
                    _dtResult = command.ExecuteScalar().ToString();
                }
                fb.Close();
                fb.Dispose();

                Preferences.Set("SYNC_VSU", true);

                return _dtResult;
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en el metodo ExecuteScalar, generado por: " + ea.Message);
                Preferences.Set("SYNC_VSU", false);
                return null;
            }

        }

        //// Descargar Padron
        public List<PADRON> DownloadPadron(string querry,bool tipo)
        {
            try
            {
                List<PADRON> listPadron = new List<PADRON>();
                int triesTBL = 0;

                do
                {
                    try
                    {
                        FbConnection fb = new FbConnection(connectionString(tipo));

                        fb.Open();
                        FbCommand command = new FbCommand(
                            querry,
                            fb);


                        FbDataReader dtResult = command.ExecuteReader();

                        if (dtResult.HasRows)
                        {
                            while (dtResult.Read())
                            {
                                PADRON persona = new PADRON();

                                if (dtResult[0] != System.DBNull.Value)
                                {
                                    persona.CEDULA = dtResult[0].ToString();
                                }
                                if (dtResult[1] != System.DBNull.Value)
                                {
                                    persona.NOMBRES = dtResult[1].ToString();
                                }
                                if (dtResult[2] != System.DBNull.Value)
                                {
                                    persona.APELLIDO1 = dtResult[2].ToString();
                                }
                                if (dtResult[3] != System.DBNull.Value)
                                {
                                    persona.APELLIDO2 = dtResult[3].ToString();
                                }
                                listPadron.Add(persona);
                                //Debug.WriteLine("Agregado, NOMBRE: "+persona.NOMBRES+" "+persona.APELLIDO1 + " CEDULA: "+persona.CEDULA);
                            }
                        }
                        else
                        {
                            Debug.WriteLine("No rows found.");
                        }
                        dtResult.Close();

                        fb.Close();
                        Debug.WriteLine($"La lista retornada contiene {listPadron.Count} elementos");
                        return listPadron;
                    }
                    catch (Exception et)
                    {
                        Debug.WriteLine("Error en Sycn" + et.Message);
                        Analytics.TrackEvent("Error descargando padron:  " + et.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));

                        Debug.WriteLine("No se pudo conectar con la base de datos: " + et.Message);
                        triesTBL += 1;

                        return null;

                    }
                } while (triesTBL <= 5);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al descagar el padron, provocado por: " + ex.Message);
                return null;
            }
        }


        public int ReturnCountCedulas(int value)
        {
            try
            {
                string querry = "select count(p.cedula) as anyCount from padron p";
                //string contenido;
                int maxRegistro = 0;

                int continuar = value;
                if (continuar <= 5)
                {
                    try
                    {
                        FbConnection fb = new FbConnection(connectionString(false));

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
                        return maxRegistro;
                    }
                    catch (Exception ex)
                    {
                        Analytics.TrackEvent("Error al conectarse a base de datos " + ex.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
                        Debug.WriteLine("Error en Sync");
                        var x = ex.Message;
                        continuar += 1;
                        if (continuar >= 5)
                        {
                            return 0;
                        }
                        else
                        {
                            continuar += 1;
                            return ReturnCountCedulas(continuar);
                        }
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en el metodo CountCedulas, provocado por: " + ex);
                return 0;
            }
        }

        // Retornar una lista de invitados
        public List<Invitados> ExecuteGuest(string query)
        {
            try
            {
                List<Invitados> GuestsList = new List<Invitados>();

                FbConnection fb = new FbConnection(connectionString(true));

                fb.Open();
                FbCommand command = new FbCommand(
                    query,
                    fb);

                var dtResult = command.ExecuteReader();

                if (dtResult.HasRows)
                {
                    while (dtResult.Read())
                    {
                        Invitados invitado = new Invitados();
                        #region Verificar que los valores no sean nulos antes de convertirlos, de lo contrario asignar nulo por defecto
                        if (dtResult[0] != System.DBNull.Value)
                        {
                            invitado.INVIDATO_ID = Convert.ToInt32(dtResult[0]);
                        }
                        else
                        {
                            continue;
                        }
                        if (dtResult[1] != System.DBNull.Value)
                        {
                            invitado.Subida = Convert.ToBoolean(dtResult[1]);
                        }
                        if (dtResult[2] != System.DBNull.Value)
                        {
                            invitado.salidaSubida = Convert.ToBoolean(dtResult[2]);
                        }
                        if (dtResult[3] != System.DBNull.Value)
                        {
                            invitado.Compania_ID = Convert.ToInt32(dtResult[3]);

                        }
                        if (dtResult[4] != System.DBNull.Value)
                        {
                            var prueba = dtResult[4].ToString();
                            invitado.Nombres = dtResult[4].ToString();
                        }
                        if (dtResult[5] != System.DBNull.Value)
                        {
                            invitado.Apellidos = dtResult[5].ToString();
                        }
                        if (dtResult[6] != System.DBNull.Value)
                        {
                            invitado.Fecha_Registro = Convert.ToDateTime(dtResult[6]);
                        }
                        if (dtResult[7] != System.DBNull.Value)
                        {
                            invitado.Fecha_Salida = Convert.ToDateTime(dtResult[7]);
                        }
                        if (dtResult[8] != System.DBNull.Value)
                        {
                            invitado.Tipo = dtResult[8].ToString();
                        }
                        if (dtResult[9] != System.DBNull.Value)
                        {
                            invitado.Cargo = dtResult[9].ToString();
                        }
                        if (dtResult[10] != System.DBNull.Value)
                        {
                            invitado.Tiene_Activo = Convert.ToInt32(dtResult[10]);
                        }
                        if (dtResult[11] != System.DBNull.Value)
                        {
                            invitado.Estatus_ID = Convert.ToInt32(dtResult[11]);
                        }
                        if (dtResult[12] != System.DBNull.Value)
                        {
                            invitado.Modulo = Convert.ToInt32(dtResult[12]);
                        }
                        if (dtResult[13] != System.DBNull.Value)
                        {
                            invitado.Empresa_ID = Convert.ToInt32(dtResult[13]);
                        }
                        if (dtResult[14] != System.DBNull.Value)
                        {
                            invitado.Placa = dtResult[14].ToString();
                        }
                        if (dtResult[15] != System.DBNull.Value)
                        {
                            invitado.Tipo_Visitante = dtResult[15].ToString();
                        }
                        if (dtResult[16] != System.DBNull.Value)
                        {
                            invitado.Es_Grupo = Convert.ToInt32(dtResult[16]);
                        }
                        if (dtResult[17] != System.DBNull.Value)
                        {
                            invitado.Grupo_ID = Convert.ToInt32(dtResult[17]);
                        }
                        if (dtResult[18] != System.DBNull.Value)
                        {
                            invitado.Puerta_Entrada = Convert.ToInt32(dtResult[18]);
                        }
                        if (dtResult[19] != System.DBNull.Value)
                        {
                            invitado.Actualizada_La_Salida = Convert.ToInt32(dtResult[19]);
                        }
                        if (dtResult[20] != System.DBNull.Value)
                        {
                            invitado.Horas_Caducidad = Convert.ToInt32(dtResult[20]);
                        }
                        if (dtResult[21] != System.DBNull.Value)
                        {
                            invitado.Personas = Convert.ToInt32(dtResult[21]);
                        }
                        if (dtResult[22] != System.DBNull.Value)
                        {
                            invitado.In_Out = Convert.ToInt32(dtResult[22]);
                        }
                        if (dtResult[23] != System.DBNull.Value)
                        {
                            invitado.Origen_Entrada = dtResult[23].ToString(); ;
                        }
                        if (dtResult[24] != System.DBNull.Value)
                        {
                            invitado.Origen_Salida = dtResult[24].ToString();
                        }
                        if (dtResult[25] != System.DBNull.Value)
                        {
                            invitado.Comentario = dtResult[25].ToString();
                        }
                        if (dtResult[26] != System.DBNull.Value)
                        {
                            invitado.Origen_IO = Convert.ToInt32(dtResult[26]);
                        }
                        if (dtResult[27] != System.DBNull.Value)
                        {
                            invitado.Actualizado = Convert.ToInt32(dtResult[27]);
                        }
                        if (dtResult[28] != System.DBNull.Value)
                        {
                            invitado.Cpost = dtResult[28].ToString();
                        }
                        if (dtResult[29] != System.DBNull.Value)
                        {
                            invitado.Texto1_Entrada = dtResult[29].ToString();
                        }
                        if (dtResult[30] != System.DBNull.Value)
                        {
                            invitado.Texto2_Entrada = dtResult[30].ToString();
                        }
                        if (dtResult[31] != System.DBNull.Value)
                        {
                            invitado.Texto3_Entrada = dtResult[31].ToString();
                        }
                        if (dtResult[32] != System.DBNull.Value)
                        {
                            invitado.Secuencia_Dia = dtResult[32].ToString(); ;
                        }
                        if (dtResult[33] != System.DBNull.Value)
                        {
                            invitado.No_Aplica_Induccion = dtResult[33].ToString();
                        }
                        if (dtResult[34] != System.DBNull.Value)
                        {
                            invitado.Visitado = Convert.ToInt32(dtResult[34]);
                        }
                        if (dtResult[35] != System.DBNull.Value)
                        {
                            invitado.Lector = Convert.ToInt32(dtResult[35]);
                        }
                        if(dtResult[36] != System.DBNull.Value)
                        {
                            invitado.Codigo_carnet = Convert.ToString(dtResult[36]);
                        }
                        #endregion

                        GuestsList.Add(invitado);
                        Debug.WriteLine("Usuario agregado, Id: " + invitado.INVIDATO_ID);
                    }
                }
                dtResult.Close();
                fb.Close();
                fb.Dispose();
                Preferences.Set("SYNC_VSU", true);
                return GuestsList;
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en el metodo ExecuteGuest, provocado por: " + ea.Message);
                Preferences.Set("SYNC_VSU", false);
                return null;
            }
        }


        // Retornar una lista de invitados
        public List<Invitados> ExecuteGuestOuts(string query)
        {
            try
            {
                List<Invitados> GuestsList = new List<Invitados>();

                FbConnection fb = new FbConnection(connectionString(true));

                fb.Open();

                FbCommand command = new FbCommand(
                    query,
                    fb);
                var dtResult = command.ExecuteReader();

                if (dtResult.HasRows)
                {
                    while (dtResult.Read())
                    {
                        Invitados invitado = new Invitados();

                        #region Verificar que los valores no sean nulos antes de convertirlos, de lo contrario asignar nulo por defecto
                        if (dtResult[0] != System.DBNull.Value)
                        {
                            invitado.INVIDATO_ID = Convert.ToInt32(dtResult[0]);
                        }
                        else
                        {
                            continue;
                        }
                        if (dtResult[1] != System.DBNull.Value)
                        {
                            invitado.Subida = Convert.ToBoolean(dtResult[1]);
                        }
                        if (dtResult[2] != System.DBNull.Value)
                        {
                            invitado.salidaSubida = Convert.ToBoolean(dtResult[2]);
                        }
                        if (dtResult[3] != System.DBNull.Value)
                        {
                            invitado.Compania_ID = Convert.ToInt32(dtResult[3]);

                        }
                        if (dtResult[4] != System.DBNull.Value)
                        {
                            invitado.Nombres = dtResult[4].ToString();
                        }
                        if (dtResult[5] != System.DBNull.Value)
                        {
                            invitado.Apellidos = dtResult[5].ToString();
                        }
                        if (dtResult[6] != System.DBNull.Value)
                        {
                            invitado.Fecha_Registro = Convert.ToDateTime(dtResult[6]);
                        }
                        if (dtResult[7] != System.DBNull.Value)
                        {
                            invitado.Fecha_Salida = Convert.ToDateTime(dtResult[7]);
                        }
                        if (dtResult[8] != System.DBNull.Value)
                        {
                            invitado.Tipo = dtResult[8].ToString();
                        }
                        if (dtResult[9] != System.DBNull.Value)
                        {
                            invitado.Cargo = dtResult[9].ToString();
                        }
                        if (dtResult[10] != System.DBNull.Value)
                        {
                            invitado.Tiene_Activo = Convert.ToInt32(dtResult[10]);
                        }
                        if (dtResult[11] != System.DBNull.Value)
                        {
                            invitado.Estatus_ID = Convert.ToInt32(dtResult[11]);
                        }
                        if (dtResult[12] != System.DBNull.Value)
                        {
                            invitado.Modulo = Convert.ToInt32(dtResult[12]);
                        }
                        if (dtResult[13] != System.DBNull.Value)
                        {
                            invitado.Empresa_ID = Convert.ToInt32(dtResult[13]);
                        }
                        if (dtResult[14] != System.DBNull.Value)
                        {
                            invitado.Placa = dtResult[14].ToString();
                        }
                        if (dtResult[15] != System.DBNull.Value)
                        {
                            invitado.Tipo_Visitante = dtResult[15].ToString();
                        }
                        if (dtResult[16] != System.DBNull.Value)
                        {
                            invitado.Es_Grupo = Convert.ToInt32(dtResult[16]);
                        }
                        if (dtResult[17] != System.DBNull.Value)
                        {
                            invitado.Grupo_ID = Convert.ToInt32(dtResult[17]);
                        }
                        if (dtResult[18] != System.DBNull.Value)
                        {
                            invitado.Puerta_Entrada = Convert.ToInt32(dtResult[18]);
                        }
                        if (dtResult[19] != System.DBNull.Value)
                        {
                            invitado.Actualizada_La_Salida = Convert.ToInt32(dtResult[19]);
                        }
                        if (dtResult[20] != System.DBNull.Value)
                        {
                            invitado.Horas_Caducidad = Convert.ToInt32(dtResult[20]);
                        }
                        if (dtResult[21] != System.DBNull.Value)
                        {
                            invitado.Personas = Convert.ToInt32(dtResult[21]);
                        }
                        if (dtResult[22] != System.DBNull.Value)
                        {
                            invitado.In_Out = Convert.ToInt32(dtResult[22]);
                        }
                        if (dtResult[23] != System.DBNull.Value)
                        {
                            invitado.Origen_Entrada = dtResult[23].ToString();
                        }
                        if (dtResult[24] != System.DBNull.Value)
                        {
                            invitado.Origen_Salida = dtResult[24].ToString();
                        }
                        if (dtResult[25] != System.DBNull.Value)
                        {
                            invitado.Comentario = dtResult[25].ToString();
                        }
                        if (dtResult[26] != System.DBNull.Value)
                        {
                            invitado.Origen_IO = Convert.ToInt32(dtResult[26]);
                        }
                        if (dtResult[27] != System.DBNull.Value)
                        {
                            invitado.Actualizado = Convert.ToInt32(dtResult[27]);
                        }
                        if (dtResult[28] != System.DBNull.Value)
                        {
                            invitado.Cpost = dtResult[28].ToString();
                        }
                        if (dtResult[29] != System.DBNull.Value)
                        {
                            invitado.Texto1_Entrada = dtResult[29].ToString();
                        }
                        if (dtResult[30] != System.DBNull.Value)
                        {
                            invitado.Texto2_Entrada = dtResult[30].ToString();
                        }
                        if (dtResult[31] != System.DBNull.Value)
                        {
                            invitado.Texto3_Entrada = dtResult[31].ToString();
                        }
                        if (dtResult[32] != System.DBNull.Value)
                        {
                            invitado.Secuencia_Dia = dtResult[32].ToString();
                        }
                        if (dtResult[33] != System.DBNull.Value)
                        {
                            invitado.No_Aplica_Induccion = dtResult[33].ToString();
                        }
                        if (dtResult[34] != System.DBNull.Value)
                        {
                            invitado.Visitado = Convert.ToInt32(dtResult[34]);
                        }
                        if (dtResult[35] != System.DBNull.Value)
                        {
                            invitado.Lector = Convert.ToInt32(dtResult[35]);
                        }
                        if (dtResult[36] != System.DBNull.Value)
                        {
                            invitado.SALIDA_ID = Convert.ToInt32(dtResult[36]);
                        }
                        #endregion

                        GuestsList.Add(invitado);
                        Debug.WriteLine("Usuario agregado, Id: " + invitado.INVITADO_ID);
                    }
                }
                dtResult.Close();
                fb.Close();
                fb.Dispose();
                Preferences.Set("SYNC_VSU", true);
                return GuestsList;
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en el metodo ExecuteGuestOuts, provocado por: " + ea.Message);
                Preferences.Set("SYNC_VSU", false);
                return null;
            }
        }


        // Retornar lista de Reservaciones
        public List<VW_RESERVA_VISITA> ExecuteReservations(string query)
        {
            try
            {
                List<VW_RESERVA_VISITA> ReservationsList = new List<VW_RESERVA_VISITA>();

                FbConnection fb = new FbConnection(connectionString(true));

                fb.Open();
                FbCommand command = new FbCommand(
                    query,
                    fb);

                var dtResult = command.ExecuteReader();

                if (dtResult.HasRows)
                {
                    while (dtResult.Read())
                    {
                        VW_RESERVA_VISITA reservation = new VW_RESERVA_VISITA();

                        #region Verificacion
                        if (dtResult[0] != System.DBNull.Value)
                        {
                            reservation.VISITA_ID = Convert.ToInt32(dtResult[0]);
                        }
                        else
                        {
                            continue;
                        }
                        if (dtResult[1] != System.DBNull.Value)
                        {
                            reservation.VISITAS_VISITA_A_ID = Convert.ToInt32(dtResult[1]);
                        }
                        if (dtResult[2] != System.DBNull.Value)
                        {
                            reservation.VISITAS_DOCUMENTO = dtResult[2].ToString();
                        }
                        if (dtResult[3] != System.DBNull.Value)
                        {
                            reservation.VISITAS_NOMBRE_COMPLETO = dtResult[3].ToString();
                        }
                        if (dtResult[4] != System.DBNull.Value)
                        {
                            reservation.VISITAS_FECHA_VISITA_DESDE = Convert.ToDateTime(dtResult[4]);
                        }
                        if (dtResult[5] != System.DBNull.Value)
                        {
                            reservation.VISITAS_FECHA_VISITA_HASTA = Convert.ToDateTime(dtResult[5]);
                        }
                        if (dtResult[6] != System.DBNull.Value)
                        {
                            reservation.EMPRESAS_NOMBRE = dtResult[6].ToString();
                        }
                        if (dtResult[7] != System.DBNull.Value)
                        {
                            reservation.VISITAS_FECHA_RESERVA = Convert.ToDateTime(dtResult[7]);
                        }
                        if (dtResult[8] != System.DBNull.Value)
                        {
                            reservation.VISITAS_DEPARTAMENTO_ID = Convert.ToInt32(dtResult[8]);
                        }
                        if (dtResult[9] != System.DBNull.Value)
                        {
                            reservation.DEPARTAMENTO_NOMBRE = dtResult[9].ToString();
                        }
                        #endregion

                        ReservationsList.Add(reservation);
                        Debug.WriteLine("Visita agregada, Id de visita: " + reservation.VISITA_ID);
                    }
                }
                dtResult.Close();
                fb.Close();
                fb.Dispose();

                Preferences.Set("SYNC_VSU", true);
                return ReservationsList;
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en el metodo ExecuteReservations, provocado por: " + ea.Message);
                Preferences.Set("SYNC_VSU", false);
                return null;
            }
        }
        public List<COMPANIASLOC> ExecuteCompaniesLoc(string query)
        {
            try
            {
                List<COMPANIASLOC> CompaniesList = new List<COMPANIASLOC>();

                FbConnection fb = new FbConnection(connectionString(true));

                fb.Open();
                FbCommand command = new FbCommand(
                    query,
                    fb);

                var dtResult = command.ExecuteReader();

                if (dtResult.HasRows)
                {
                    while (dtResult.Read())
                    {
                        COMPANIASLOC company = new COMPANIASLOC();

                        #region Verificar que los valores no sean nulos antes de la conversion
                        if (dtResult[0] != System.DBNull.Value)
                        {
                            company.COMPANIA_ID = Convert.ToInt32(dtResult[0]);
                        }

                        if (dtResult[1] != System.DBNull.Value)
                        {
                            company.NOMBRE = dtResult[1].ToString();
                        }
                        if (dtResult[2] != System.DBNull.Value)
                        {
                            company.PUNTO_VSU = Convert.ToInt32(dtResult[2]);
                        }
                        if (dtResult[3] != System.DBNull.Value)
                        {
                            company.ESTATUS = Convert.ToInt32(dtResult[3]);
                        }
                        #endregion

                        CompaniesList.Add(company);
                        Debug.WriteLine("Compania agregada, Id: " + company.COMPANIA_ID);
                    }
                }
                dtResult.Close();
                fb.Close();
                fb.Dispose();

                Preferences.Set("SYNC_VSU", true);
                return CompaniesList;
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en el metodo ExecuteCompanies, provocado por: " + ea.Message);
                Preferences.Set("SYNC_VSU", false);
                return null;
            }
        }

        // Retornar lista de Companias
        public List<COMPANIAS> ExecuteCompanies(string query)
        {
            try
            {
                List<COMPANIAS> CompaniesList = new List<COMPANIAS>();

                FbConnection fb = new FbConnection(connectionString(true));

                fb.Open();
                FbCommand command = new FbCommand(
                    query,
                    fb);

                var dtResult = command.ExecuteReader();

                if (dtResult.HasRows)
                {
                    while (dtResult.Read())
                    {
                        COMPANIAS company = new COMPANIAS();

                        #region Verificar que los valores no sean nulos antes de la conversion
                        if (dtResult[0] != System.DBNull.Value)
                        {
                            company.COMPANIA_ID = Convert.ToInt32(dtResult[0]);
                        }

                        if (dtResult[1] != System.DBNull.Value)
                        {
                            company.NOMBRE = dtResult[1].ToString();
                        }
                        if (dtResult[2] != System.DBNull.Value)
                        {
                            company.PUNTO_VSU = Convert.ToInt32(dtResult[2]);
                        }
                        if (dtResult[3] != System.DBNull.Value)
                        {
                            company.ESTATUS = Convert.ToInt32(dtResult[3]);
                        }
                        #endregion

                        CompaniesList.Add(company);
                        Debug.WriteLine("Compania agregada, Id: " + company.COMPANIA_ID);
                    }
                }
                dtResult.Close();
                fb.Close();
                fb.Dispose();

                Preferences.Set("SYNC_VSU", true);
                return CompaniesList;
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en el metodo ExecuteCompanies, provocado por: " + ea.Message);
                Preferences.Set("SYNC_VSU", false);
                return null;
            }
        }

        public List<DEPTO_LOCALIDAD> executeDepto_Localidad(string query)
        {
            try
            {
                List<DEPTO_LOCALIDAD> lista = new List<DEPTO_LOCALIDAD>();

                FbConnection fb = new FbConnection(connectionString(true));

                fb.Open();
                FbCommand command = new FbCommand(
                    query,
                    fb);

                var dtResult = command.ExecuteReader();

                if (dtResult.HasRows)
                {
                    while (dtResult.Read())
                    {
                        DEPTO_LOCALIDAD depto_localidad = new DEPTO_LOCALIDAD();

                        #region Verificar que los valores no sean nulos antes de la conversion                        
                        if (dtResult[0] != System.DBNull.Value)
                        {
                            depto_localidad.ID_DEPTO_LOCALIDAD = Convert.ToInt32(dtResult[0]);
                        }
                        if (dtResult[1] != System.DBNull.Value)
                        {
                            depto_localidad.ID_DEPARTAMENTO = Convert.ToInt32(dtResult[1]);
                        }
                        if (dtResult[2] != System.DBNull.Value)
                        {
                            depto_localidad.DEPTO_NOMBRE = Convert.ToString(dtResult[2]);
                        }
                        if (dtResult[3] != System.DBNull.Value)
                        {
                            depto_localidad.ID_LOCALIDAD = Convert.ToInt32(dtResult[3]);
                        }
                        if (dtResult[4] != System.DBNull.Value)
                        {
                            depto_localidad.LOCALIDAD_NOMBRE = Convert.ToString(dtResult[4]);
                        }
                        #endregion

                        lista.Add(depto_localidad);
                        Debug.WriteLine("DEPTO_LICALIDAD agregada, Id: " + depto_localidad.ID_DEPTO_LOCALIDAD);
                    }
                }
                dtResult.Close();
                fb.Close();
                fb.Dispose();

                Preferences.Set("SYNC_VSU", true);
                return lista;
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en el metodo ExecuteDEPTO_LOCALIDAD, provocado por: " + ea.Message);
                Preferences.Set("SYNC_VSU", false);
                return null;
            }
        }
        // Retornar lista de Personas
        public List<PERSONAS> ExecutePeople(string query)
        {
            try
            {
                List<PERSONAS> PeopleList = new List<PERSONAS>();

                FbConnection fb = new FbConnection(connectionString(true));

                fb.Open();
                FbCommand command = new FbCommand(
                    query,
                    fb);

                var dtResult = command.ExecuteReader();

                if (dtResult.HasRows)
                {
                    while (dtResult.Read())
                    {
                        #region Verificar que los valores no sean nulos antes de realizar la conversion de ser asi, asignar null
                        PERSONAS person = new PERSONAS();
                        if (dtResult[0] != System.DBNull.Value)
                        {
                            person.PERSONA_ID = Convert.ToInt32(dtResult[0]);
                        }
                        else
                        {
                            continue;
                        }
                        if (dtResult[1] != System.DBNull.Value)
                        {
                            person.NOMBRES_APELLIDOS = dtResult[1].ToString();
                        }
                        if (dtResult[2] != System.DBNull.Value)
                        {
                            person.DEPARTAMENTO_ID = Convert.ToInt32(dtResult[2]);
                        }
                        #endregion
                        PeopleList.Add(person);
                        Debug.WriteLine("Persona agregada, Id: " + person.PERSONA_ID);
                    }
                }
                dtResult.Close();
                fb.Close();
                fb.Dispose();

                Preferences.Set("SYNC_VSU", true);
                return PeopleList;
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en el metodo ExecutePeople, provocado por: " + ea.Message);
                Preferences.Set("SYNC_VSU", false);
                return null;
            }
        }


        //// Probar que hay conexion trayendo datos
        public void tryConnection()
        {
            string dt = obtenerfecha();

            try
            {
                if (!string.IsNullOrEmpty(dt))
                {
                    Preferences.Set("SYNC_VSU", true);
                }
                else
                {
                    Preferences.Set("SYNC_VSU", false);
                }
            }
            catch (Exception)
            {
                Preferences.Set("SYNC_VSU", false);

            }

        }


        //// Proveer informacion relativo a la region
        public void PublicServices()
        {
            try
            {
                CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                culture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
                Thread.CurrentThread.CurrentCulture = culture;

            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en el metodo PublicServices, provocado por: " + ea.Message);
            }
        }


        //// Subir Visitantes que NO se han subido
        public void UploadVisits()
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));

                // Cargar los invitados con un valor null en la propiedad SUBIDA
                var visitasASubir = db.Query<Invitados>("SELECT * FROM Invitados where SUBIDA is null");

                // Iterar la lista de visitasASubir
                foreach (Invitados registro in visitasASubir)
                {
                    #region Variables
                    string visitado;
                    string fechaSalida;
                    string placa;
                    string queryInv;
                    string codigo_carnet;
                    #endregion

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
                    if (string.IsNullOrWhiteSpace(registro.Codigo_carnet))
                    {
                        codigo_carnet = "null";
                    }
                    else
                    {
                        codigo_carnet = registro.Codigo_carnet.ToString().ToUpper();
                    }

                    #region Query Invitado
                    queryInv = "SELECT IN_INVIDATO_ID as anyCount FROM INSERTAR_VISITAS(" +
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
                          $", {Util.CoalesceStr(fechaSalida, "null")}, " +
                          $" '{Util.CoalesceStr(codigo_carnet, "null")}')";
                    #endregion

                    var dtResult = ExecuteScalar(queryInv);


                    if (Preferences.Get("SYNC_VSU", false))
                    {

                        if (!string.IsNullOrEmpty(dtResult))
                        {
                            registro.INVIDATO_ID = Convert.ToInt32(dtResult);
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
                        }
                        db.UpdateAll(visitasASubir);
                    }
                }
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Excepcion en el metodo UploadVisit, error: " + ea.Message);
                Analytics.TrackEvent("Error de SQL en el escaner: " + Preferences.Get("LECTOR", "N/A") + " Error: " + ea.Message);
            }
        }


        //// Subir Visitantes con reservaciones que no se hayan subido
        public void UploadVisitsReservation()
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));

                // Cargar Invitados con reservas a visitasASubir2
                var visitasASubir2 = db.Query<InvitadosReservas>("SELECT * FROM InvitadosReservas where SUBIDA is null");
                Debug.WriteLine("There are: " + visitasASubir2.Count.ToString() + " To upload");

                //Iterar el arreglo visitas con reservas
                foreach (InvitadosReservas registro2 in visitasASubir2)
                {
                    string queryInv2;
                    string visitado2;
                    string fechaSalida2;
                    string placa2;
                    string Codigo_carnet;

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
                    if (string.IsNullOrWhiteSpace(registro2.Codigo_carnet))
                    {
                        Codigo_carnet = "null";
                    }
                    else
                    {
                        Codigo_carnet = registro2.Codigo_carnet.ToString().ToUpper();
                    }

                    #region string de la variable queryInv
                    queryInv2 = "SELECT IN_INVIDATO_ID as anyCount FROM INSERTAR_VISITAS(" +
                        "" + registro2.Compania_ID.ToString() + ", " +
                        "'" + registro2.Nombres + "', " +
                        "'" + registro2.Apellidos + "', " +
                        "'" + registro2.Fecha_Registro.ToString() + "', " +
                        "'" + registro2.Cargo + "'," +
                        "0," +
                        "100, " +
                        "1, " +
                        $"{Util.Coalesce(registro2.Empresa_ID, "null")}, " +
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
                        $"{fechaSalida2}, " +
                        $"'{Util.CoalesceStr(Codigo_carnet, "null")}')";
                    #endregion

                    var content = ExecuteScalar(queryInv2);


                    if (!string.IsNullOrEmpty(content))
                    {

                        registro2.INVIDATO_ID = Convert.ToInt32(content);
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
                    }
                }
                db.UpdateAll(visitasASubir2);
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Excepcion en el metodo UploadVisitsReservation, error: " + ea.Message);
                Analytics.TrackEvent("Escaner: " + Preferences.Get("LECTOR", "N/A") + " Excepcion en el metodo UploadVisitsReservation, error: " + ea.Message);
            }
        }


        //// Subir Verificaciones 
        public void UploadVerifications()
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));

                var verificarASubir = db.Query<Invitados>("SELECT * FROM Invitados where VERIFICACIONSUBIDA is null and Fecha_Verificacion is not null and SUBIDA is not null");
                foreach (Invitados registro in verificarASubir)
                {
                    var queryVer = "SELECT * FROM SP_DAR_VERIFICACION(" + registro.INVIDATO_ID.ToString() + ", " + registro.Puerta_Registro.ToString() + ", '" +
                    registro.Fecha_Verificacion.ToString() + "')";


                    var content = ExecuteScalar(queryVer);
                    Debug.WriteLine("Waiting for: " + queryVer);

                    if (!string.IsNullOrEmpty(content))
                    {
                        if (true)
                        {

                        }
                        registro.verificacionSubida = true;
                        Debug.WriteLine("Verificacion subida: ID" + registro.INVIDATO_ID.ToString() + " " + registro.Fecha_Verificacion.ToString());
                    }
                    else
                    {
                        registro.verificacionSubida = null;

                    }
                }
                db.UpdateAll(verificarASubir);
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Excepcion en el metodo UploadVerifications, error: " + ea.Message);
                Analytics.TrackEvent("Error en el metodo UploadVerifications, error: " + ea.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            }
        }

        //// Subir Salidas
        public void UploadOut()
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                var salidasASubir = db.Query<Invitados>("SELECT * FROM Invitados where SALIDASUBIDA is null and FECHA_SALIDA is not null and SUBIDA is not null");
                var salidas = db.Query<Invitados>("SELECT * FROM Invitados");

                foreach (Invitados registro in salidasASubir)
                {
                    var querrySal = "SELECT * FROM SP_DAR_SALIDA(" + registro.INVIDATO_ID.ToString() + ", '" + registro.Fecha_Salida.ToString() + "', " +
                    Preferences.Get("LECTOR", "1") + ")";

                    var content = ExecuteScalar(querrySal);
                    Debug.WriteLine("Waiting for: " + querrySal);

                    if (!string.IsNullOrEmpty(content))
                    {
                        registro.salidaSubida = true;
                        Debug.WriteLine("Salida subida: ID" + registro.INVIDATO_ID.ToString() + " " + registro.Fecha_Salida.ToString());
                    }
                    else
                    {
                        registro.salidaSubida = null;
                    }
                }
                db.UpdateAll(salidasASubir);
            }
            catch (Exception ea)
            {
                Analytics.TrackEvent("Escaner: " + Preferences.Get("LECTOR", "N/A") + "\n Excepcion en el metodo UploadOut, Error: " + ea.Message);
                Debug.WriteLine("Excepcion en el metodo UploadOut, Error: " + ea.Message);
            }
        }

        //// Subir Salidas desconocidas
        public void UploadUnknownOuts()
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                var salidasOFF = db.Query<SalidaOffline>("SELECT * FROM SalidaOffline where SUBIDA is null");

                foreach (SalidaOffline registros in salidasOFF)
                {
                    var querrySalOFF = "SELECT * FROM SP_DAR_SALIDA(" + registros.INVIDATO_ID + ", '" + registros.Fecha_Salida + "', " +
                    Preferences.Get("LECTOR", "1") + ")";

                    var content = ExecuteScalar(querrySalOFF);
                    Debug.WriteLine("Waiting for: " + querrySalOFF);

                    if (!string.IsNullOrEmpty(content))
                    {
                        registros.Subida = true;
                        Debug.WriteLine("Salida subida: ID" + registros.INVIDATO_ID.ToString() + " " + registros.Fecha_Salida.ToString());
                    }
                    else
                    {
                        registros.Subida = null;

                    }
                }
                db.UpdateAll(salidasOFF);
            }
            catch (Exception ea)
            {
                Analytics.TrackEvent("Escaner: " + Preferences.Get("LECTOR", "N/A") + "\n Excepcion en el metodo UploadUnknownOuts, Error: " + ea.Message);
                Debug.WriteLine("Excepcion en el metodo UploadUnknownOuts, Error: " + ea.Message);
            }
        }


        //// Cargar Reservaciones
        public void DownloadReservations()
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));

                #region string que contiene el Query
                string query = "SELECT FIRST " + Preferences.Get("CHUNK_SIZE", "10000") + " VISITA_ID,VISITAS_VISITA_A_ID, VISITAS_DOCUMENTO" +
                            ", VISITAS_NOMBRE_COMPLETO, CAST(VISITAS_FECHA_VISITA_DESDE as VARCHAR(25)) as VISITAS_FECHA_VISITA_DESDE" +
                            ", CAST(VISITAS_FECHA_VISITA_HASTA as VARCHAR(25)) as VISITAS_FECHA_VISITA_HASTA, EMPRESAS_NOMBRE" +
                            ", CAST(VISITAS_FECHA_RESERVA as VARCHAR(25)) as VISITAS_FECHA_RESERVA, " + " VISITAS_DEPARTAMENTO_ID, " + "DEPARTAMENTO_NOMBRE " +
                            "FROM vw_reserva_visita where VISITA_ID > " + Preferences.Get("MAX_RESERVA_ID", "0") + " ORDER BY VISITA_ID desc";
                #endregion

                var ListaReservaciones = ExecuteReservations(query);

                if (ListaReservaciones != null)
                {
                    try
                    {
                        if (ListaReservaciones.Any())
                        {
                            db.InsertAll(ListaReservaciones);
                            Debug.WriteLine("MAX_RESERVA_ID: " + ListaReservaciones.First().VISITA_ID.ToString());
                            Preferences.Set("MAX_RESERVA_ID", ListaReservaciones.First().VISITA_ID.ToString());
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
                    Debug.WriteLine("ListaReservaciones = null");
                }
            }
            catch (Exception ea)
            {

                Analytics.TrackEvent("Escaner: " + Preferences.Get("LECTOR", "N/A") + " Excepcion en el metodo DownloadReservations, Error: " + ea.Message);
                Debug.WriteLine("Excepcion en el metodo DownloadReservations, Error: " + ea.Message);
            }
        }


        //// Cargar Companies
        public void DownloadCompanies()
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                string queryCompanias = " SELECT FIRST "
                                                   + Preferences.Get("CHUNK_SIZE", "10000")
                                                   + " COMPANIA_ID, NOMBRE, PUNTO_VSU, ESTATUS  FROM COMPANIAS where COMPANIA_ID > "
                                                   + Preferences.Get("MAX_COMPANIA_ID", "0")
                                                   + " ORDER BY COMPANIA_ID desc";

                var ListaCompanias = ExecuteCompanies(queryCompanias);

                var stlRegistros = new List<string>();
                var stRegisros = "";
                if (ListaCompanias != null)
                {
                    try
                    {
                        if (ListaCompanias.Any())
                        {

                            db.InsertAll(ListaCompanias);
                            Debug.WriteLine("MAX_COMPANIA_ID: " + ListaCompanias.First().COMPANIA_ID.ToString());
                            Preferences.Set("MAX_COMPANIA_ID", ListaCompanias.First().COMPANIA_ID.ToString());
                            Debug.WriteLine("Companias Descargadas: " + DateTime.Now);
                            string sortNames = "select nombre from companias where PUNTO_VSU = 0 AND ESTATUS = 1 order by nombre";
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

            }
            catch (Exception ea)
            {
                Analytics.TrackEvent("Escaner: " + Preferences.Get("LECTOR", "N/A") + " Excepcion en el metodo DownloadCompanies, Error: " + ea.Message);
                Debug.WriteLine("Error de SQL: " + ea.Message);
            }
        }

        //// Cargar Departamentos
        public void DownloadDeptoLocalidad()
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                string query = " SELECT * "
                                                   + " FROM VW_DEPTO_LOCALIDAD where ID_DEPTO_LOCALIDAD > "
                                                   + Preferences.Get("MAX_DEPTO_LOCALIDAD", "0")
                                                   + " ORDER BY ID_DEPTO_LOCALIDAD desc";

                var Lista_DEPTO_LOCALIDAD = executeDepto_Localidad(query);

                var stlRegistros = new List<string>();
                var stRegisros = "";
                if (Lista_DEPTO_LOCALIDAD != null)
                {
                    try
                    {
                        if (Lista_DEPTO_LOCALIDAD.Any())
                        {

                            db.InsertAll(Lista_DEPTO_LOCALIDAD);
                            Debug.WriteLine("MAX_COMPANIA_ID: " + Lista_DEPTO_LOCALIDAD.First().ID_DEPTO_LOCALIDAD.ToString());
                            Preferences.Set("MAX_COMPANIA_ID", Lista_DEPTO_LOCALIDAD.First().ID_DEPTO_LOCALIDAD.ToString());
                            Debug.WriteLine("Departamento Localidad Descargadas: " + DateTime.Now);
                            //string sortNames = "select nombre from companias where PUNTO_VSU = 0 AND ESTATUS = 1 order by nombre";
                            //var Sorting = db.Query<DEPTO_LOCALIDAD>(sortNames);
                            //foreach (DEPTO_LOCALIDAD registro in Sorting)
                            //{
                            //    //db.Insert(registro);
                            //    Debug.WriteLine("DEPTO_LOCALIDAD: " + registro.DEPTO_NOMBRE);
                            //    stlRegistros.Add(registro.NOMBRE.ToString());
                            //    stRegisros = stRegisros + "," + registro.NOMBRE.ToString();
                            //}
                            //stRegisros = stRegisros.TrimStart(',');
                            //Preferences.Set("COMPANIAS_LIST", stRegisros);
                        }
                    }
                    catch (Exception ex)
                    {
                        var properties = new Dictionary<string, string> {
                                            { "Category", "Error insertando DEPTO_LOCALIDAD" },
                                            { "Code", "App.xaml.cs Line: 516" },
                                            { "Lector", Preferences.Get("LECTOR", "N/A")}
                                        };
                        Debug.WriteLine("Excepcion insertando DEPTO_LOCALIDAD: " + ex.ToString());
                        Crashes.TrackError(ex, properties);
                    }

                }

            }
            catch (Exception ea)
            {
                Analytics.TrackEvent("Escaner: " + Preferences.Get("LECTOR", "N/A") + " Excepcion en el metodo Download_DEPTO_LOCALIDAD, Error: " + ea.Message);
                Debug.WriteLine("Error de SQL: " + ea.Message);
            }
        }
        public void DownloadCompaniesPorLocalidad(string querry)
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                var dbd = new SQLiteConnection(Preferences.Get("DB_PATH", ""));


                var ListaCompanias = ExecuteCompaniesLoc(querry);

                var stlRegistros = new List<string>();
                var stRegisros = "";
                if (ListaCompanias != null)
                {
                    try
                    {
                        if (ListaCompanias.Any())
                        {
                            dbd.DeleteAll<COMPANIASLOC>();
                            db.InsertAll(ListaCompanias);
                            //Debug.WriteLine("MAX_COMPANIA_ID: " + ListaCompanias.First().COMPANIA_ID.ToString());
                            //Preferences.Set("MAX_COMPANIA_ID", ListaCompanias.First().COMPANIA_ID.ToString());
                            Debug.WriteLine("Companias Descargadas: " + DateTime.Now);
                            string sortNames = "select nombre from COMPANIASLOC where  ESTATUS = 1 order by nombre";
                            var Sorting = db.Query<COMPANIASLOC>(sortNames);
                            foreach (COMPANIASLOC registro in Sorting)
                            {
                                //db.Insert(registro);
                                Debug.WriteLine("EMPRESAS: " + registro.NOMBRE);
                                stlRegistros.Add(registro.NOMBRE.ToString());
                                stRegisros = stRegisros + "," + registro.NOMBRE.ToString();
                            }
                            stRegisros = stRegisros.TrimStart(',');
                            Preferences.Set("COMPANIAS_LIST_LOC", stRegisros);
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

            }
            catch (Exception ea)
            {
                Analytics.TrackEvent("Escaner: " + Preferences.Get("LECTOR", "N/A") + " Excepcion en el metodo DownloadCompanies, Error: " + ea.Message);
                Debug.WriteLine("Error de SQL: " + ea.Message);
            }
        }


        //// Cargar Personas(Destinos)
        public void DownloadPeople_Destination()
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));

                var stlRegistros2 = new List<string>();
                var stRegisros2 = "";
                string queryPersonas = "SELECT FIRST " + Preferences.Get("CHUNK_SIZE", "10000") + " PERSONA_ID, NOMBRES_APELLIDOS, DEPARTAMENTO_ID " +
                                       "FROM PERSONAS " +
                                       "where TIPO=1 AND PERSONA_ID > " + Preferences.Get("MAX_PERSONA_ID", "0") + 
                                       " ORDER BY PERSONA_ID desc";
                var ListaPersonas = ExecutePeople(queryPersonas);


                if (ListaPersonas != null)
                {

                    try
                    {
                        if (ListaPersonas.Any())
                        {
                            db.InsertAll(ListaPersonas);
                            Debug.WriteLine("MAX_PERSONA_ID: " + ListaPersonas.First().PERSONA_ID.ToString());
                            Preferences.Set("MAX_PERSONA_ID", ListaPersonas.First().PERSONA_ID.ToString());
                            Debug.WriteLine("Personas Descargadas: " + DateTime.Now);

                            string sortPersons = "select nombres_apellidos from personas order by nombres_apellidos";
                            var Sorting = db.Query<PERSONAS>(sortPersons);
                            foreach (var registro in Sorting)
                            {
                                if (registro.NOMBRES_APELLIDOS != null)
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
            }
            catch (Exception ea)
            {
                Analytics.TrackEvent("Escaner: " + Preferences.Get("LECTOR", "N/A") + " Excepcion en el metodo DownloadPeople_Destination, Error: " + ea.Message);
                Debug.WriteLine("Excepcion en el metodo DownloadPeople_Destination, Error: " + ea.Message);
            }
        }


        //// Descargar Invitados
        public void DownloadGuests()
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));

                string queryDownInv = $"SELECT FIRST {Preferences.Get("CHUNK_SIZE", "10000")} I1.INVIDATO_ID, 1 AS SUBIDA, IIF(I1.FECHA_SALIDA IS NULL, 0, 1) AS SALIDASUBIDA, I1.COMPANIA_ID" +
                            ", I1.NOMBRES, I1.APELLIDOS, I1.FECHA_REGISTRO, I1.FECHA_SALIDA, I1.TIPO, I1.CARGO, I1.TIENE_ACTIVO, I1.ESTATUS_ID, I1.MODULO" +
                            ", I1.EMPRESA_ID, I1.PLACA, I1.TIPO_VISITANTE, I1.ES_GRUPO, I1.GRUPO_ID, I1.PUERTA_ENTRADA, I1.ACTUALIZADA_LA_SALIDA" +
                            ", COALESCE(I1.HORAS_CADUCIDAD,0) AS HORAS_CADUCIDAD, I1.PERSONAS, I1.IN_OUT, I1.ORIGEN_ENTRADA, I1.ORIGEN_SALIDA, I1.COMENTARIO, COALESCE(I1.ORIGEN_IO,0) AS ORIGEN_IO, I1.ACTUALIZADO" +
                            ", I1.CPOST, I1.TEXTO1_ENTRADA, I1.TEXTO2_ENTRADA, I1.TEXTO3_ENTRADA, I1.SECUENCIA_DIA, I1.NO_APLICA_INDUCCION, I1.VISITADO" +
                            ", COALESCE(I1.LECTOR, 0) AS LECTOR, I1.CODIGO_CARNET FROM INVITADOS I1 INNER JOIN(SELECT MAX(I2.INVIDATO_ID) AS INVIDATO_ID FROM INVITADOS I2" +
                            $" WHERE INVIDATO_ID > {Preferences.Get("MAX_INVIDATO_ID", "0")} AND COALESCE(LECTOR, 0) <> {Preferences.Get("LECTOR", "1")} " +
                            "GROUP BY I2.CARGO) I3 ON I1.invidato_id = I3.INVIDATO_ID ORDER BY I1.INVIDATO_ID DESC";


                var contentDownInv = ExecuteGuest(queryDownInv);


                if (contentDownInv != null)
                {

                    try
                    {
                        if (contentDownInv.Any())
                        {
                            Debug.WriteLine("Se va a descargar: " + contentDownInv.Count().ToString() + " Visitas");
                            db.InsertAll(contentDownInv);
                            Debug.WriteLine("MAX_INVIDATO_ID: " + contentDownInv.First().INVIDATO_ID.ToString());
                            Preferences.Set("MAX_INVIDATO_ID", contentDownInv.First().INVIDATO_ID.ToString());
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
            }
            catch (Exception ea)
            {
                Analytics.TrackEvent("Escaner: " + Preferences.Get("LECTOR", "N/A") + " Excepcion en el metodo DownloadGuests, Error: " + ea.Message);
                Debug.WriteLine("Excepcion en el metodo DownloadGuests, Error: " + ea.Message);
            }
        }

        //// Descargar Salidas
        public void DownloadOuts()
        {
            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                var maxInvidatoIdLocal = db.Query<counterObj>("SELECT MAX(INVIDATO_ID) as anycount FROM Invitados");

                var queryDownSal = "SELECT FIRST " + Preferences.Get("CHUNK_SIZE", "10000") + " INVIDATO_ID, 1 as SUBIDA, 1 as SALIDASUBIDA, COMPANIA_ID, NOMBRES, APELLIDOS, " +
                "FECHA_REGISTRO, FECHA_SALIDA, TIPO, CARGO, TIENE_ACTIVO, ESTATUS_ID, MODULO, EMPRESA_ID, PLACA, TIPO_VISITANTE, ES_GRUPO, GRUPO_ID, " +
                "PUERTA_ENTRADA, ACTUALIZADA_LA_SALIDA, HORAS_CADUCIDAD, PERSONAS, IN_OUT, ORIGEN_ENTRADA, ORIGEN_SALIDA, COMENTARIO, ORIGEN_IO, " +
                "ACTUALIZADO, CPOST, TEXTO1_ENTRADA, TEXTO2_ENTRADA, TEXTO3_ENTRADA, SECUENCIA_DIA, NO_APLICA_INDUCCION, VISITADO, COALESCE(LECTOR, 0) AS LECTOR, SALIDA_ID " +
                "FROM INVITADOS WHERE SALIDA_ID > " + Preferences.Get("MAX_SALIDA_ID", "0") + " AND INVIDATO_ID <= " + maxInvidatoIdLocal.First().anyCount.ToString() +
                " AND SALIDA_ID IS NOT NULL AND COALESCE(LECTOR_SALIDA,0) <> " + Preferences.Get("LECTOR", "1") +
                " ORDER BY SALIDA_ID DESC";

                var listSalidas = ExecuteGuestOuts(queryDownSal);

                if (listSalidas != null)
                {

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
            catch (Exception ea)
            {
                Analytics.TrackEvent("Excepcion en el metodo DownloadOuts, Error: " + ea.Message);
                Debug.WriteLine("Excepcion en el metodo DownloadOuts, Error: " + ea.Message);
            }
        }

        //// obtener la fecha del servidor
        public string obtenerfecha()
        {
            string query = "select current_timestamp FROM Invitados";
            try
            {
                string fecha = "";
                //List<Mes> minutos = new List<Mes>();
                FbConnection fb = new FbConnection(connectionString(true));

                fb.Open();
                FbCommand command = new FbCommand(
                    query,
                    fb);

                fecha = command.ExecuteScalar().ToString();


                fb.Close();
                fb.Dispose();
                //Preferences.Set("SYNC_VSU", true);

                return fecha;
            }
            catch (Exception e)
            {
                Preferences.Set("SYNC_VSU", false);
                Debug.WriteLine("Error en la fecha de base de datos " + e.Message);
                return null;
            }
        }

        //// Descargar el padron al dispositivo
        public void DownloadPadron()
        {

        }

        public List<VisitasDepto> extraerDeparatamentoId(COMPANIAS cc) //Extraer personas con su departamento//
        {
            string query = $"select p.nombres, p.departamento_id, c.nombre from personas p inner join companias c on c.compania_id = p.departamento_id where p.departamento_id = {cc.COMPANIA_ID}";
            try
            {
                List<VisitasDepto> deptosVisits = new List<VisitasDepto>();

                FbConnection fb = new FbConnection(connectionString(true));

                fb.Open();
                FbCommand command = new FbCommand(query, fb);

                var dtResult = command.ExecuteReader();

                if (dtResult.HasRows)
                {

                    while (dtResult.Read())
                    {
                        VisitasDepto visitasDepto = new VisitasDepto();

                        if (dtResult[0] != System.DBNull.Value)
                        {
                            visitasDepto.VisitName = dtResult[0].ToString();
                        }
                        if (dtResult[1] != System.DBNull.Value)
                        {
                            visitasDepto.DeptoId = Convert.ToInt32(dtResult[1]);
                        }
                        if (dtResult[2] != System.DBNull.Value)
                        {
                            visitasDepto.DeptoName = dtResult[2].ToString();
                        }
                        deptosVisits.Add(visitasDepto);
                    }
                }

                fb.Close();
                fb.Dispose();


                return deptosVisits;
            }
            catch (Exception e)
            {
                Preferences.Set("SYNC_VSU", false);
                Debug.WriteLine("Error en la extraccion de departamentoID " + e.Message);
                return null;
            }
        }

    }
}