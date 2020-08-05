using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCVMobil
{
    public class Invitados
    {

        [PrimaryKey]
        public int? INVIDATO_ID { get; set; }
        public bool? Subida { get; set; }
        public bool? salidaSubida { get; set; }
        public bool? verificacionSubida { get; set; }
        public int? SALIDA_ID { get; set; }
        public int Compania_ID { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public int? Puerta_Registro { get; set; }
        public DateTime? Fecha_Verificacion { get; set; }
        public DateTime Fecha_Registro { get; set; }
        public DateTime? Fecha_Salida { get; set; }
        public string Tipo { get; set; }
        public string Cargo { get; set; }
        public int? Tiene_Activo { get; set; }
        public int? Estatus_ID { get; set; }
        public int? Modulo { get; set; }
        public int? Empresa_ID { get; set; }
        public string Placa { get; set; }
        public string Tipo_Visitante { get; set; }
        public int? Es_Grupo { get; set; }
        public int? Grupo_ID { get; set; }
        public int? Puerta_Entrada { get; set; }
        public int? Actualizada_La_Salida { get; set; }
        public int? Horas_Caducidad { get; set; }
        public int? Personas { get; set; }
        public int? In_Out { get; set; }
        public string Origen_Entrada { get; set; }
        public string Origen_Salida { get; set; }
        public string Comentario { get; set; }
        public int? Origen_IO { get; set; }
        public int? Actualizado { get; set; }
        public string Cpost { get; set; }
        public string Texto1_Entrada { get; set; }
        public string Texto2_Entrada { get; set; }
        public string Texto3_Entrada { get; set; }
        public string Secuencia_Dia { get; set; }
        public string No_Aplica_Induccion { get; set; }
        public int? Visitado { get; set; }
        public int Lector { get; set; }
        public string Codigo_carnet { get; set; }
        public string CODIGO_BARRA { get; set; }

        public Invitados()
        {

        }
    }
}