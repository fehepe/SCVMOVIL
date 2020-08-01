using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SCVMobil
{
    public interface PRINT
    {      
        Task Print(string deviceName, Invitados invitados);        
    }
}
