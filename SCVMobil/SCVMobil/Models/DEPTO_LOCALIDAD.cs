using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCVMobil.Models
{
    public class DEPTO_LOCALIDAD
    {
        [PrimaryKey]
        public int ID_DEPTO_LOCALIDAD { get; set; }
        public int ID_DEPARTAMENTO { get; set; }
        public string DEPTO_NOMBRE { get; set; }
        public int ID_LOCALIDAD { get; set; }
        public string LOCALIDAD_NOMBRE { get; set; }

        public DEPTO_LOCALIDAD()
        {

        }
    }
}
