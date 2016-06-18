using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.UI.ViewManagement;

namespace KioskiLaskin
{
    public sealed partial class Laskin : Page 
    {
        Tapahtuma tapahtuma = null;

        private bool NimiEditorChanged = false;

        public Laskin()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BackStackClass.SetBackButtonVisibility();

            if (e.Parameter != null )
            {
                tapahtuma = Tapahtuma.getTapahtuma();
                if (tapahtuma == null)
                {
                    if (e.Parameter is Tapahtuma)
                    {
                        tapahtuma = (Tapahtuma)e.Parameter;
                        Tapahtuma.UpdateTapahtuma(tapahtuma);
                    }
                    else if (e.Parameter is Hinnasto)
                    {
                        tapahtuma = new Tapahtuma();
                        DateTime localDate = DateTime.Today;
                        tapahtuma.Paivamaara = localDate.ToString();
                        tapahtuma.Paivamaara = tapahtuma.Paivamaara.Substring(0, tapahtuma.Paivamaara.Length - 9); // - " 00:00:00"
                        {
                            tapahtuma.MaaritaHinnasto((Hinnasto)e.Parameter);
                            NimiTapahtumalle();
                        }
                    }
                    else
                    {
                        /* Problem */
                        throw new NotImplementedException();
                    }
                }
                if (tapahtuma.myyntitapahtuma == null)
                {
                    tapahtuma.LisaaMyyntiTapahtuma();
                }
                if (listArtikkelit.Visibility == Visibility.Visible)
                {
                    PiilotaEditori();
                    PageEnabled(true);
                }

                EuroaBlock.Text = Settings.getCurrency();
                BackStackClass.Navigate(typeof(Laskin_Projected), e.Parameter);
                calculateSum();
            }
        }

        public void UpdatePage()
        {
            MyyntiArtikkeli ma = (MyyntiArtikkeli)listArtikkelit.SelectedItem;
            /* Reffress list */
            listArtikkelit.ItemsSource = null;
            listArtikkelit.ItemsSource = tapahtuma.myyntitapahtuma.ostoslista.myyntiArtikkelit;
            listArtikkelit.SelectedItem = ma;
            calculateSum();
        }

        private void NimiTapahtumalle()
        {
            EditBox.Text = tapahtuma.hinnasto.nimi;
            EditBox.Visibility = Visibility.Visible;
            EditBoxHeader.Visibility = Visibility.Visible;
            TapahtumaSelitys.Visibility = Visibility.Visible;
            TapahtumaQuery.Visibility = Visibility.Visible;
            TapahtumaQuery.Opacity = 1;
            /*{
                DoubleAnimation da = new DoubleAnimation();
                da.From = 1;
                da.To = 0;
                da.Duration = new Duration(TimeSpan.FromSeconds(2));
                da.AutoReverse = true;
                /da.RepeatBehavior = RepeatBehavior.Forever;
                da.RepeatBehavior=new RepeatBehavior(3);
                CancelButton.BeginAnimation(OpacityProperty, da);
            }*/
            PageEnabled(false);
        }

        private void PageEnabled(bool enable)
        {
            Visibility v = (enable ? Visibility.Visible : Visibility.Collapsed);
            //SolidColorBrush c = new SolidColorBrush((enable ? Colors.Black : Colors.Gray));
            double o = (enable ? 1 : 0.3);
            LisaaTuotetta.Opacity = o;
            VähennäTuotetta.Opacity = o;
            listArtikkelit.IsEnabled = enable;
            LisaaTuotetta.IsEnabled = enable;
            VähennäTuotetta.IsEnabled = enable;
            ValmisButton.IsEnabled = enable;
            CancelButton.IsEnabled = enable;
            listArtikkelit.Visibility = v;
            yhteensaBlock.Opacity = o;
            SumBlock.Opacity = o;
            EuroaBlock.Opacity = o;
            yhteensaBorder.Opacity = o;
            if (listArtikkelit.Visibility == Visibility.Visible)
            {
                listArtikkelit.ItemsSource = tapahtuma.myyntitapahtuma.ostoslista.myyntiArtikkelit;
                TapahtumaNimi.Text = Localization.GetLocalizedTextWithVariables("EventX", tapahtuma.nimi);
                //TapahtumaNimi.Text = "Tapahtuma:\n" + tapahtuma.nimi;
            }
        }

        private void ValmisClick(object sender, RoutedEventArgs e)
        {
            UInt16 maara = 0;
            foreach (MyyntiArtikkeli ma in tapahtuma.myyntitapahtuma.ostoslista.myyntiArtikkelit)
            {
                maara += ma.maara;
            }
            if(maara > 0)
            {
                this.Frame.Navigate(typeof(Laskin2), tapahtuma);
                //BackStackClass.Navigate(this.Frame, typeof(Laskin2), typeof(Laskin2), tapahtuma);
            }
        }

        private void LisaaTuotettaClick(object sender, RoutedEventArgs e)
        {
            MyyntiArtikkeli ma = (MyyntiArtikkeli)listArtikkelit.SelectedItem;
            
            // Laskin l2;
            if (ma != null)
            {
                ma.maara++;
                BackStackClass.WindowNotify(null, null);
                UpdatePage();
            }
        }

        private void VähennäTuotettaClick(object sender, RoutedEventArgs e)
        {
            MyyntiArtikkeli ma = (MyyntiArtikkeli)listArtikkelit.SelectedItem;
            if (ma != null)
            {
                if (ma.maara > 0) ma.maara--;
                
                BackStackClass.WindowNotify(null, null);
                UpdatePage();
            }
        }

        private void calculateSum()
        {
            float sum = 0;
            foreach(MyyntiArtikkeli ma in tapahtuma.myyntitapahtuma.ostoslista.myyntiArtikkelit)
            {
                sum += (ma.hinta * ma.maara);
            }
            SumBlock.Text = sum.ToString();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            foreach (MyyntiArtikkeli ma in tapahtuma.myyntitapahtuma.ostoslista.myyntiArtikkelit)
            {
                ma.maara = 0;
            }
            BackStackClass.WindowNotify(null, null);
            UpdatePage();
        }

        private void NimiOnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            NimiEditorChanged = true;
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                EditBox.Visibility = Visibility.Collapsed;
            }
        }

        private void NimiFocusLost(object sender, RoutedEventArgs e)
        {
            if (NimiEditorChanged)
            {
                tapahtuma.nimi = EditBox.Text;
                PiilotaEditori();
                PageEnabled(true);
                NimiEditorChanged = false;
            }
        }

        private void PiilotaEditori()
        { 
            EditBox.Visibility = Visibility.Collapsed;
            EditBoxHeader.Visibility = Visibility.Collapsed;
            TapahtumaSelitys.Visibility = Visibility.Collapsed;
            TapahtumaQuery.Visibility = Visibility.Collapsed;
        }
    }
}