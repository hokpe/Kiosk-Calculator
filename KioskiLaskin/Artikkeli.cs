using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KioskiLaskin
{
    [XmlRoot(ElementName = "Artikkeli")]
    public class Artikkeli
    {
        public string nimi {get;set;}
        public bool onKaytossa { get; set; }
        public float hinta { get; set; }
        public string currency { get; set; }
        public UInt64 uid = 1;
        public static UInt64 LastUid { get; set; } = 1;

        public Artikkeli() : this("Default")
        {
        }

        public Artikkeli(string name)
        {
            nimi = name;
            currency = Settings.getCurrency();
            onKaytossa = true;
            hinta = 1;
            uid = LastUid++;
        }

        public override string ToString()
        {
            return nimi;
        }
    }

    public class MyyntiArtikkeli : Artikkeli
    {
        public UInt16 maara { get; set; } = 0;
    }
}
