using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
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
    public class AppSettings
    {
        public int selectedLanguage { get; set; } = 0;
        public int SelectedCurrency { get; set; } = 0;

        public static implicit operator AppSettings(List<Hinnasto> v)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        enum SettingsLoadingStatus { NotStarted, OnGoing, Done};

        private static readonly string[] LanguageNames = new string[2] { "English", "Suomi" };
        private static readonly string[] Languages = new string[2] { "en-US", "fi-FI"};
        private static AppSettings appSettings = null;
        private static readonly string[] Currencies = new string[3] { "€", "$", "£" };
        private static SettingsLoadingStatus SettingsLoaded = SettingsLoadingStatus.NotStarted;

        private bool InitialKieliBoxDefinition = true;

        int MaxCounter = 0;
        private DispatcherTimer mainTimer;

        public Settings()
        {
            this.InitializeComponent();
            BackStackClass.DefineBack();
        }

        public static bool getSettings()
        {
            if (appSettings == null)
            {
                getSetting();
            }
            if (appSettings != null)
            {
                return true;
            }
            return false;
        }

        public static string getlanguage()
        {
            if (appSettings != null)
            {
                return Languages[appSettings.selectedLanguage];
            }
            return "en-US";
        }
        public static string getCurrency()
        {
            if (appSettings == null)
            {
                return Currencies[appSettings.SelectedCurrency];
            }
            return "€";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InitialKieliBoxDefinition = true;
            KieliBox.ItemsSource = LanguageNames;
            KieliBox.SelectedIndex = appSettings.selectedLanguage;
            InitialKieliBoxDefinition = false;
            ValuuttaBox.ItemsSource = Currencies;
            ValuuttaBox.SelectedIndex = appSettings.SelectedCurrency;
            BackStackClass.SetBackButtonVisibility();
        }

        private async void PoistaKaikkiClick(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog(Localization.GetLocalizedText("RemoveAllMsg"));
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
                InputOutput.DeleteAll(null);
                var dialog2 = new MessageDialog(Localization.GetLocalizedText("AllRemovedMsg"));
                await dialog2.ShowAsync();
            }
        }

        private async void PoistaTapahtumatClick(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog(Localization.GetLocalizedText("RemoveAllEventsMsg"));
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
                InputOutput.DeleteAll("Tapahtuma");
                var dialog2 = new MessageDialog(Localization.GetLocalizedText("AllEventsRemovedMsg"));
                await dialog2.ShowAsync();
            }
        }

        private void PoistaTapahtumiaClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TapahtumaLista), TapahtumaLista.UseCaseType.poista);
        }

        private void LanguageChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!InitialKieliBoxDefinition)
            {
                appSettings.selectedLanguage = KieliBox.SelectedIndex;
                ApplicationLanguages.PrimaryLanguageOverride = getlanguage();
                Frame.Navigate(this.GetType());
                this.Frame.BackStack.Remove(this.Frame.BackStack.Last());
                saveSettings();
                mainTimer = new DispatcherTimer();
                mainTimer.Tick += mainTimer_Tick;
                mainTimer.Interval = TimeSpan.FromMilliseconds(50);
                mainTimer.Start();
                MaxCounter = 0;
            }
        }
        void mainTimer_Tick(object sender, object e)
        {
            if (MaxCounter >= 2)
            {
                mainTimer.Stop();
            }
            else
            {
                Frame.Navigate(this.GetType());
                this.Frame.BackStack.Remove(this.Frame.BackStack.Last());
                MaxCounter++;
            }
        }

        private void CurrencyChanged(object sender, SelectionChangedEventArgs e)
        {
            appSettings.SelectedCurrency = ValuuttaBox.SelectedIndex;
            saveSettings();
        }

        private async void saveSettings()
        {
            uint i = 0;
            do
            {
                try
                {
                    await InputOutput.SaveObjectToXml(appSettings, "Settings.xml");
                }
                catch (Exception)
                {
                    i++;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            } while (i > 10);
        }

        public static async void loadSetting()
        {
            SettingsLoaded = SettingsLoadingStatus.OnGoing;
            try
            {
                appSettings = await InputOutput.ReadObjectFromXmlFileAsync<AppSettings>("Settings.xml");
            }
            catch (Exception ex) when (ex is System.IO.FileNotFoundException || ex is System.InvalidOperationException)
            {
                appSettings = new AppSettings();
            }
            if (appSettings == null)
            {
                appSettings = new AppSettings();
            }
            SettingsLoaded = SettingsLoadingStatus.Done;
        }

        private static async void getSetting()
        {
            uint i = 0;
            do
            {
                if (appSettings == null && SettingsLoaded == SettingsLoadingStatus.NotStarted)
                {
                    loadSetting();
                }
                if (appSettings == null && SettingsLoaded == SettingsLoadingStatus.OnGoing)
                {
                    i++;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            } while (i > 10);
        }
    }
}
