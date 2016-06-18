using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace KioskiLaskin
{
    public sealed partial class Laskin2 : Page
    {
        Tapahtuma tapahtuma = null;
        private string SaatuBoxOkString = "0";

        public Laskin2()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BackStackClass.SetBackButtonVisibility();
            BackStackClass.Navigate(typeof(Laskin2Projected), e.Parameter);

            if (e.Parameter != null && e.Parameter is Tapahtuma)
            {
                float yht_hinta = 0;

                tapahtuma = (Tapahtuma)e.Parameter;
                TapahtumaNimi.Text = Localization.GetLocalizedTextWithVariables("EventX", tapahtuma.nimi);
                //TapahtumaNimi.Text = "Tapahtuma:\n" + tapahtuma.nimi;
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

        private void ValmisClick(object sender, RoutedEventArgs e)
        {
            tapahtuma.LisaaMyynti();
            tapahtuma.TallennaTapahtuma();
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void SaatuValmis(object sender, RoutedEventArgs e)
        {
            float saatu;
            bool parsed = float.TryParse(SaatuBox.Text, out saatu);
            if (parsed)
            {
                float takaisin;
                float summa;
                parsed = float.TryParse(SumBlock.Text, out summa);
                if (parsed)
                {
                    takaisin = saatu - summa;
                    ErotusBlock.Text = takaisin.ToString();
                }
            }
        }
        private void SaatuTextChanged(object sender, TextChangedEventArgs e)
        {
            string s = SaatuBox.Text;
            float saatu;
            float takaisin;
            float summa;
            if (s != "")
            {
                bool isNumeric = float.TryParse(s, out saatu);
                if (isNumeric)
                {
                    SaatuBoxOkString = s;
                    isNumeric = float.TryParse(SumBlock.Text, out summa);
                    if (isNumeric)
                    {
                        takaisin = saatu - summa;
                        ErotusBlock.Text = takaisin.ToString();
                        BackStackClass.WindowNotify("saatu", saatu.ToString());
                        BackStackClass.WindowNotify("Erotus", takaisin.ToString());
                    }
                }
                else
                {
                    SaatuBox.Text = SaatuBoxOkString;
                    SaatuBox.SelectionStart = SaatuBox.Text.Length; // Sets the cursor to the end of the text
                    SaatuBox.SelectionLength = 0;                   // Sets the cursor to the end of the text
                    BackStackClass.WindowNotify("saatu", SaatuBoxOkString);
                }
            }
            else // s == ""
            {
                BackStackClass.WindowNotify("saatu", "");
            }
        }
    }
}
