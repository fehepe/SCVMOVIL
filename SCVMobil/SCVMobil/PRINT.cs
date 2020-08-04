using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XF.Bluetooth.Printer.Plugin.Abstractions;

namespace SCVMobil
{
    public interface PRINT: IPrint
    {      
        List<string> DevicesConnected();
    }
}
