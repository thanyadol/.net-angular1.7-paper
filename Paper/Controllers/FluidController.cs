using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;

namespace Default
{
    public class FluidController : ApiController
    {
        //step 1;
        public IHttpActionResult Volumn(Cart cart)
        {
            //simply valid
            if(cart == null)
            {
                return BadRequest();
            }

            if (cart.volmlist == null)
            {
                return BadRequest();
            }

            List<Volm> result = new List<Volm>();

            //loop by leg
            foreach(Volm mm in cart.volmlist)
            {
                decimal sumvol =  mm.VolumnLT + mm.VolumnRT + mm.VolumnTH;
               

                Volm vlm = new Volm
                {
                    //volumn
                    VolumnLT = mm.VolumnLT,
                    VolumnRT = mm.VolumnRT,
                    VolumnTH = mm.VolumnTH,

                    //peak hour -- < 1.0
                    PeakHourFactor = mm.PeakHourFactor > 1 ? 1 : mm.PeakHourFactor,

                    //adjust flow rate -- column / peak hour
                    AdjustFlowRateLT =  mm.VolumnLT / mm.PeakHourFactor,
                    AdjustFlowRateRT = mm.VolumnRT / mm.PeakHourFactor,
                    AdjustFlowRateTH = mm.VolumnTH / mm.PeakHourFactor,

                    //lane group
                    LaneGroupLT = mm.LaneGroupLT,
                    LaneGroupRT = mm.LaneGroupRT,
                    LaneGroupTH = mm.LaneGroupTH,

                    //poppostion v divide sum of v
                    //propotionTH = 0
                    ProportionLT = mm.LaneGroupLT == "0" ?  mm.VolumnLT / sumvol : 1,
                    ProportionRT = mm.LaneGroupRT == "0" ?  mm.VolumnRT / sumvol : 1,
                    ProportionTH = 0,

                    //sat flow

                };

                vlm.AdjustFlowRateLaneGroup = vlm.AdjustFlowRateLT + vlm.AdjustFlowRateRT + vlm.AdjustFlowRateTH; 

                result.Add(vlm);
            }

            Thread.Sleep(3000);
            return Ok(result);
        }

    }
}
