﻿using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCVMobil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopUpPing : PopupPage
    {
        public PopUpPing()
        {
            InitializeComponent();
        }

        private void okping_Clicked(object sender, EventArgs e)
        {
            this.IsVisible = false;
        }
    }
}