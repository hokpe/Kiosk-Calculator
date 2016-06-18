using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KioskiLaskin
{
    [XmlRoot(ElementName = "Hinnasto")]
    public class Hinnasto
    {
        [XmlArrayItem("Artikkelit")]
        public List<Artikkeli> Artikkelit = new List<Artikkeli>();

        public string nimi { get; set; }

        public UInt64 uid = 1;
        public static UInt64 LastUid { get; set; } = 1;
        public Hinnasto()
        {
            this.nimi = "Hinnasto";
            uid = LastUid++;
        }

        public Hinnasto(string name)
        {
            this.nimi = name;
            uid = LastUid++;
        }

        public override string ToString()
        {
            return nimi;
        }

        public Artikkeli getArtikkeli(string name)
        {
            Artikkeli a = null;
            if (name != "")
            {
                a = Artikkelit.Find(x => x.nimi.Equals(name));
            }
            return a;
        }

        public Artikkeli getArtikkeli(UInt64 id)
        {
            Artikkeli a = null;
            if (id != 0)
            {
                a = Artikkelit.Find(x => x.uid == id);
            }
            
            return a;
        }

        public Artikkeli addArtikkeli(string name)
        {
            Artikkeli uusiA = new Artikkeli(name);
            Artikkelit.Add(uusiA);
            return uusiA;
        }

        public bool delArtikkeli(Artikkeli a)
        {
            return Artikkelit.Remove(a);
        }

        public bool delArtikkeli(string name)
        {
            Artikkeli a = getArtikkeli(name);
            return delArtikkeli(a);
        }
    }

    public class Ostostoslista : Hinnasto
    {
        [XmlArrayItem("MyyntiArtikkelit")]
        public List<MyyntiArtikkeli> myyntiArtikkelit = new List<MyyntiArtikkeli>();
    }
}
