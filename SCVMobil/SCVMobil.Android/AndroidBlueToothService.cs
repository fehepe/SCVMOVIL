using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using SCVMobil.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidBlueToothService))]
namespace SCVMobil.Droid
{
    class AndroidBlueToothService : PRINT
    {
        public string GetBluetoothDeviceName()
        {            
            using (BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                var btdevice = bluetoothAdapter?.BondedDevices.Select(i => i.Name).ToList();
                return "MPA52186";
            }
        }

        public IList<string> GetDeviceList()
        {
            using (BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                var btdevice = bluetoothAdapter?.BondedDevices.Select(i => i.Name).ToList();
                return btdevice;
            }
        }

        public async Task Print(string deviceName, Invitados invitados, byte[] vs)
        {
            using (BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                BluetoothDevice device = (from bd in bluetoothAdapter?.BondedDevices
                                          where bd?.Name == deviceName
                                          select bd).FirstOrDefault();
                try
                {
                    using (BluetoothSocket bluetoothSocket = device?.
                        CreateRfcommSocketToServiceRecord(
                        UUID.FromString("00001101-0000-1000-8000-00805f9b34fb")))
                    {
                        bluetoothSocket?.Connect();
                        //byte[] buffer = Encoding.UTF8.GetBytes("EZ" +
                        //    "{AHEAD:20}" +
                        //    "{PRINT, STOP 300:" +
                        //    $"@10,15:MF204,HMULT2,VMULT3|{invitados.Nombres}|" +
                        //    "@75,15:MF204||" +
                        //    "@100,15:MF204|Size: 3 Libras|" +
                        //    $"@100,150:MF185,HMULT2,VMULT4|{invitados.Fecha_Registro}|" +
                        //    "@125,15:MF204|Codigo:12454548|" +
                        //    "@150,15:MF204||" +
                        //    "@180,20:UPC-A,WIDE 2, HIGH 8|00000000001|" +
                        //    "@180,230:MF204||" +
                        //    "@205,280:MF204|Tarjeta:No |" +
                        //    "@225,50:MF204|0000000000001|" +
                        //    "@230,210:MF204|07/29/20      Tax:|" +
                        //    "}");
                        bluetoothSocket?.OutputStream.Write(vs, 0, vs.Length);
                        bluetoothSocket.Close();
                    }
                }
                catch (Exception exp)
                {
                    throw exp;
                }
            }
        }

    }
}