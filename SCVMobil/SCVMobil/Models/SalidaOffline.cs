using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCVMobil
{
    class SalidaOffline
    {
        [PrimaryKey]
        public int? INVIDATO_ID { get; set; }
        public DateTime? Fecha_Salida { get; set; }
        public bool? Subida { get; set; }
    }
}
