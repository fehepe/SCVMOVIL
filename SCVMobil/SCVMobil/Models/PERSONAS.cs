using System;
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

        public PERSONAS(int PERSONA_ID, string NOMBRES_APELLIDOS)
        {
            this.PERSONA_ID = PERSONA_ID;
            this.NOMBRES_APELLIDOS = NOMBRES_APELLIDOS;
        }

        public PERSONAS()//Constructor en blanco necesario para hacer querrys
        {

        }
    }
}
