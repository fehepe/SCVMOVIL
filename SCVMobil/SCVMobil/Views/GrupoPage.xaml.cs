using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SCVMobil.Models;
using Rg.Plugins.Popup.Services;
using SCVMobil.Views;

namespace SCVMobil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GrupoPage : ContentPage
    {
        ////Variables
        //Escaner scan;
        Escaner scanner;
        public GrupoPage(string empresa, string visita, string cedula, string nombres)//Constructor
        {
            InitializeComponent();
            this.empresa.Text = empresa;
            this.visitaA.Text = visita;
            //this.cedula.Text = cedula;
            this.Nombres.Text = nombres;
            Nombres.Completed += Nombres_Completed;
            this.cedula.Completed += Cedula_Completed;


            using (var datos = new DataAccess())
            {
                listaGrupo.ItemsSource = datos.GetVisitas();// Obtetiendo las visitas agregadas
            }
            scanner = new Escaner(cedulaScanned);// obteniendo el scanner

        }

        private async void Nombres_Completed(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cedula.Text))
                {
                    await DisplayAlert("Error", "Debe Ingresar Cedula", "Aceptar");
                    cedula.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(Nombres.Text))
                {
                    await DisplayAlert("Error", "Debe Ingresar nombres y apellidos", "Aceptar");
                    Nombres.Focus();
                    return;
                }

                Visitas visita = new Visitas
                {
                    Cedula = cedula.Text,
                    Nombres = Nombres.Text,
                    Empresa = empresa.Text,
                    VisitaA = visitaA.Text,

                };
                using (var datos = new DataAccess())
                {
                    datos.InsertVisita(visita);
                    listaGrupo.ItemsSource = datos.GetVisitas();
                }
                cedula.Text = string.Empty;
                Nombres.Text = string.Empty;

                await DisplayAlert("Confirmacion", "Visita agregada", "Aceptar");
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Error en Nombres_Completed, motivo: "+ ea.Message);
                Analytics.TrackEvent("Error al ingresar nombre seleccionadas: " + ea.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            }

        }


        public void refreshPage()
        {

        }
        protected override void OnDisappearing()// Cuando desaparezca la pantalla, apagamos el scanner
        {
            try
            {

                //cedula.Text = string.Empty;
                base.OnDisappearing();
                scanner.GetScanner(false);
                ppCedulaNoExiste.IsVisible = false;


            }
            catch (Exception ea)
            {
                Debug.WriteLine("Ha ocurrido una excepcion en el OnDisappearing, motivo: " + ea.Message);
                Analytics.TrackEvent("Error al apagar el scanner: " + ea.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            }
        }

        private void Cedula_Completed(object sender, EventArgs e)
        {
            entrada(cedula.Text);
        }
        public void cedulaScanned(string inString)// Metodo que se invoca cuando se scanea un documento
        {
            try
            {
                if (inString.Contains("|"))
                {
                    inString = inString.Substring(0, inString.IndexOf("|") + 0);
                    //Creamos la coneccion con la base de datos.
                    entrada(inString);
                }
                else
                {
                    entrada(inString);
                }
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Ha ocurrido una excepcion en el metodo cedulaScanned, motivo: " + ea.Message);
                Analytics.TrackEvent("Error escanear cedula: " + ea.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            }

        }
        private void BtOKCedulaNoExiste_Clicked(object sender, EventArgs e)
        {

            try
            {
                Navigation.PushAsync(new MainPage());
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Ha ocurrido una excepcion, en el BtOKCedulaNoExiste_Clicked, motivo: " + ea.Message);
                Analytics.TrackEvent("Error al presionar boton ok  seleccionadas: " + ea.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            }
        }

        protected override void OnAppearing() //Cuando aparesca la pagina, refrescamos.
        {

            try
            {
                Debug.WriteLine("Appeared");
                if (Preferences.Get("VISITA_A_SELECTED", false) == false)
                {
                    VisitaALabel.IsVisible = false;
                    FrameName1.IsVisible = false;
                    FrameName2.IsVisible = false;
                }
                cedula.Text = string.Empty;
                Nombres.Text = string.Empty;
                scanner.GetScanner(true);
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Ha ocurrido una excepcion, en el metodo OnAppeared, motivo: " + ea.Message);
                Analytics.TrackEvent("Error al refrescar pagina: " + ea.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            }
            
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)//Metodo para imprimir los grupos
        {

            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                //Creamos el query para buscar el documento.
                var querry = "SELECT * FROM Visitas";

                var registro = db.Query<Visitas>(querry);
                foreach (Visitas registros in registro)

                {


                    var registroInvitados = new Invitados();

                    //Vamos a buscar la compania seleccionada 


                    if (Preferences.Get("VISITA_A_SELECTED", true))
                    {
                        //Vamos a buscar la compania seleccionada 

                        var TBL_COMPANIAS = db.Query<COMPANIAS>("SELECT COMPANIA_ID FROM COMPANIAS WHERE NOMBRE = '" + empresa.Text + "'");

                        //Vamos a buscar la persona seleccionada

                        var TBL_PERSONAS = db.Query<PERSONAS>("SELECT PERSONA_ID FROM PERSONAS WHERE NOMBRES_APELLIDOS = '" + visitaA.Text + "'");
                        registroInvitados.Compania_ID = TBL_COMPANIAS.First().COMPANIA_ID;
                        registroInvitados.Nombres = registros.Nombres;
                        registroInvitados.Apellidos = " ";
                        registroInvitados.Fecha_Registro = DateTime.Now;
                        registroInvitados.Cargo = registros.Cedula;
                        registroInvitados.Tiene_Activo = 0;
                        registroInvitados.Estatus_ID = 100;
                        registroInvitados.Modulo = 1;
                        registroInvitados.Empresa_ID = 8;
                        registroInvitados.Placa = "00000";
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
                        registroInvitados.Visitado = TBL_PERSONAS.First().PERSONA_ID;
                        registroInvitados.Lector = int.Parse(Preferences.Get("LECTOR", "1"));
                        db.Insert(registroInvitados);

                    }
                    else
                    {
                        var TBL_COMPANIAS = db.Query<COMPANIAS>("SELECT COMPANIA_ID FROM COMPANIAS WHERE NOMBRE = '" + empresa.Text + "'");

                        //Vamos a buscar la persona seleccionada

                        var TBL_PERSONAS = db.Query<PERSONAS>("SELECT PERSONA_ID FROM PERSONAS WHERE NOMBRES_APELLIDOS = '" + visitaA.Text + "'");
                        registroInvitados.Compania_ID = TBL_COMPANIAS.First().COMPANIA_ID;
                        registroInvitados.Nombres = registros.Nombres;
                        registroInvitados.Apellidos = " ";
                        registroInvitados.Fecha_Registro = DateTime.Now;
                        registroInvitados.Cargo = registros.Cedula;
                        registroInvitados.Tiene_Activo = 0;
                        registroInvitados.Estatus_ID = 100;
                        registroInvitados.Modulo = 1;
                        registroInvitados.Empresa_ID = 8;
                        registroInvitados.Placa = "00000";
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
                        registroInvitados.Visitado = null;
                        registroInvitados.Lector = int.Parse(Preferences.Get("LECTOR", "1"));
                        db.Insert(registroInvitados);
                    }

                    await Navigation.PopToRootAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ToolBarItem");
                Analytics.TrackEvent("Error al buscar visitas: " + ex.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
                throw;
            }
        }

        public async void entrada(String inString)
        {
            try
            {
                if (inString != "")
                {

                    if (inString.Length == 11 || inString.StartsWith("ID"))
                    {
                        var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                        //Vamos a ver si es un ID de salida.
                        if (inString.StartsWith("ID"))
                        {

                            // var querry = "SELECT * FROM Invitados WHERE INVIDATO_ID = " + inString.Replace("ID", "") + " AND FECHA_SALIDA is null AND Fecha_Verificacion is not null";
                            var querrys = "SELECT * FROM Invitados WHERE INVIDATO_ID= " + inString.Replace("ID", "") + " AND FECHA_SALIDA is null";
                            //var registroInv = db.Query<Invitados>(querry);
                            var registroVer = db.Query<Invitados>(querrys);

                            if (registroVer.Any())
                            {
                                if (Preferences.Get("VERIFICA", false))
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
                                    else
                                    {
                                        var time = (DateTime.Now - registroVer.First().Fecha_Verificacion.Value).TotalSeconds;
                                        var pref = Convert.ToDouble(Preferences.Get("TIEMPOS", "1")) * 60;
                                        if (time < pref) //REVISAR//
                                        {
                                            DependencyService.Get<IToastMessage>().DisplayMessage("Se ha acaba de verificar este id.");
                                        }
                                        else
                                        {
                                            if (registroVer.First().Fecha_Salida is null)
                                            {

                                                registroVer.First().Fecha_Salida = DateTime.Now;
                                                registroVer.First().salidaSubida = null;
                                                db.UpdateAll(registroVer);
                                                DependencyService.Get<IToastMessage>().DisplayMessage("Se ha dado salida correctamente.");
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    registroVer.First().Fecha_Verificacion = DateTime.Now;
                                    registroVer.First().Puerta_Registro = Convert.ToInt32(Preferences.Get("PUERTA", "1459").ToString());
                                    registroVer.First().verificacionSubida = null;
                                    registroVer.First().Fecha_Salida = DateTime.Now;
                                    registroVer.First().salidaSubida = null;
                                    db.UpdateAll(registroVer);
                                    DependencyService.Get<IToastMessage>().DisplayMessage("Se ha dado salida correctamente.");
                                }
                            }
                            else
                            {

                                if (!Preferences.Get("SYNC_VSU", false))
                                {
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
                                else
                                {
                                    DependencyService.Get<IToastMessage>().DisplayMessage("Este ID de Salida no existe.");
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
                                //Esto quiere decir que aun la persona no ha salido.

                                registroVerifInv.First().Fecha_Salida = DateTime.Now;
                                registroVerifInv.First().salidaSubida = null;
                                db.UpdateAll(registroVerifInv);
                                DependencyService.Get<IToastMessage>().DisplayMessage("Se ha dado salida correctamente.");
                            }
                            else
                            {
                                // Como no tiene reserva entramos.
                                //Buscamos la cedula en el padron.
                                querry = "SELECT * FROM PADRON WHERE CEDULA = '" + inString + "'";
                                //var que = "select * from VW_RESERVA_VISITA where VISITAS_DOCUMENTO='" + inString + "'";
                                //var registro2 = db.Query<VW_RESERVA_VISITA>(que);

                                try
                                {
                                    //Ejecutamos el query.
                                    var registro = db.Query<PADRON>(querry);
                                    //  var registro2 = db.Query<VW_RESERVA_VISITA>(que);

                                    var x = string.IsNullOrEmpty(registro.First().CEDULA);
                                    if (string.IsNullOrEmpty(registro.First().CEDULA))
                                    {
                                        await DisplayAlert("Error", "No puede dejar este campo vacio", "Aceptar");
                                        cedula.Text = registro.First().CEDULA;
                                        Nombres.Text = registro.First().NOMBRES + " " + registro.First().APELLIDO1 + " " + registro.First().APELLIDO2;


                                    }
                                    else
                                    {
                                        cedula.Text = registro.First().CEDULA;
                                        Nombres.Text = registro.First().NOMBRES + " " + registro.First().APELLIDO1 + " " + registro.First().APELLIDO2;

                                        Visitas visita = new Visitas
                                        {
                                            Cedula = cedula.Text,
                                            Nombres = Nombres.Text,
                                            Empresa = empresa.Text,
                                            VisitaA = visitaA.Text,

                                        };
                                        using (var datos = new DataAccess())
                                        {
                                            datos.InsertVisita(visita);
                                            listaGrupo.ItemsSource = datos.GetVisitas();
                                        }
                                        cedula.Text = string.Empty;
                                        Nombres.Text = string.Empty;

                                        //DisplayAlert("Confirmacion", "Visita agregada", "Aceptar");
                                        DependencyService.Get<IToastMessage>().DisplayMessage("Visita agregada");

                                    }

                                    db.Close();
                                }
                                catch (Exception ey)
                                {
                                    await Navigation.PushAsync(new RegistroPage(inString, false));
                                    //Documento no econtrado
                                    {

                                        Nombres.Text = string.Empty;
                                        cedula.Text = inString;
                                        /*var properties = new Dictionary<string, string> {
                                            { "Category", "Documento NO EXISTE?" },
                                            { "Code", "MainPage.xaml.cs Line: 179" },
                                            { "Lector", Preferences.Get("LECTOR", "0")}
                                        }; */
                                        //Crashes.TrackError(ey, properties);
                                        //ppCedulaNoExiste.IsVisible = true;

                                        await PopupNavigation.PushAsync(new PopUpCedulaNoEncontradaGrupo());                                        // Analytics.TrackEvent("Documento No-Existe: " + inString + " en el escaner " + Preferences.Get("LECTOR", "N/A"));
                                        // await TextToSpeech.SpeakAsync("Documento No Existe");
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
                        DependencyService.Get<IToastMessage>().DisplayMessage("Ha escaneado un codigo que no es la cedula");
                    }
                }
            }
            catch (Exception ea)
            {
                Debug.WriteLine("Ha ocurrido una excepcion en el metodo entrada, motivo: " + ea.Message);
                Analytics.TrackEvent("Error al verificar si es un id de salida " + ea.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            }

        }

        
        private void ListaGrupo_ItemSelected(object sender, SelectedItemChangedEventArgs e)// Cuando se selecciona un item se convoca este metodo
        {
            var item = e.SelectedItem;
        }


    }
}
