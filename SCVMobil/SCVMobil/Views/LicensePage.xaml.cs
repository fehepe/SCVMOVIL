using Plugin.DeviceInfo;
using SCVMobil.Connections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCVMobil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LicensePage : ContentPage
    {
        const int Counter = 130;
        String SerialNumber;
        VerifyHash verification = new VerifyHash(Counter);
        public LicensePage()
        {
            InitializeComponent();
            LblSerialNumber.Text = SerialNumber =CrossDeviceInfo.Current.Id;
            Debug.WriteLine("El Serial number es: " + CrossDeviceInfo.Current.Id);
        }

        private async void BtnIngresar_Clicked(object sender, EventArgs e)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                if (verification.VerifyMd5Hash(md5Hash, SerialNumber, EntLicense.Text))
                {
                    Preferences.Set("IS_SET", true);
                    Debug.WriteLine("Preferece IS_SET, ha cambiado a TRUE");
                    await Navigation.PushAsync(new MainPage());
                }
                else
                {
                    Debug.WriteLine("Licencia Invalida");
                    await DisplayAlert("Error", "Licencia Invalida", "Reintentar");
                }
            }
        }
    }
}