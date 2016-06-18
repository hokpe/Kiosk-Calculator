using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace KioskiLaskin
{
    class BackStackClass
    {
        //public static Frame ProjectionFrame = null;
        public static List<Window> windows = new List<Window>();
        private static bool BackDefined = false;
        internal static void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            /* Just to keep in mind how to investicate backstack problems. */
            IList<PageStackEntry> pse = rootFrame.BackStack;
            PageStackEntry[] psea = new PageStackEntry[pse.Count];
            pse.CopyTo(psea, 0);

            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        internal static void DefineBack()
        {
            if (!BackDefined)
            {
                SystemNavigationManager.GetForCurrentView().BackRequested += BackStackClass.OnBackRequested;
                BackDefined = true;
            }
        }

        internal static void SetBackButtonVisibility()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                (rootFrame != null && rootFrame.CanGoBack) ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        internal static async void WindowNotify(string field, string text)
        {
            foreach (Window w in windows)
            {
                await w.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (w.Visible == true)
                    {
                        Frame f = (Frame)w.Content;
                        ((iPageUpdateListener)f.Content).UpdatePage(field, text);
                    }
                });
            }
        }

        internal static void Navigate(Type ProjectedSourcePageType)
        {
            Navigate(ProjectedSourcePageType, null);
        }

        internal static async void Navigate(Type ProjectedSourcePageType, Object object1)
        {
            int? secondViewId = null;
            
            if (ProjectionManager.ProjectionDisplayAvailable)
            {
                if (windows.Count == 0)
                {
                    int thisViewId;
                    thisViewId = ApplicationView.GetForCurrentView().Id;

                    var thisDispatcher = Window.Current.Dispatcher;
                    await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        secondViewId = ApplicationView.GetForCurrentView().Id;
                        // Display the page in the view. Not visible until “StartProjectionAsync” called
                        Frame ProjectionFrame = new Frame();
                        ProjectionFrame.Navigate(ProjectedSourcePageType, object1);
                        Window.Current.Content = ProjectionFrame;
                        windows.Add(Window.Current);
                        Window.Current.Activate();

                    });
                    // Show the view on a second display
                    if (secondViewId.HasValue)
                    {
                        await ProjectionManager.StartProjectingAsync(secondViewId.Value, thisViewId);
                    }
                }
                else
                {
                    foreach (Window w in windows)
                    {
                        await w.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (w.Visible == true)
                            {
                                Frame f = (Frame)w.Content;
                                f.Navigate(ProjectedSourcePageType, object1);
                            }
                        });
                    }
                }
            }

            // Read more at https://blogs.windows.com/buildingapps/2015/12/07/optimizing-apps-for-continuum-for-phone/#SZEzLFPFjA9SGHkk.99
        }
    }
}
