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
    public sealed partial class Tilasto : Page
    {
        public Tilasto()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            float yht_hinta = 0;
            BackStackClass.SetBackButtonVisibility();
            Myyntitapahtuma TapahtumanMyyntiTapahtumat = new Myyntitapahtuma();

            if (e.Parameter is Tapahtuma)
            {
                Tapahtuma t = (Tapahtuma)e.Parameter;
                TapahtumanMyyntiTapahtumat.uusiOstoslista(t.hinnasto);
                foreach (Myyntitapahtuma mt in t.Myyntitapahtumat)
                {
                    foreach (MyyntiArtikkeli ma in mt.ostoslista.myyntiArtikkelit)
                    {
                        foreach (MyyntiArtikkeli tma in TapahtumanMyyntiTapahtumat.ostoslista.myyntiArtikkelit)
                        {
                            if (ma.nimi == tma.nimi)
                            {
                                tma.maara += ma.maara;
                            }
                        }
                    }
                }
                TapahtumanNimi.Text = "Tapahtuma: " + t.nimi;
                Tuotenimi.Text = "";
                Tuotemaara.Text = "";
                Yhteishinta.Text = "";
                foreach (MyyntiArtikkeli ma in TapahtumanMyyntiTapahtumat.ostoslista.myyntiArtikkelit)
                {
                    Tuotenimi.Text = Tuotenimi.Text + ma.nimi + "\n";
                    Tuotemaara.Text = Tuotemaara.Text + ma.maara.ToString() + " " + Localization.GetLocalizedText("pcs") + "\n";
                    Yhteishinta.Text = Yhteishinta.Text + (ma.maara * ma.hinta).ToString() + " " + Settings.getCurrency() + "\n";
                    yht_hinta += (ma.maara * ma.hinta);
                    SumBlock.Text = yht_hinta.ToString();
                }
                EuroaBlock.Text = Settings.getCurrency();
            }
            else
            {
                /* Problem */
                throw new NotImplementedException();
            }
        }
    }
}
