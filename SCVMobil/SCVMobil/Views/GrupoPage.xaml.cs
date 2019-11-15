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
            this.cedula.Text = cedula;
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


        public void refreshPage()
        {

        }
        protected override void OnDisappearing()// Cuando desaparezca la pantalla, apagamos el scanner
        {
            base.OnDisappearing();
            scanner.GetScanner(false);
            ppCedulaNoExiste.IsVisible = false;
           

        }

        private void Cedula_Completed(object sender, EventArgs e)
        {
            entrada(cedula.Text);
        }
        public void cedulaScanned(string inString)// Metodo que se invoca cuando se scanea un documento
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
        private void BtOKCedulaNoExiste_Clicked(object sender, EventArgs e)
        {

            Navigation.PushAsync(new MainPage());
        }
        protected override void OnAppearing() //Cuando aparesca la pagina, refrescamos.
        {
            Debug.WriteLine("Appeared");

            scanner.GetScanner(true);
            cedula.Text = string.Empty;
            Nombres.Text = string.Empty;
             
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)//Metodo para imprimir los grupos
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
                registroInvitados.Puerta_Entrada = 2;
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
                    registroInvitados.Puerta_Entrada = 2;
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
                    registroInvitados.Visitado = null;
                    registroInvitados.Lector = int.Parse(Preferences.Get("LECTOR", "1"));
                    db.Insert(registroInvitados);
                }

            await Navigation.PopToRootAsync();
          }
        }

        public async void entrada(String inString)
        {
            if (inString != "")
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                //Vamos a ver si es un ID de salida.
                if (inString.StartsWith("ID"))
                {
                    var querry = "SELECT * FROM Invitados WHERE INVIDATO_ID = " + inString.Replace("ID", "") + " AND FECHA_SALIDA is null";
                    var registroInv = db.Query<Invitados>(querry);
                    if (registroInv.Any())
                    {
                        if (registroInv.First().Fecha_Salida is null)
                        {
                            registroInv.First().Fecha_Salida = DateTime.Now;
                            registroInv.First().salidaSubida = null;
                            db.UpdateAll(registroInv);
                            //await DisplayAlert("Salida", "Se ha dado salida correctamente.", "OK");
                            DependencyService.Get<IToastMessage>().DisplayMessage("Se ha dado salida correctamente.");
                        }
                        else
                        {
                            //await DisplayAlert("Error", "Esta persona ya salió.", "OK");
                            DependencyService.Get<IToastMessage>().DisplayMessage("Esta persona ya salió.");
                        }

                    }
                    else
                    {
                        // await DisplayAlert("Error", "No existe este ID de salida", "OK");
                        DependencyService.Get<IToastMessage>().DisplayMessage("No existe este ID de salida");

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
                         db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));
                         querry = "SELECT * FROM Invitados WHERE CARGO = '" + inString + "' AND FECHA_SALIDA is null";
                        var registroInv = db.Query<Invitados>(querry);
                        registroInv.First().Fecha_Salida = DateTime.Now;
                        registroInv.First().salidaSubida = null;
              
                        db.UpdateAll(registroInv);

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


                            if (registro.First().CEDULA == " ")
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
                                await DisplayAlert("Error", "Este documento ya esta en la lista o no existe en el Padron", "Aceptar");
                                Analytics.TrackEvent("Documento No-Existe: " + inString + " en el escaner " + Preferences.Get("LECTOR", "N/A"));
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

        }
        private void ListaGrupo_ItemSelected(object sender, SelectedItemChangedEventArgs e)// Cuando se selecciona un item se convoca este metodo
        {
            var item = e.SelectedItem;
        }


    }
}
