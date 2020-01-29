using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCVMobil.Models
{
    public class COMPANIASLOC
    {
        [PrimaryKey]
        public int COMPANIA_ID { get; set; }
        public string NOMBRE { get; set; }
        public int? PUNTO_VSU { get; set; }
        public int? ESTATUS { get; set; }


        public COMPANIASLOC()//Constructor en blanco necesario para hacer querrys
        {

        }
    }
}
