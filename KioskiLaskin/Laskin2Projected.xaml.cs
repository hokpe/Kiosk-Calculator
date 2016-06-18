using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace KioskiLaskin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class Laskin2Projected : Page, iPageUpdateListener
    {
        Tapahtuma tapahtuma = null;

        public Laskin2Projected()
        {
            this.InitializeComponent();
        }

        public void UpdatePage(string field, string text)
        {
            if(field != null && text != null)
            {
                if(field == "saatu")
                {
                    SaatuBox.Text = text;
                }
                else if(field == "Erotus")
                {
                    ErotusBlock.Text = text;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is Tapahtuma)
            {
                float yht_hinta = 0;

                tapahtuma = (Tapahtuma)e.Parameter;
                Tuotenimi.Text = "";
                Tuotemaara.Text = "";
                Yhteishinta.Text = "";
                foreach (MyyntiArtikkeli ma in tapahtuma.myyntitapahtuma.ostoslista.myyntiArtikkelit)
                {
                    if (ma.maara > 0)
                    {
                        Tuotenimi.Text = Tuotenimi.Text + ma.nimi + "\n";
                        Tuotemaara.Text = Tuotemaara.Text + ma.maara.ToString() + " " + Localization.GetLocalizedText("pcs") + "\n";
                        Yhteishinta.Text = Yhteishinta.Text + (ma.maara * ma.hinta).ToString() + " " + Settings.getCurrency() + "\n";
                        yht_hinta += (ma.maara * ma.hinta);
                    }
                    SumBlock.Text = yht_hinta.ToString();
                }
                EuroaBlock.Text = Settings.getCurrency();
                Euroa2Block.Text = Settings.getCurrency();
                Euroa3Block.Text = Settings.getCurrency();
            }
        }
    }
}
