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
using SCVMobil.Models;
using Rg.Plugins.Popup.Services;
using SCVMobil.Connections;

namespace SCVMobil
{
    public partial class MainPage : ContentPage
    {
      
        Escaner scanner;
        
        
        //---------------------------------------------------------------------------

        public MainPage()//Constructor
        {
            
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                
                    Device.BeginInvokeOnMainThread(() => refreshPage());
                    Debug.WriteLine("Refreshed");
                return true;
                
            });
          
            InitializeComponent();
            scanner = new Escaner(cedulaScanned);
            
            DeviceDisplay.KeepScreenOn = false;




        }


        //--------------------------------------------------------------------------------------
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new PopupView());  //Popup para entrar la contraseña e ir a la configuracion//
        }


        //Refrescar la pagina
        //---------------------------------------------------------------------------------------
        public void refreshPage()
        {
            
            try
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
            catch (Exception e)
            {
                Debug.WriteLine("Error en el refresh" + e);
                throw;
            }
        }
       
        //-----------------------------------------------------------------------------------------
        protected override void OnAppearing() //Cuando aparezca la pagina, refrescamos.
        {

           
            Debug.WriteLine("Appeared");
            refreshPage();
            scanner.GetScanner(true);
            entCedula.Text = string.Empty;
            entApellidos.Text = string.Empty;
        }
        
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        protected override void OnDisappearing()//Cuando la pagina desaparezca
        {
            scanner.GetScanner(false);// Se desactiva el Scaner
           
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
     

        //Boton de agregar cedula.


        //Boton de Limpiar los campos
        private void BtClear_Clicked(object sender, EventArgs e)
        {
            entApellidos.Text = "";
            entCedula.Text = "";
            entNombres.Text = "";
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [Obsolete]
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

                                querry = "SELECT * FROM VW_RESERVA_VISITA WHERE VISITAS_DOCUMENTO = '" + inString + "' and visitas_Fecha_reserva=" + DateTime.Today.Ticks;


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
                                        await PopupNavigation.PushAsync(new PopUpCedulaNoexiste());  //Invocacion del PopUp para mostrar mesaje de error// 
                                        await Navigation.PushAsync(new RegistroPage(entCedula.Text, true));
                                        Debug.WriteLine("Entrada");
                                        Analytics.TrackEvent("Error al buscar cedula en el padron" + ey.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));

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
                Debug.Write("Error en escaneo de cedula: " + ea.Message);
                //throw;
            }
        }
    
    }
    
}



