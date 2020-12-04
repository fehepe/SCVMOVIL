using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCVMobil.Models
{
    public class OptionsIn
    {
        [PrimaryKey, AutoIncrement]
        public int ID_Option_In { get; set; }
        [Unique]
        public string OptionDescIn { get; set; }
    }
}
