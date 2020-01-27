﻿using System;
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
        public int? PUNTO_VSU { get; set; }
        public int? ESTATUS { get; set; }



        public COMPANIAS(int COMPANIA_ID, string NOMBRE, int PUNTO_VSU, int ESTATUS)
        {
            this.COMPANIA_ID = COMPANIA_ID;
            this.NOMBRE = NOMBRE;
            this.PUNTO_VSU = PUNTO_VSU;
            this.ESTATUS = ESTATUS;

        }

        public COMPANIAS()//Constructor en blanco necesario para hacer querrys
        {

        }
    }
}
