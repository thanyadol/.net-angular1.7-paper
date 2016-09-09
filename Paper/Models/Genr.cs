using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default
{
    public class Genr
    {
        public string Analyst { get; set; }
        public string Company { get; set; }
        public string Intersect { get; set; }
        public string Area { get; set; }
        public string Date { get; set; }

        public string Period { get; set; }

        public decimal StationFlow { get; set; }
        public decimal Crosswork { get; set; }

        public decimal HeavyL1L2 { get; set; }
        public decimal HeavyL3L4 { get; set; }

    }

    public class Cart
    {
        public Genr genr { get; set; }
        public List<Geome> geolist { get; set; }
        public List<Volm> volmlist { get; set; }
        public List<Signr> signllist { get; set; }
    }
}