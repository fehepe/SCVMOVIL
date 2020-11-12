using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCVMobil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTextViews : ContentPage
    {
        public EditTextViews()
        {
            InitializeComponent();
        }

        private void Savebtn_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("texto1", fisrttext.Text);
            Preferences.Set("texto2", secontext.Text);
            Preferences.Set("texto3", thirdtext.Text);
        }
    }
}