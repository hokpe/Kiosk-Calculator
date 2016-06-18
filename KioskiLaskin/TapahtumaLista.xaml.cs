using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
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
    public sealed partial class TapahtumaLista : Page
    {
        public enum UseCaseType { tilasto, jatkaPaivaa, poista };
        private List<Tapahtuma> tapahtumat;
        private UseCaseType UseCase;
        
        public TapahtumaLista()
        {
            this.InitializeComponent();
            BackStackClass.DefineBack();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            BackStackClass.SetBackButtonVisibility();
            
            if (e.Parameter != null)
            {
                if (e.NavigationMode == NavigationMode.Back)
                {
                    if (e.Parameter is UseCaseType && (UseCaseType)e.Parameter == UseCaseType.jatkaPaivaa)
                    {
                        /* Asynkroninen operaatio ilman varoituksia. */
                        string s = Localization.GetLocalizedTextWithVariables("EventXStoppedInListMsg", Tapahtuma.getTapahtuma().nimi);
                        Task task = new MessageDialog(s).ShowAsync().AsTask();
                        /* Sulje tapahtuma*/
                        Tapahtuma.hylkääTapahtuma();
                    }
                    BackStackClass.Navigate(typeof(StartupProjected));
                }
                if (e.Parameter is UseCaseType)
                {
                    UseCase = (UseCaseType)e.Parameter;
                    tapahtumat = await InputOutput.ReadTapahtumatFromXmlFilesAsyncToList("Tapahtuma");
                    listTapahtumat.ItemsSource = tapahtumat;
                    /* List creates Tapahtuma objects and last read Tapahtuma is stored to static lastThis variable in Tapahtuma. It needs to be discarded. */
                    Tapahtuma.hylkääTapahtuma();
                }
                else
                {
                    /* Problem */
                    throw new NotImplementedException();
                }
                if(UseCase == UseCaseType.poista)
                {
                    ValmisButton.Content = Localization.GetLocalizedText("Delete");
                    ValmisButton.Background = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void ValmisClick(object sender, RoutedEventArgs e)
        {
            if(UseCase == UseCaseType.jatkaPaivaa)
            {
                //BackStackClass.Navigate(this.Frame, typeof(Laskin), typeof(Laskin_Projected), listTapahtumat.SelectedItem);
                this.Frame.Navigate(typeof(Laskin), listTapahtumat.SelectedItem);
            }
            else if(UseCase == UseCaseType.poista)
            {
                //BackStackClass.Navigate(this.Frame, typeof(Tilasto), typeof(Tilasto), listTapahtumat.SelectedItem);
                InputOutput.DeleteFile(((Tapahtuma)listTapahtumat.SelectedItem).filename);
                tapahtumat.Remove((Tapahtuma)listTapahtumat.SelectedItem);
                listTapahtumat.ItemsSource = null;
                listTapahtumat.ItemsSource = tapahtumat;
            }
            else
            {
                //BackStackClass.Navigate(this.Frame, typeof(Tilasto), typeof(Tilasto), listTapahtumat.SelectedItem);
                this.Frame.Navigate(typeof(Tilasto), listTapahtumat.SelectedItem);
            }
        }
    }
}
