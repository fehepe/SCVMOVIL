using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SCVMobil
{
    public interface PRINT
    {
        IList<string> GetDeviceList();
        Task Print(string deviceName, Invitados invitados, byte[] vs);
        string GetBluetoothDeviceName();
    }
}
