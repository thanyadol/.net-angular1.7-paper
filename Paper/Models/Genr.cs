using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default
{
    //sh1 - input class
    public class Genr
    {
        public string Analyst { get; set; }
        public string Company { get; set; }
        public string Intersect { get; set; }
        public string Area { get; set; }
        public string Date { get; set; }

        public string Period { get; set; }

        public decimal StationFlow { get; set; }
        public decimal Crosswalk { get; set; }

        //Heavy-vehicle (%) 
        public decimal HvL1L2 { get; set; }
        public decimal HvL3L4 { get; set; }


    }

    public class Cart
    {
        //last result, Intersection delay
        public decimal dI { get; set; }
        public decimal sumDA { get; set; }
        public decimal sumVA { get; set; }

        public string dISTR { get; set; }

        //component list
        public Genr genr { get; set; }
        public List<Geome> geolist { get; set; }
        public List<Volm> volmlist { get; set; }
        public List<Oppsr> opposelist { get; set; }
        public List<Bicycl> bicyclelist { get; set; }
        public List<Signr> signllist { get; set; }

        public List<Phsr> phaselist { get; set; }


        public List<Loss> losslist { get; set; }
    }
}