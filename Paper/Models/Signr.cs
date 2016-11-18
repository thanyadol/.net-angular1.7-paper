using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default
{
    //sh1 - input class
    public class Signr
    {
        public string Leg { get; set; }

        //Traffic flow direction
        public string Tfd { get; set; }

        public decimal TimeY { get; set; }
        public decimal TimeG { get; set; }
    }
}