using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KioskiLaskin
{
    public class Myyntitapahtuma
    {
        public Ostostoslista ostoslista = null;

        public UInt64 uid = 1;
        public static UInt64 LastUid { get; set; } = 1;

        public Myyntitapahtuma()
        {
            uid = LastUid++;
        }

        public void uusiOstoslista(Hinnasto h)
        {
            ostoslista = new Ostostoslista();
            foreach (Artikkeli a in h.Artikkelit)
            {
                if (a.onKaytossa)
                {
                    MyyntiArtikkeli ma = new MyyntiArtikkeli();
                    ma.nimi = a.nimi;
                    ma.maara = 0;
                    ma.hinta = a.hinta;
                    ma.onKaytossa = a.onKaytossa;
                    ma.currency = a.currency;
                    ostoslista.myyntiArtikkelit.Add(ma);
                }
            }
        }

        // Lisää myynti on keskeneräinen, siinä ei enää luoda MyyntiArtikkelia.
        public void LisaaMyynti(Artikkeli artikkeli, UInt16 maara)
        {
            MyyntiArtikkeli ma = new MyyntiArtikkeli();
            ma.hinta = artikkeli.hinta;
            ma.nimi = artikkeli.nimi;
            ma.onKaytossa = artikkeli.onKaytossa;
            ma.maara = maara;
            ostoslista.myyntiArtikkelit.Add(ma);
        }
    }
}
