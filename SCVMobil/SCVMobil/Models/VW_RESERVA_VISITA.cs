using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SCVMobil.Models
{
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
}
