using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default
{
    public class Capa
    {

        //Approach
        public string AP { get; set; }

        //Traffic flow direction form Input sheet
        public string TFD { get; set; }

        //Adjusted flow rate, v (veh/h)
        public decimal v { get; set; }

        //Saturation flow rate, s (veh/h)
        public decimal s { get; set; }

        //Lost time, tL (s), tL = l1 + Y – e
        public decimal tL { get; set; }

        //Effective green time, g (s), g = G + Y – tL
        public decimal gs { get; set; }

        //Green ratio, g/C
        public decimal gC { get; set; }

        //Lane group capacity,1 c = s(g/C), (veh/h)
        public decimal c { get; set; }

        //v/c ratio, X
        public decimal X { get; set; }

        //Flow ratio, v/s
        public decimal? vs { get; set; }

        //Critical lane group/phase (√)
        public decimal Vbeta { get; set; }

    }


    public class Phsr
    {
         //Phase number
        public string PH { get; set; }

        public List<Capa> capalist;

        public decimal Cs { get; set; }

        //Critical Flow ratio, v/s
        public decimal Cvs { get; set; }


        //common value 
        //Sum of flow ratios for critical lane groups, Yc = Σ (critical lane groups, v/s)
        public decimal sumYc { get; set; }

        //Total lost time per cycle, L (s)
        public decimal Ls { get; set; }

        //Critical flow rate to capacity ratio, Xc = (Yc)(C)/(C – L)"
        public decimal Xc { get; set; }
    }
}