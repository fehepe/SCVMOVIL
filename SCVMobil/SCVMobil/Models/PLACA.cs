using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SCVMobil.Models
{
    public class PLACA
    {
        [PrimaryKey]
        public string CODIGO { get; set; }

        public string PLACA_CODE { get; set; }


        public PLACA(string CODIGO, string PLACA_CODE)
        {
            this.CODIGO = CODIGO;
            this.PLACA_CODE = CODIGO;

        }

        public PLACA()//Constructor en blanco necesario para hacer querrys
        {

        }
    }
}
