using Newtonsoft.Json;
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
    public partial class Reservas : ContentPage
    {
        private HttpClient _client = new HttpClient();
        private string Url = "";
        private List<VW_RESERVA_VISITA> reser;

        public Reservas(VW_RESERVA_VISITA reservas)
        {
            InitializeComponent();
          
            
        }

        public Reservas(List<VW_RESERVA_VISITA> reser)
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
          
         

            //picker.ItemsSource = Preferences.Get("EMPRESAS_LIST", "").Split(',').ToList<string>();


        

       
            
        }

        private async void BtnImprimir_Clicked(object sender, EventArgs e)
        {
        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
         {
                string prueba = "^XA" +
"^CF0,60" +
"^FO50,50^GB100,100,100^FS" +
"^FO75,75^FR^GB100,100,100^FS" +
"^FO88,88^GB50,50,50^FS" +
"^FO220,50^FDGAD Intermec^FS" +
"^ CF0,40" +
 "^ FO220,100^FDVisita:^FS" +
     "^FO320,100^FDVacio^FS" +
      "^FO220,135^FDFecha:^FS" +
          "^FO320,135^FD27 / 06 / 2019^FS" +
          "^FO220,170^FDHora: ^FS" +
              "^FO320,170^FD03:50:03PM^FS" +
                 "^FO50,220^GB700,1,3^FS" +
                   "^BY2,1,60" +
                   "^FO300,300^BC^FD" + cedula.Text +"^FS" +
                     "^CFA,30" +
                       "^FO200,250 ^FD" + nombre.Text + "^FS" +
                          "^XZ";
            try
            {
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                client.Connect("192.168.1.123", 9100);

                // Write ZPL String to connection
                System.IO.StreamWriter writer = new System.IO.StreamWriter(client.GetStream());
                writer.Write(prueba);
                writer.Flush();

                // Close Connection
                writer.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error de impresora" + ex.Message, "Aceptar");
            }
            }
            else
            {
                //await DisplayAlert("Error", "No esta conectado a ningua red", "Acceptar");
            }

            var db = new SQLiteConnection(Preferences.Get("DB_PATH", ""));



            var registroInvitados = new Invitados();

            //Vamos a buscar la compania seleccionada 



            //Vamos a buscar la persona seleccionada

            var TBL_PERSONAS = db.Query<PERSONAS>("SELECT PERSONA_ID FROM PERSONAS WHERE NOMBRES_APELLIDOS = '" + empresa.Text + "'");

            registroInvitados.Compania_ID = 1000;
            registroInvitados.Nombres = nombre.Text;
            registroInvitados.Apellidos =" ";
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

            await Navigation.PopToRootAsync();
        }

        }

    }
