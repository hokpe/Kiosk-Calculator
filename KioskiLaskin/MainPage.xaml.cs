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
            //UIViewSettings.GetForCurrentView().UserInteractionMode;
        }

        private void UusiPaivaPress(object sender, RoutedEventArgs e)
        {
            /*string s = "uusi";
            int? secondViewId = null;

            if (ProjectionManager.ProjectionDisplayAvailable)
            {
                int thisViewId;
                thisViewId = ApplicationView.GetForCurrentView().Id;

                var thisDispatcher = Window.Current.Dispatcher;
                await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    secondViewId = ApplicationView.GetForCurrentView().Id;
                    // Display the page in the view. Not visible until “StartProjectionAsync” called
                    var rootFrame = new Frame();
                    rootFrame.Navigate(typeof(HinnastoJaArtikkelit), s);
                    Window.Current.Content = rootFrame;
                    Window.Current.Activate();
                });
                // Show the view on a second display
                if (secondViewId.HasValue)
                {
                    await ProjectionManager.StartProjectingAsync(secondViewId.Value, thisViewId);
                }
            }

            // Read more at https://blogs.windows.com/buildingapps/2015/12/07/optimizing-apps-for-continuum-for-phone/#SZEzLFPFjA9SGHkk.99 */
            this.Frame.Navigate(typeof(HinnastoJaArtikkelit));
            //BackStackClass.Navigate(this.Frame, typeof(HinnastoJaArtikkelit), typeof(HinnastoJaArtikkelit));
        }

        private void JatkaVanhaaPaivaa(object sender, RoutedEventArgs e)
        {
            //BackStackClass.Navigate(this.Frame, typeof(TapahtumaLista), typeof(TapahtumaLista), TapahtumaLista.UseCaseType.jatkaPaivaa);
            this.Frame.Navigate(typeof(TapahtumaLista), TapahtumaLista.UseCaseType.jatkaPaivaa);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            BackStackClass.Navigate(typeof(StartupProjected));
        }
        void App_Exit(object sender, ExitEventArgs e)
        {
            InitializeComponent();
        }

        private void TilastoClick(object sender, RoutedEventArgs e)
        {
            //BackStackClass.Navigate(this.Frame, typeof(TapahtumaLista), typeof(TapahtumaLista), TapahtumaLista.UseCaseType.Tilasto);
            this.Frame.Navigate(typeof(TapahtumaLista), TapahtumaLista.UseCaseType.tilasto);
        }

        private void AsetuksetClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }
    }
}
