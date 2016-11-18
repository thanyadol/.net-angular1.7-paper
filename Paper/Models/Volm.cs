using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default
{
    //sh2 - class
    public class Volm
    {
        //Volume, V (veh/h)				
        public decimal VLT { get; set; }
        public decimal VTH { get; set; }
        public decimal VRT { get; set; }

        //Peak-hour factor, PHF				
        public decimal PHF { get; set; }

        //Pretimed (P) or actuated (A)				
        public string PA { get; set; }

        //Start-up lost time, l1 (s)				
        public string l1 { get; set; }


        //Extension of effective green time, e (s)				
        public decimal e { get; set; }

        public decimal T { get; set; }

        //Arrival type, AT				
        public decimal AT { get; set; }

        //Approach pedestrian volume2, vped (p/h)				
        public decimal Vped { get; set; }

        //Approach bicycle volume2, vbic (bicycles/h)				
        public decimal vbic { get; set; }

        //Parking maneuvers, Nm (maneuvers/h)				
        public string Pyos { get; set; }	
        public decimal Nm { get; set; }

        //Bus stopping, NB (buses/h)				
        public decimal NB { get; set; }

        //Min. timing for pedestrians3, Gp (s)				
        public decimal Gps { get; set; }

        //No. of Lanes Groups				
        public string LGLT { get; set; }  
        public string LGTH { get; set; }
        public string LGRT { get; set; }

        //Nunber of Left Turning Lanes	/ Phase		 		
        public string Nlt { get; set; }
        public string LTp { get; set; }

        //Nunber of Right Turning Lanes	/ Phase			

        public string Nrt { get; set; }
        public string RTp { get; set; }


        //---------------------- for output --------------------------------//

        //Adjusted flow rate, vp = V/PHF (veh/h)
        public decimal vpLT { get; set; }
        public decimal vpTH { get; set; }
        public decimal vpRT { get; set; }

        //Adjusted flow rate in lane group, v (veh/h)
        public decimal VvLT { get; set; }
        public decimal VvTH { get; set; }
        public decimal VvRT { get; set; }

        //Proportion1 of  LT or RT (PLT or PRT)
        public decimal PLT { get; set; }
        //is zero
        public decimal PTH { get; set; }
        public decimal PRT { get; set; }

        //********* saturation flow rate
        //Base saturation flow, so (pc/h/ln)
        public decimal S0 { get; set; }

        //Number of lanes, N
        public decimal NnLT { get; set; }
        public decimal NnTH { get; set; }
        public decimal NnRT { get; set; }

        //adjustment factor

        //Lane width adjustment factor, fw
        public decimal fwLT { get; set; }
        public decimal fwTH { get; set; }
        public decimal fwRT { get; set; }

        //Heavy-vehicle adjustment factor, fHV
        public decimal fHVLT { get; set; }
        public decimal fHVTH { get; set; }
        public decimal fHVRT { get; set; }


        //Grade adjustment factor, fg
        public decimal fgLT { get; set; }
        public decimal fgTH { get; set; }
        public decimal fgRT { get; set; }


        //Parking adjustment factor, fp
        public decimal fpLT { get; set; }
        public decimal fpTH { get; set; }
        public decimal fpRT { get; set; }

        //Bus blockage adjustment factor, fbb
        public decimal fbbLT { get; set; }
        public decimal fbbTH { get; set; }
        public decimal fbbRT { get; set; }

        //Area type adjustment factor, fa
        public decimal faLT { get; set; }
        public decimal faTH { get; set; }
        public decimal faRT { get; set; }

        //Lane utilization adjustment factor, fLU
        public decimal fLULT { get; set; }
        public decimal fLUTH { get; set; }
        public decimal fLURT { get; set; }

        //Left-turn adjustment factor, fLT
        public decimal fLTLT { get; set; }
        public decimal fLTTH { get; set; }
        public decimal fLTRT { get; set; }


        //Right-turn adjustment factor, fRT
        public decimal fRTLT { get; set; }
        public decimal fRTTH { get; set; }
        public decimal fRTRT { get; set; }


        //Left-turn ped/bike adjustment factor, fLpb
        public decimal fLpbLT { get; set; }
        public decimal fLpbTH { get; set; }
        public decimal fLpbRT { get; set; }

        //Right-turn ped/bike adjustment factor, fRpb
        public decimal fRpbLT { get; set; }
        public decimal fRpbTH { get; set; }
        public decimal fRpbRT { get; set; }

        //"Adjusted saturation flow, s (veh/h) s = so N fw fHV fg fp fbb fa fLU fLT fRT fLpb fRpb"

        public decimal sLT { get; set; }
        public decimal sTH { get; set; }
        public decimal sRT { get; set; }


        //cross-street receiving 
        public decimal Nrec { get; set; }

        //Number of turning lanes,1 Nturn
        public decimal Nturn { get; set; }



    }


}