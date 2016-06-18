using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace KioskiLaskin
{
    public sealed partial class Laskin_Projected : Page, iPageUpdateListener
    {
        Tapahtuma tapahtuma = null;
        
        public Laskin_Projected()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                tapahtuma = Tapahtuma.getTapahtuma();
                if (tapahtuma == null)
                {
                    if (e.Parameter is Tapahtuma)
                    {
                        tapahtuma = (Tapahtuma)e.Parameter;
                    }
                    else if (e.Parameter is Hinnasto)
                    {
                        listArtikkelit.ItemsSource = null;
                    }
                    else
                    {
                        /* Problem */
                        throw new NotImplementedException();
                    }
                }
                if(tapahtuma != null)
                {
                    listArtikkelit.ItemsSource = tapahtuma.myyntitapahtuma.ostoslista.myyntiArtikkelit;
                    calculateSum();
                }
                EuroaBlock.Text = Settings.getCurrency();
            }
        }

        public void UpdatePage(string field, string text)
        {
            if (tapahtuma == null)
            {
                tapahtuma = Tapahtuma.getTapahtuma();
            }
            if (tapahtuma != null)
            {
                /* Reffress list */
                listArtikkelit.ItemsSource = null;
                listArtikkelit.ItemsSource = tapahtuma.myyntitapahtuma.ostoslista.myyntiArtikkelit;
                calculateSum();
            }
        }

        private void calculateSum()
        {
            float sum = 0;
            foreach (MyyntiArtikkeli ma in tapahtuma.myyntitapahtuma.ostoslista.myyntiArtikkelit)
            {
                sum += (ma.hinta * ma.maara);
            }
            SumBlock.Text = sum.ToString();
        }
    }
}
