using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Honeywell.AIDC.CrossPlatform;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SCVMobil
{
    //Clase generica del Scanner para acceder a el desde cualquier pantalla
    public class Escaner
    {
        private Dictionary<string, BarcodeReader> mBarcodeReaders;
        private Action<string> method2Call;
        private BarcodeReader mSelectedReader = null;

        public Escaner(Action<string> method2Call)
        {
            this.method2Call = method2Call;
            mBarcodeReaders = new Dictionary<string, BarcodeReader>();
            
        }

        public async void OpenScanner(BarcodeReader mSelectedReader)
        {
            Debug.WriteLine("DataSetting: " + mSelectedReader.SettingKeys.DataProcessorLaunchBrowser);
            BarcodeReader.Result result = await mSelectedReader.OpenAsync();

            if (result.Code == BarcodeReader.Result.Codes.SUCCESS ||
                result.Code == BarcodeReader.Result.Codes.READER_ALREADY_OPENED)
            {
                Debug.WriteLine("Scanner oppened");
                SetScannerAndSymbologySettings(mSelectedReader);
            }
        }

        public async void CloseScanner(BarcodeReader mSelectedReader)
        {
            BarcodeReader.Result result = await mSelectedReader.CloseAsync();

            if (result.Code == BarcodeReader.Result.Codes.SUCCESS ||
                result.Code == BarcodeReader.Result.Codes.NO_ACTIVE_CONNECTION)
            {
                Debug.WriteLine("Scanner closed");
            }
        }

        private void MBarcodeReader_BarcodeDataReady(object sender, BarcodeDataArgs e)
        {
            Debug.WriteLine("SCAN EVENT");

            //e.Data.ToString() es la data que leyo el escaner
           
            Device.BeginInvokeOnMainThread(() =>
            {
                //Vamos a poner la data que se escaneo en el campo de boleta
                //entCedula.Text = e.Data.ToString();
                Debug.WriteLine("Data: " + e.Data.ToString());
                method2Call(e.Data.ToString());
                //Invocamos el evento de escaneo completado.
                //EntCedula_Completed(entCedula, new EventArgs());
            });
            

        }

        public BarcodeReader GetBarcodeReader(string readerName)
        {
            BarcodeReader reader = null;

            if (readerName == "default")
            { // This name was added to the Open Reader picker list if the
              // query for connected barcode readers failed. It is not a
              // valid reader name. Set the readerName to null to default
              // to internal scanner.
                readerName = null;
            }

            if (null == readerName)
            {
                if (mBarcodeReaders.ContainsKey("default"))
                {
                    reader = mBarcodeReaders["default"];
                }
            }
            else
            {
                if (mBarcodeReaders.ContainsKey(readerName))
                {
                    reader = mBarcodeReaders[readerName];
                }
            }

            if (null == reader)
            {
                // Create a new instance of BarcodeReader object.
                reader = new BarcodeReader(readerName);

                // Add an event handler to receive barcode data.
                // Even though we may have multiple reader sessions, we only
                // have one event handler. In this app, no matter which reader
                // the data come frome it will update the same UI controls.
                reader.BarcodeDataReady += MBarcodeReader_BarcodeDataReady;

                // Add the BarcodeReader object to mBarcodeReaders collection.
                if (null == readerName)
                {
                    mBarcodeReaders.Add("default", reader);
                }
                else
                {
                    mBarcodeReaders.Add(readerName, reader);
                }
            }

            return reader;
        }

        public async void GetScanner(bool bOpenClose)
        {
            IList<BarcodeReaderInfo> readerList = await BarcodeReader.GetConnectedBarcodeReaders();
            foreach (BarcodeReaderInfo reader in readerList)
            {
                Debug.WriteLine("SCANNER FOUND: " + reader.ScannerName.ToString());
            }
            if (readerList.Count > 0)
            {
                if (bOpenClose)
                {
                    OpenScanner(GetBarcodeReader(readerList[0].ScannerName));
                    Debug.WriteLine("GOT SCANNER");
                }
                else
                {
                    CloseScanner(GetBarcodeReader(readerList[0].ScannerName));
                    Debug.WriteLine("CLOSED SCANNER: " + readerList[0].ScannerName);
                }

            }
            else
            {
                OpenScanner(GetBarcodeReader("default"));
                Debug.WriteLine("NO SCANNER");
            }
        }
        private async void SetScannerAndSymbologySettings(BarcodeReader lector)
        {
            try
            {
                if (lector.IsReaderOpened)
                {
                    Dictionary<string, object> settings = new Dictionary<string, object>()
                    {
                        {lector.SettingKeys.DataProcessorLaunchBrowser, false }
                        
                    };

                    BarcodeReader.Result result = await lector.SetAsync(settings);
                    if (result.Code != BarcodeReader.Result.Codes.SUCCESS)
                    {
                        Debug.WriteLine("DONE CONFIG SCANNER");
                    }
                }
            }
            catch (Exception exp)
            {
                //await DisplayAlert("Error", "Symbology settings failed. Message: " + exp.Message, "OK");
                Debug.WriteLine("Excepcion: " + exp);
            }
        }
    }
}
