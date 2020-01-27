using SQLite;

using System;
using System.Collections.Generic;
using System.Text;

namespace SCVMobil
{
    public class Visitas
    {
        [PrimaryKey]
        public string Cedula { get; set; }
        public string Nombres { get; set; }
        public string Empresa { get; set; }
        public string VisitaA { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Cedula,
                Nombres, Empresa, VisitaA);
        }
    }
}
