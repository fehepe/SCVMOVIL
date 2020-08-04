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
using XF.Bluetooth.Printer.Plugin.Abstractions;
[assembly: Xamarin.Forms.Dependency(typeof(Prints))]

namespace SCVMobil.Droid
{
    public class Prints : IPrint
    {
        //public List<string> DevicesConnected()
        //{
        //    List<string> devicesName = new List<string>();

        //    using (BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
        //    {
        //        if (bluetoothAdapter == null)
        //        {
        //            throw new Exception("No default adapter");
        //        }

        //        if (!bluetoothAdapter.IsEnabled)
        //        {
        //            throw new Exception("Bluetooth not enabled");
        //        }

        //        List<BluetoothDevice> device = (from bd in bluetoothAdapter.BondedDevices
        //                                        where bd.Name != null
        //                                        select bd).ToList();

        //        foreach (var item in device)
        //        {
        //            devicesName.Add(item.Name);
        //        }
        //    }
        //    if (devicesName.Any())
        //    {
        //        return devicesName;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public async Task PrintText(string input, string printerName)
        {
            using (BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                if (bluetoothAdapter == null)
                {
                    throw new Exception("No default adapter");
                    
                    //return;
                }

                if (!bluetoothAdapter.IsEnabled)
                {
                    throw new Exception("Bluetooth not enabled");
                    //Intent enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                    //StartActivityForResult(enableIntent, REQUEST_ENABLE_BT);
                    // Otherwise, setup the chat session
                }


                BluetoothDevice device = (from bd in bluetoothAdapter.BondedDevices
                                          where bd.Name == printerName
                                          select bd).FirstOrDefault();
                if (device == null)
                    throw new Exception(printerName + " device not found.");

                try
                {
                    using (BluetoothSocket _socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb")))
                    {
                        await _socket.ConnectAsync();

                        //byte[] buffer = System.Text.Encoding.UTF8.GetBytes("EZ" +
                        //  "{AHEAD:20}" +
                        //  "{PRINT, STOP 300:" +
                        //  $"@10,15:MF204,HMULT2,VMULT3|holaaa|" +
                        //  "@75,15:MF204||" +
                        //  "@100,15:MF204|Size: 3 Libras|" +
                        //  $"@100,150:MF185,HMULT2,VMULT4|holiii|" +
                        //  "@125,15:MF204|Codigo:12454548|" +
                        //  "@150,15:MF204||" +
                        //  "@180,20:UPC-A,WIDE 2, HIGH 8|00000000001|" +
                        //  "@180,230:MF204||" +
                        //  "@205,280:MF204|Tarjeta:No |" +
                        //  "@225,50:MF204|0000000000001|" +
                        //  "@230,210:MF204|07/29/20      Tax:|" +
                        //  "}");
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(input);
                        await Task.Delay(3000);
                        // Write data to the device
                        await _socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        _socket.Close();
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