using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default
{
    //sh 6- 7 class
    public class Oppsr
    {
        //Cycle length, C (s)
        public decimal Cs { get; set; }

        //Total actual green time for LT lane group,1 G (s)
        public decimal Gs { get; set; }

        //Effective permitted green time for LT lane group,1 g (s)
        public decimal gs { get; set; }

        //Opposing effective green time, go (s)
        public decimal g0 { get; set; }

        //Number of lanes in LT lane group,2 N
        public decimal NnLT { get; set; }
        public decimal NnTH { get; set; }
        public decimal NnRT { get; set; }

        //Number of lanes in opposing approach, No
        public decimal N0LT { get; set; }
        public decimal N0TH { get; set; }
        public decimal N0RT { get; set; }

        //Adjusted LT flow rate, vLT (veh/h)
        public decimal vltLT { get; set; }
        public decimal vltTH { get; set; }
        public decimal vltRT { get; set; }

        //Proportion of LT volume in LT lane group,3 PLT
        public decimal PltLT { get; set; }
        public decimal PltTH { get; set; }
        public decimal PltRT { get; set; }

        //Proportion of LT volume in LT lane group, PLTo sheet 7
        public decimal Plt0LT { get; set; }
        public decimal Plt0TH { get; set; }
        public decimal Plt0RT { get; set; }


        //Adjusted flow rate for opposing approach, vo (veh/h)
        public decimal v0LT { get; set; }
        public decimal v0TH { get; set; }
        public decimal v0RT { get; set; }

        //Lost time for LT lane group, tL
        public decimal tL { get; set; }


        /*****************************************************/
        //computation

        //LT volume per cycle, LTC = vLTC/3600
        public decimal LTCLT { get; set; }
        public decimal LTCTH { get; set; }
        public decimal LTCRT { get; set; }

        //"Opposing lane utilization factor, fLUo 

        public decimal fLUoLT { get; set; }
        public decimal fLUoTH { get; set; }
        public decimal fLUoRT { get; set; }

        //Opposing flow per lane, per cycle
        public decimal VolcLT { get; set; }
        public decimal VolcTH { get; set; }
        public decimal VolcRT { get; set; }

        public decimal GfLT { get; set; }
        public decimal GfTH { get; set; }
        public decimal GfRT { get; set; }

        //Opposing platoon ratio, Rpo (refer to Exhibit 16-11)
        public decimal Rp0LT { get; set; }
        public decimal Rp0TH { get; set; }
        public decimal Rp0RT { get; set; }

        //Opposing queue ratio, qro = max[1 – Rpo(go/C), 0]
        public decimal Qr0LT { get; set; }
        public decimal Qr0TH { get; set; }
        public decimal Qr0RT { get; set; }

        public decimal GqLT { get; set; }
        public decimal GqTH { get; set; }
        public decimal GqRT { get; set; }


        public decimal GuLT { get; set; }
        public decimal GuTH { get; set; }
        public decimal GuRT { get; set; }

        //sh7 only
        public decimal nnLT { get; set; }
        public decimal nnTH { get; set; }
        public decimal nnRT { get; set; }


        public decimal PTH0LT { get; set; }
        public decimal PTH0TH { get; set; }
        public decimal PTH0RT { get; set; }

        //Elt Throught -car Equivalent
        public decimal El1LT { get; set; }
        public decimal El1TH { get; set; }
        public decimal El1RT { get; set; }

        //sh7 only
        public decimal El2LT { get; set; }
        public decimal El2TH { get; set; }

        public decimal El2RT { get; set; }

        //sh6 only
        public decimal PlLT { get; set; }
        public decimal PlTH { get; set; }
        public decimal PlRT { get; set; }

        public decimal FminLT { get; set; }
        public decimal FminTH { get; set; }
        public decimal FminRT { get; set; }

        //sh07
        public decimal gdiffLT { get; set; }
        public decimal gdiffTH { get; set; }
        public decimal gdiffRT { get; set; }

        public decimal FmLT { get; set; }
        public decimal FmTH { get; set; }
        public decimal FmRT { get; set; }

        //last result
        //Left turn phase adj
        public decimal fLTLT { get; set; }
        public decimal fLTTH { get; set; }
        public decimal fLTRT { get; set; }


    }
}