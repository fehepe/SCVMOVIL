using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SCVMobil.Models
{
    public class PADRON
    {
        [PrimaryKey]//Seteamos la columna CEDULA como primarykey
        public string CEDULA { get; set; }

        public string NOMBRES { get; set; }

        public string APELLIDO1 { get; set; }

        public string APELLIDO2 { get; set; }
        public string ID_PADRON { get; set; }

        public PADRON(string sCedula, string sNombres, string sApellido1, string sApellido2) //Constructor para la tabla
        {

            CEDULA = sCedula;
            NOMBRES = sNombres;
            APELLIDO1 = sApellido1;
            APELLIDO2 = sApellido2;

        }

        public PADRON()//Constructor en blanco necesario para hacer querrys
        {

        }
    }
}
