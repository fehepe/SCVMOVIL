﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SCVMobil.Models
{
    public class PERSONAS
    {
        [PrimaryKey]
        public int PERSONA_ID { get; set; }
        public string NOMBRES_APELLIDOS { get; set; }
        public int DEPARTAMENTO_ID { get; set; }
        public string DOCUMENTO { get; set; }


        public PERSONAS()//Constructor en blanco necesario para hacer querrys
        {

        }
    }
}
