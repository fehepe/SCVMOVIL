using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using Xamarin.Essentials;

namespace SCVMobil.Connections
{
    public class FireBirdData
    {
        public FireBirdData()
        {

        }

        public string connectionString()
        {
            return "";
        }

        public bool tryConnection()
        {
            HttpClient _client = new HttpClient();
            _client.Timeout = new TimeSpan(0, 0, 100);

            string url = $"http://{Preferences.Get("REGISTROS_IP", "192.168.1.158") }:{Preferences.Get("REGISTROS_PORT", "4441")}/?sql=";
            string querrySYNC = "SELECT FIRST 2 * FROM COMPANIAS";
            var contentSync = _client.GetStringAsync(url + querrySYNC);
            contentSync.Wait();
            Preferences.Set("SYNC_VSU", contentSync.IsCompleted);

            return contentSync.IsCompleted;
        }



        //Proveer informacion relativo
        public void PublicServices()
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            culture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            Thread.CurrentThread.CurrentCulture = culture;
        }


        
    }
}
