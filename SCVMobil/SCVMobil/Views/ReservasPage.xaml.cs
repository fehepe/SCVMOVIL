using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json;
using SCVMobil.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCVMobil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReservasPage : ContentPage
    {
        private HttpClient _client = new HttpClient();
        private string Url = "";
        private List<VW_RESERVA_VISITA> reser;

        public ReservasPage(VW_RESERVA_VISITA reservas)
        {
            InitializeComponent();


        }

        public ReservasPage(List<VW_RESERVA_VISITA> reser)
        {
            InitializeComponent();
            this.reser = reser;
            cedula.Text = reser.First().VISITAS_DOCUMENTO;
            nombre.Text = reser.First().VISITAS_NOMBRE_COMPLETO;
            empresa.Text = reser.First().EMPRESAS_NOMBRE;
            desde.Text = reser.First().VISITAS_FECHA_VISITA_DESDE.ToString();
            hasta.Text = reser.First().VISITAS_FECHA_VISITA_HASTA.ToString();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();



        }

        private async void BtnImprimir_Clicked(object sender, EventArgs e)
        {



            try
            {
                var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));



                var registroInvitados = new InvitadosReservas();

                //Vamos a buscar la compania seleccionada 

                //Vamos a buscar la persona seleccionada

                var TBL_PERSONAS = db.Query<PERSONAS>("SELECT PERSONA_ID FROM PERSONAS WHERE NOMBRES_APELLIDOS = '" + empresa.Text + "'");
                var TBL_COMPANIAS = db.Query<VW_RESERVA_VISITA>("SELECT VISITAS_DEPARTAMENTO_ID FROM VW_RESERVA_VISITA WHERE VISITAS_DOCUMENTO = '" + cedula.Text + "' order by VISITAS_FECHA_RESERVA desc");
                registroInvitados.Compania_ID = TBL_COMPANIAS.First().VISITAS_DEPARTAMENTO_ID;
                registroInvitados.Nombres = nombre.Text;
                registroInvitados.Apellidos = " ";
                registroInvitados.Fecha_Registro = DateTime.Now;
                registroInvitados.Cargo = cedula.Text;
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
                registroInvitados.Visitado = reser.First().VISITAS_VISITA_A_ID;
                registroInvitados.Lector = int.Parse(Preferences.Get("LECTOR", "1"));

                db.Insert(registroInvitados);

            }
            catch (Exception ey)
            {
                await DisplayAlert("Error", ey.Message, "OK");
                Debug.WriteLine("Error en btnImprimir");
                Analytics.TrackEvent("Error al insertar registros de invitados  " + ey.Message + "\n Escaner: " + Preferences.Get("LECTOR", "N/A"));
            
                throw;
            }
            await Navigation.PopToRootAsync();
        }

    }

}