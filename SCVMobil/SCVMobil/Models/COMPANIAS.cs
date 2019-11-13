using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SCVMobil.Models
{
    public class COMPANIAS
    {
        [PrimaryKey]
        public int COMPANIA_ID { get; set; }

        public string NOMBRE { get; set; }


        public COMPANIAS(int COMPANIA_ID, string NOMBRE)
        {
            this.COMPANIA_ID = COMPANIA_ID;
            this.NOMBRE = NOMBRE;

        }

        public COMPANIAS()//Constructor en blanco necesario para hacer querrys
        {

        }
    }
}
