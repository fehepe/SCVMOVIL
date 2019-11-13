using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCVMobil
{
    class InvitadosTmp
    {
     
        public int Invidato_ID { get; set; }
        public int Compania_ID { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }

        public string cargo { get; set; }
        public DateTime Fecha_Registro { get; set; }
        public int Ya_Salio {get; set;}

        public InvitadosTmp()
        {

        }
        
    }
}
