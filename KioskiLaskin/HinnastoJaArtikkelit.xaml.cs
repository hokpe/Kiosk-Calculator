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
    public sealed partial class HinnastoJaArtikkelit : Page
    {
        //HinnastoJaArtikkelitConfiguration _pageParameters;
        [XmlArrayItem("Hinnastot")]
        private List<Hinnasto> Hinnastot = null;
        private bool HinnastoListSelectionChanged = true;
        private object SelectedHinnasto;
        private UInt64 ArtikkeliId = 0;
        private object SelectedArtikkeli;
        private bool ArtikkeliListSelectionChanged;
        private string ArtikkeliTextBoxOkString = null;
        private bool EditorChanged = false;

        public HinnastoJaArtikkelit()
        {
            this.InitializeComponent();

            BackStackClass.DefineBack();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            BackStackClass.SetBackButtonVisibility();
            if (e.NavigationMode == NavigationMode.Back)
            {
                /* Asynkroninen operaatio ilman varoituksia. */
                string s = Localization.GetLocalizedTextWithVariables("EventXStoppedMsg", Tapahtuma.getTapahtuma().nimi);
                Task task = new MessageDialog(s).ShowAsync().AsTask();
                /* Sulje tapahtuma*/
                Tapahtuma.hylkääTapahtuma();
                BackStackClass.Navigate(typeof(StartupProjected));
            }

            try
            {
                Hinnastot = await InputOutput.ReadObjectFromXmlFileAsync<List<Hinnasto>>("KioskiLaskinHinnastot.xml");
                ChangeCurrencies(Hinnastot);
                defineLastUid();
            }
            catch (Exception ex) when (ex is System.IO.FileNotFoundException || ex is System.InvalidOperationException)
            {
                Hinnastot = new List<Hinnasto>();
            }
            if(Hinnastot == null)
            {
                Hinnastot = new List<Hinnasto>();
            }

            EditBox2.Visibility = Visibility.Collapsed;
            EditBox2Header.Visibility = Visibility.Collapsed;
            ArtikkeliQuery.Visibility = Visibility.Collapsed;
            EditBox.Visibility = Visibility.Collapsed;
            EditBoxHeader.Visibility = Visibility.Collapsed;
            HinnastoQuery.Visibility = Visibility.Collapsed;

            if (Hinnastot.Count > 0)
            {
                listHinnasto.ItemsSource = Hinnastot;
                listHinnasto.SelectedIndex = 0;
                EditBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                listHinnasto.ItemsSource = null;
                LuoEnsimmainenHinnasto();
            }
        }

        private void ChangeCurrencies(List<Hinnasto> hinnastot)
        {
            string c = Settings.getCurrency();
            foreach(Hinnasto h in hinnastot)
            {
                foreach(Artikkeli a in h.Artikkelit)
                {
                    a.currency = c;
                }
            }
        }

        private async void LuoEnsimmainenHinnasto()
        {
            var dialog = new MessageDialog(Localization.GetLocalizedText("NoCataloguesMsg"));
            // "Ei vanhoja hinnastoja, luo ensin ensimmäinen.
            var okCommand = new UICommand("Ok", null, 0);

            dialog.Commands.Add(okCommand);
            dialog.Options = MessageDialogOptions.None;
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            var command = await dialog.ShowAsync();

            if (command != null && (int)command.Id == 0)
            {
                UusiHinnasto();
            }
        }

        private void defineLastUid()
        {
            UInt64 h_uid = 0, a_uid = 0;

            foreach (var h in Hinnastot)
            {
                if (h.uid > h_uid) h_uid = h.uid;
                foreach(var a in h.Artikkelit)
                {
                    if (a.uid > a_uid) a_uid = a.uid;
                }
            }
            if(Artikkeli.LastUid != a_uid + 1)
                Artikkeli.LastUid = a_uid+1;
            if (Hinnasto.LastUid != h_uid + 1)
                Hinnasto.LastUid = h_uid+1;
        }

        private async void PoistaHinnasto()
        {
            Hinnasto h;
            h = (Hinnasto)listHinnasto.SelectedItem;
            string s = listHinnasto.SelectedItem.ToString();

            var dialog = new MessageDialog(Localization.GetLocalizedTextWithVariables("DeleteXQuestionMsg", s), Localization.GetLocalizedText("ConfirmationMsgHeader"));

            //var dialog = new MessageDialog("Haluatko poistaa " + s + "?");
            var yesCommand = new UICommand(Localization.GetLocalizedText("Yes"), null, 0);
            var noCommand = new UICommand(Localization.GetLocalizedText("No"), null, 1);

            dialog.Commands.Add(yesCommand);
            dialog.Commands.Add(noCommand);
            dialog.Options = MessageDialogOptions.None;
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            var command = await dialog.ShowAsync();

            if (command != null && (int)command.Id == 0)
            {
                Hinnastot.Remove(h);
                listHinnasto.ItemsSource = null;
                listHinnasto.ItemsSource = Hinnastot;
                listHinnasto.SelectedItem = Hinnastot.First();
                h = (Hinnasto)listHinnasto.SelectedItem;
                listArtikkelit.ItemsSource = null;
                if(h == null)
                    LuoEnsimmainenHinnasto();
                else
                    listArtikkelit.ItemsSource = h.Artikkelit;
                string ss = Localization.GetLocalizedTextWithVariables("XHasDeletedMsg", s);
                var dialog2 = new MessageDialog(ss);
                await dialog2.ShowAsync();
            }
        }

        private void UusiHinnasto()
        {
            DateTime localDate = DateTime.Now;
            Hinnasto h;
            Artikkeli a;

            Hinnastot.Add(new Hinnasto(localDate.ToString()));
            h = Hinnastot.Find(x => x.nimi.Equals(localDate.ToString()));
            a = h.addArtikkeli(Localization.GetLocalizedText("DefaultArticleName"));
            a.hinta = 1;
            a.onKaytossa = true;
            listHinnasto.ItemsSource = Hinnastot;
            listHinnasto.SelectedIndex = Hinnastot.IndexOf(h);
            EditHinnasto(listHinnasto);
            HinnastoListSelectionChanged = false;
        }

        private void UusiArtikkeli()
        {
            Artikkeli a;
            Hinnasto h;
            h = (Hinnasto)listHinnasto.SelectedItem;

            a = h.addArtikkeli(Localization.GetLocalizedText("DefaultArticleName"));
            ArtikkeliId = a.uid;
            listArtikkelit.ItemsSource = null;
            listArtikkelit.ItemsSource = h.Artikkelit;
            EditArtikkeliNimi(a);
        }

        private async void PoistaArtikkeli()
        {
            Hinnasto h;
            Artikkeli a;
            h = (Hinnasto)listHinnasto.SelectedItem;
            a = (Artikkeli)h.getArtikkeli(listArtikkelit.SelectedItem.ToString());
            string s = a.ToString();

            var dialog = new MessageDialog(Localization.GetLocalizedTextWithVariables("DeleteXQuestionMsg",s), Localization.GetLocalizedText("ConfirmationMsgHeader"));
            //var dialog = new MessageDialog("Haluatko poistaa " + s + "?", "Kysymys");
            var yesCommand = new UICommand("Yes", null, 0);
            var noCommand = new UICommand("No", null, 1);

            dialog.Commands.Add(yesCommand);
            dialog.Commands.Add(noCommand);
            dialog.Options = MessageDialogOptions.None;
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            var command = await dialog.ShowAsync();

            if (command != null && (int)command.Id == 0)
            {
                h.delArtikkeli(a);
                string ss = Localization.GetLocalizedTextWithVariables("XHasDeletedMsg", s);
                var dialog2 = new MessageDialog(ss);
                await dialog2.ShowAsync();
                listArtikkelit.ItemsSource = null;
                listArtikkelit.ItemsSource = h.Artikkelit;
            }
        }

        private void EditHinnasto(ListBox lb)
        {
            EditBox.Text = lb.SelectedItem.ToString();
            lb.ScrollIntoView(lb.SelectedItem);
            EditBox.VerticalAlignment = lb.VerticalAlignment;
            EditBox.HorizontalAlignment = lb.HorizontalAlignment;
            EditBox.Visibility = Visibility.Visible;
            EditBoxHeader.Visibility = Visibility.Visible;
            HinnastoQuery.Visibility = Visibility.Visible;
            PageEnabled(false);
        }

        private void PageEnabled(bool enable)
        {
            listArtikkelit.IsEnabled = enable;
            listHinnasto.IsEnabled = enable;
            LisaaArtikkeliButton.IsEnabled = enable;
            PoistaArtikkeliButton.IsEnabled = enable;
            LisaaHinnastoButton.IsEnabled = enable;
            PoistaHinnastoButton.IsEnabled = enable;
            OkButton.IsEnabled = enable;
        }

        private void HinnastoValittu(object sender, SelectionChangedEventArgs e)
        {
            Hinnasto h;
            h = (Hinnasto)listHinnasto.SelectedItem;
            if (h == null)
            {
                listHinnasto.SelectedItem = Hinnastot.First();
                h = (Hinnasto)listHinnasto.SelectedItem;
            }
            if (h != null)
            {
                listArtikkelit.ItemsSource = h.Artikkelit;
            }
            if (SelectedHinnasto != listHinnasto.SelectedItem && listHinnasto.SelectedItem != null)
            {
                SelectedHinnasto = listHinnasto.SelectedItem;
                HinnastoListSelectionChanged = true;
            }
        }
        private void ArtikkeliValittu(object sender, SelectionChangedEventArgs e)
        {
            Artikkeli a;
            a = (Artikkeli)listArtikkelit.SelectedItem;

            if(a == null)
            {
                if (((Hinnasto)(listHinnasto.SelectedItem)).Artikkelit.Count != 0)
                {
                    listArtikkelit.SelectedItem = ((Hinnasto)(listHinnasto.SelectedItem)).Artikkelit.First();
                    a = (Artikkeli)listArtikkelit.SelectedItem;
                }
                else
                {
                    listArtikkelit.SelectedItem = null;
                }
            }
            if (SelectedArtikkeli != listArtikkelit.SelectedItem && listArtikkelit.SelectedItem != null)
            {
                SelectedArtikkeli = listArtikkelit.SelectedItem;
                ArtikkeliListSelectionChanged = true;
            }
        }

        private void HinnastoFocusLost(object sender, RoutedEventArgs e)
        {
            Hinnasto h;
            if (EditorChanged)
            {
                EditorChanged = false;
                EditBox.Visibility = Visibility.Collapsed;
                EditBoxHeader.Visibility = Visibility.Collapsed;
                HinnastoQuery.Visibility = Visibility.Collapsed;
                PageEnabled(true);
                h = (Hinnasto)listHinnasto.SelectedItem;
                if (h == null)
                {
                    listHinnasto.SelectedItem = Hinnastot.First();
                    h = (Hinnasto)listHinnasto.SelectedItem;
                }
                if (h != null)
                {
                    h.nimi = EditBox.Text;
                    TallennaHinnastot();
                    /* Reffress list */
                    listHinnasto.ItemsSource = null;
                    listHinnasto.ItemsSource = Hinnastot;
                    listHinnasto.ScrollIntoView(h);
                    listHinnasto.SelectedItem = h;
                }
            }
        }

        private async void ArtikkeliFocusLost(object sender, RoutedEventArgs e)
        {
            Artikkeli a = null;
            Hinnasto h;
            bool changed = true;

            if (EditorChanged)
            {
                EditorChanged = false;
                h = (Hinnasto)listHinnasto.SelectedItem;
                if (h == null)
                {
                    listHinnasto.SelectedItem = Hinnastot.First();
                    h = (Hinnasto)listHinnasto.SelectedItem;
                }
                if (h != null)
                {
                    a = h.getArtikkeli(ArtikkeliId);
                    if (a == null)
                    {
                        /* Problem */
                        throw new NotImplementedException();
                    }
                }
                if (a != null)
                {
                    if (EditBox2Header.Text == Localization.GetLocalizedText("articleNameEditHeader"))
                    {
                        foreach (Artikkeli ar in h.Artikkelit)
                        {
                            if (ar.nimi == EditBox2.Text && a != ar)
                            {
                                EditBox2.Visibility = Visibility.Visible;
                                string s = Localization.GetLocalizedTextWithVariables("CatalogueHasArticleXMsg", ar.nimi);
                                var dialog2 = new MessageDialog(s);
                                await dialog2.ShowAsync();
                                changed = false;
                            }
                        }
                        if (changed)
                        {
                            a.nimi = EditBox2.Text;
                            TallennaHinnastot();
                        }
                    }
                    else
                    {
                        float numValue;
                        bool parsed = float.TryParse(EditBox2.Text, out numValue);
                        if (numValue > 0 && parsed)
                        {
                            a.hinta = numValue;
                            TallennaHinnastot();
                        }
                        else
                        {
                            var dialog2 = new MessageDialog(Localization.GetLocalizedText("PriceCanBeOnlyPositiveNumberMsg"));
                            await dialog2.ShowAsync();
                        }
                    }
                    if (changed)
                    {
                        EditBox2.Visibility = Visibility.Collapsed;
                        EditBox2Header.Visibility = Visibility.Collapsed;
                        ArtikkeliQuery.Visibility = Visibility.Collapsed;
                        PageEnabled(true);

                        /* Reffress list */
                        listArtikkelit.ItemsSource = null;
                        listArtikkelit.ItemsSource = h.Artikkelit;
                        listArtikkelit.ScrollIntoView(a);
                        listArtikkelit.SelectedItem = a;
                    }
                }
            }
        }
        private void HinnastoOnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            EditorChanged = true;
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                EditBox.Visibility = Visibility.Collapsed;
            }
        }
        private void ArtikkeliOnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            EditorChanged = true;
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                EditBox2.Visibility = Visibility.Collapsed;
            }
        }

        private void ArtikkeliTextChanged(object sender, TextChangedEventArgs e)
        {
            EditorChanged = true;
            if (EditBox2Header.Text == Localization.GetLocalizedText("articlePriceEditHeader"))
            {
                string s = EditBox2.Text;
                float n;
                bool isNumeric = float.TryParse(s, out n);

                if (isNumeric)
                {
                    ArtikkeliTextBoxOkString = s;
                }
                else
                {
                    EditBox2.Text = ArtikkeliTextBoxOkString;
                    EditBox2.SelectionStart = EditBox2.Text.Length; // Sets the cursor to the end of the text
                    EditBox2.SelectionLength = 0;                   // Sets the cursor to the end of the text
                }
            }
        }

        private void UusiHinnastoClic(object sender, RoutedEventArgs e)
        {
            UusiHinnasto();
        }

        private void PoistaHinnastoClic(object sender, RoutedEventArgs e)
        {
            PoistaHinnasto();
        }

        private void UusiArtikkeliClic(object sender, RoutedEventArgs e)
        {
            UusiArtikkeli();
        }

        private void PoistaArtikkeliClic(object sender, RoutedEventArgs e)
        {
            PoistaArtikkeli();
        }

        private void HinnastoTapped(object sender, TappedRoutedEventArgs e)
        {
            Hinnasto h;
            h = (Hinnasto)listHinnasto.SelectedItem;

            if(!HinnastoListSelectionChanged)
            {
                if (h == null)
                {
                    listHinnasto.SelectedItem = Hinnastot.First();
                    h = (Hinnasto)listHinnasto.SelectedItem;
                }
                EditHinnasto(listHinnasto);
            }
            HinnastoListSelectionChanged = false;
        }

        private delegate void EditArtikkeli(Artikkeli a);

        private void ArtikkeliHintaTapped(object sender, TappedRoutedEventArgs e)
        {
            EditArtikkeli handler = EditArtikkeliHinta;
            ArtikkeliTapped(handler);
        }

        private void ArtikkeliTapped(EditArtikkeli edit)
        {
            Artikkeli a;
            a = (Artikkeli)listArtikkelit.SelectedItem;
            if (!ArtikkeliListSelectionChanged)
            {
                if (a == null)
                {
                    listArtikkelit.SelectedItem = ((Hinnasto)(listHinnasto.SelectedItem)).Artikkelit.First();
                    a = (Artikkeli)listArtikkelit.SelectedItem;
                }
                edit(a);
            }
            ArtikkeliListSelectionChanged = false;
        }

        private void EditArtikkeliNimi(Artikkeli a)
        {
            EditBox2.Text = a.nimi;
            ArtikkeliId = a.uid;
            EditBox2Header.Text = Localization.GetLocalizedText("articleNameEditHeader");
            //EditBox2Header.Text = "Artikkelin nimi";
            EditBox2.Visibility = Visibility.Visible;
            EditBox2Header.Visibility = Visibility.Visible;
            ArtikkeliQuery.Visibility = Visibility.Visible;
            PageEnabled(false);
        }

        private void EditArtikkeliHinta(Artikkeli a)
        {
            EditBox2.Text = a.hinta.ToString();
            ArtikkeliTextBoxOkString = EditBox2.Text;
            ArtikkeliId = a.uid;
            EditBox2Header.Text = Localization.GetLocalizedText("articlePriceEditHeader");
            EditBox2.Visibility = Visibility.Visible;
            EditBox2Header.Visibility = Visibility.Visible;
            ArtikkeliQuery.Visibility = Visibility.Visible;
            PageEnabled(false);
        }

        private void ArtikkeliNimiTapped(object sender, TappedRoutedEventArgs e)
        {
            EditArtikkeli handler = EditArtikkeliNimi;
            ArtikkeliTapped(handler);
        }

        private void HinnastoValittuClic(object sender, RoutedEventArgs e)
        {
            Hinnasto h;
            h = (Hinnasto)listHinnasto.SelectedItem;

            //BackStackClass.Navigate(this.Frame, typeof(Laskin), typeof(Laskin_Projected), h);
            this.Frame.Navigate(typeof(Laskin), h);
        }

        private async void TallennaHinnastot()
        {
            uint i = 0;
            do
            {
                try
                {
                    await InputOutput.SaveObjectToXml(Hinnastot, "KioskiLaskinHinnastot.xml");
                }
                catch (Exception)
                {
                    i++;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            } while (i > 10);
        }

        /*
   Artikkeli a;
   a = (Artikkeli)listArtikkelit.SelectedItem;
   if(!ArtikkeliListSelectionChanged)
   {
       if(a == null)
       {
           listArtikkelit.SelectedItem = ((Hinnasto)(listHinnasto.SelectedItem)).Artikkelit.First();
           a = (Artikkeli)listArtikkelit.SelectedItem;
       }
       EditArtikkeliNimi(a);
   }
   ArtikkeliListSelectionChanged = false;
}*/
    }
}
