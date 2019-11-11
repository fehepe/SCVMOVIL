using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SCVMobil.Droid;
using Xamarin.Forms;
using Android.Views.InputMethods;

[assembly: Dependency(typeof(Message_Droid))]
namespace SCVMobil.Droid
{
    class Message_Droid : IToastMessage
    {
        public void DisplayMessage(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }
    }
}