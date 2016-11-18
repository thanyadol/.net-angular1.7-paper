using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default
{
    public class Loss
    {

        //Adjusted flow rate, v (veh/h)
        public decimal vLT { get; set; }
        public decimal vTH { get; set; }
        public decimal vRT { get; set; }

        //Lane group capacity, c (veh/h)
        public decimal cLT { get; set; }
        public decimal cTH { get; set; }

        public decimal cRT { get; set; }

        //v/c ratio, X = v/c
        public decimal XLT { get; set; }
        public decimal XTH { get; set; }
        public decimal XRT { get; set; }

        //total green ratio, g/C
        public decimal gC { get; set; }

        //Uniform delay, (s/veh)
        public decimal d1LT { get; set; }
        public decimal d1TH { get; set; }
        public decimal d1RT { get; set; }

        //Incremental delay calibration , k
        public decimal k { get; set; }

        //Incremental delay , d2
        public decimal d2LT { get; set; }
        public decimal d2TH { get; set; }
        public decimal d2RT { get; set; }

        //Initial queue delay, d3 (s/veh)
        public decimal d3 { get; set; }

        //Delay, d = d1(PF) + d2 + d3 (s/veh)
        public decimal dLT { get; set; }
        public decimal dTH { get; set; }
        public decimal dRT { get; set; }

        //Progression adjustment factor, PF
        public decimal PF { get; set; }

        //LOS by lane group
        public string LOSLT { get; set; }
        public string LOSTH { get; set; }
        public string LOSRT { get; set; }

        //Delay by approach,
        public decimal dA { get; set; }

        //loss by approach,
        public string LOSSA { get; set; }

        //Approach flow rate, vA (veh/h)
        public decimal vA { get; set; }

        //Intersection delay
        public decimal dI { get; set; }
    }
}