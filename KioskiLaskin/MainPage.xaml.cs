using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace KioskiLaskin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            ApplicationLanguages.PrimaryLanguageOverride = Settings.getlanguage();
            this.InitializeComponent();
        }

        private void UusiPaivaPress(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HinnastoJaArtikkelit));
        }

        private void JatkaVanhaaPaivaa(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TapahtumaLista), TapahtumaLista.UseCaseType.jatkaPaivaa);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            Frame rootFrame = Window.Current.Content as Frame;
            while(rootFrame.BackStackDepth != 0)
            {
                /* MainPage is the initial page, if BackStack contains some pages, those needs to be removed. Like Startup Page. */
                this.Frame.BackStack.Remove(this.Frame.BackStack.Last());
            }
            BackStackClass.Navigate(typeof(StartupProjected));
        }
        void App_Exit(object sender, ExitEventArgs e)
        {
            InitializeComponent();
        }

        private void TilastoClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TapahtumaLista), TapahtumaLista.UseCaseType.tilasto);
        }

        private void AsetuksetClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }
    }
}
