using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.Core;
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
    public sealed partial class Startup : Page
    {
        static int MAIN_TIMER = 20;   // 3 sec

        bool SettingsReceived= false;
        private DispatcherTimer mainTimer;
        int timesTicked = 1;
        int timesToTick = MAIN_TIMER;

        public Startup()
        {
            SettingsReceived = Settings.getSettings();
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            BackStackClass.Navigate(typeof(StartupProjected));
            mainTimer = new DispatcherTimer();
            mainTimer.Tick += mainTimer_Tick;
            mainTimer.Interval = TimeSpan.FromMilliseconds(50);
            mainTimer.Start();
        }

        void mainTimer_Tick(object sender, object e)
        {
            if (timesTicked >= timesToTick)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (SettingsReceived)
                {
                    mainTimer.Stop();
                    rootFrame.Navigate(typeof(MainPage));
                }
            }
            else
            {
                SettingsReceived = Settings.getSettings();
                timesTicked++;
                WaitBar.Value = timesTicked;
            }
        }
    }
}
