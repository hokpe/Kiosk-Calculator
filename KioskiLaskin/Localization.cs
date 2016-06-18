using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace KioskiLaskin
{
    static class Localization
    {
        private static ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

        public static string GetLocalizedText(string logicalName)
        {
            string s;

            s = loader.GetString(logicalName);

            return s;
        }

        public static string GetLocalizedTextWithVariables(string logicalName, params object[] list)
        {
            string s;
           
            s = loader.GetString(logicalName);
            s = AddParameterToString(s, list);
               
            return s;
        }

        private static string AddParameterToString(string s, object[] list)
        {
            /* Keskeneräinen, osaa lisätä vasta yhden merkkijonon olemassa olevaan tekstiin. */
            int i;
            string ss = "";
            foreach(object o in list)
            {
                if(o is string)
                {
                    i = s.IndexOf("%s");
                    if(i != -1) // Found
                    {
                        ss = s.Substring(0, i); // Copy characters before the variable to the new string.
                        ss += (string)o;        // Copy the variable string
                        ss += s.Substring(i + 2); // Copy rest of the original string to the end of the new string
                    }
                }
            }

            return ss;
        }
    }
}
