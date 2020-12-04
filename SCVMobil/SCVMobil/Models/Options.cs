using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCVMobil.Models
{
    public class Options
    {
        [PrimaryKey, AutoIncrement]
        public int ID_Option { get; set; }
        [Unique]
        public string OptionDesc { get; set; }

        public string combo { get; set; }
    }
}
