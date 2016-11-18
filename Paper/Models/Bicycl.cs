using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default
{
    //sh8 class
    public class Bicycl
    {
        //Effective pedestrian green time,1,2 gp (s)
        public decimal gp { get; set; }

        //Conflicting pedestrian volume,1 vped (p/h)
        public decimal vped { get; set; }

        //vpedg = vped (C/gp)

        public decimal vpedg { get; set; }

        //OCCpedg = vpedg/2000 if (vpedg ≤ 1000) or

        //OCCpedg = 0.4 + vpedg/10,000 if (1000 < vpedg ≤ 5000)
        public decimal OCCpedg { get; set; }


        //Opposing queue clearing green,3,4 gq (s)
        public decimal gqLT { get; set; }
        public decimal gqTH { get; set; }
        public decimal gqRT { get; set; }

        //Effective pedestrian green consumed by opposing vehicle queue, gq/gp; if gq ≥ gp then fLpb = 1.0
        public decimal fgqgpLT { get; set; }
        public decimal fgqgpTH { get; set; }
        public decimal fgqgpRT { get; set; }


        //OCCpedu = OCCpedg [1 – 0.5(gq/gp)]
        public decimal OCCpeduLT { get; set; }
        public decimal OCCpeduTH { get; set; }
        public decimal OCCpeduRT { get; set; }


        //Opposing flow rate,3 vo (veh/h)
        public decimal v0LT { get; set; }
        public decimal v0TH { get; set; }
        public decimal v0RT { get; set; }

        //OCCl = OCCpedu [e–(5/3600)vo]
        public decimal OCCrLLT { get; set; }
        public decimal OCCrLTH { get; set; }
        public decimal OCCrLRT { get; set; }

        //Number of cross-street receiving lanes,1 Nrec
        public decimal NrecL { get; set; }

        //Number of turning lanes,1 Nturn
        public decimal NturnL { get; set; }

        public decimal NrecR { get; set; }

        //Number of turning lanes,1 Nturn
        public decimal NturnR { get; set; }

        //"ApbT = 1 – OCCr if Nrec = Nturn, ApbT = 1 – 0.6(OCCr) if Nrec > Nturn"
        public decimal ApbTLLT { get; set; }
        public decimal ApbTLTH { get; set; }
        public decimal ApbTLRT { get; set; }

        //Proportion of left turns,5 PLT
        public decimal PLT { get; set; }

        //Proportion of left turns using protected phase,6 PLTA
        public decimal PLTALT { get; set; }
        public decimal PLTATH { get; set; }
        public decimal PLTART { get; set; }

        //final conclusion
        //fLpb = 1.0 – PLT(1 – ApbT)(1 – PLTA)
        public decimal fLpbLT { get; set; }
        public decimal fLpbTH { get; set; }
        public decimal fLpbRT { get; set; }


        /***********************************************************/

        //Conflicting bicycle volume,1,7 vbic (bicycles/h)
        public decimal vbic { get; set; }

        //Effective green,1 g (s)
        public decimal gs { get; set; }

        //vbicg = vbic(C/g)
        public decimal vbicg { get; set; }

        //OCCbicg = 0.02 + vbicg/2700
        public decimal OCCbicg { get; set; }

        //OCCr = OCCpedg + OCCbicg – (OCCpedg)(OCCbicg)
        public decimal OCCrRLT { get; set; }
        public decimal OCCrRTH { get; set; }
        public decimal OCCrRRT { get; set; }

        //ApbT = 1 – 0.6(OCCr) if Nrec > Nturn
        public decimal ApbTRLT { get; set; }
        public decimal ApbTRTH { get; set; }
        public decimal ApbTRRT { get; set; }

        //Proportion of right turns,5 PRT
        public decimal PRT { get; set; }

        public decimal PRTALT { get; set; }

        public decimal PRTATH { get; set; }

        public decimal PRTART { get; set; }

        //fRpb = 1.0 – PRT(1 – ApbT)(1 – PRTA)

        public decimal fRpbLT { get; set; }

        public decimal fRpbTH { get; set; }

        public decimal fRpbRT { get; set; }
    }
}