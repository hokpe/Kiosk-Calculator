using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KioskiLaskin
{
    public class Tapahtuma
    {
        public string nimi { get; set; }
        public string Paivamaara { get; set; }
        public string filename { get; set; }
        public Hinnasto hinnasto = null;

        [XmlArrayItem("Myyntitapahtumat")]
        public List<Myyntitapahtuma> Myyntitapahtumat = new List<Myyntitapahtuma>();
        public Myyntitapahtuma myyntitapahtuma = null;

        public UInt64 uid = 1;
        public static UInt64 LastUid { get; set; } = 1;
        public static Tapahtuma lastThis = null;

        public static Tapahtuma getTapahtuma()
        {
            return lastThis;
        }

        public static void hylkääTapahtuma()
        {
            lastThis = null;  
        }

        internal static void UpdateTapahtuma(Tapahtuma tapahtuma)
        {
            lastThis = tapahtuma;
        }

        public Tapahtuma() : this("Default")
        {
        }

        public Tapahtuma(string name)
        {
            nimi = name;
            uid = LastUid++;
            lastThis = this;
        }

        public bool MaaritaHinnasto(Hinnasto h)
        {
            if (hinnasto != null) return false;

            // Luo kopio annetusta hinnastosta.
            hinnasto = new Hinnasto(h.nimi);
            foreach(Artikkeli a in h.Artikkelit)
            {
                Artikkeli uusi_a = hinnasto.addArtikkeli(a.nimi);
                uusi_a.hinta = a.hinta;
                uusi_a.onKaytossa = a.onKaytossa;
            }
            return true;
        }

        public void LisaaMyyntiTapahtuma()
        {
            myyntitapahtuma = new Myyntitapahtuma();
            myyntitapahtuma.uusiOstoslista(hinnasto);
        }

        public void LisaaMyynti()
        {
            Myyntitapahtumat.Add(myyntitapahtuma);
            myyntitapahtuma = null;
        }

        public override string ToString()
        {
            return nimi;
        }

        public async void TallennaTapahtuma()
        {
            uint i = 0;

            if (filename == null)
            {
                string s = await InputOutput.GetNextFileName("Tapahtuma");
                filename = s;
            }
            do
            {
                try
                {
                    await InputOutput.SaveObjectToXml(this, filename);
                }
                catch (Exception)
                {
                    i++;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            } while (i > 10);
        }
    }
}
