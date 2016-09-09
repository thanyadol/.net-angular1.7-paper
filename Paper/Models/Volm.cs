using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default
{
    public class Volm
    {
        //voulnm
        public decimal VolumnLT { get; set; }
        public decimal VolumnTH { get; set; }
        public decimal VolumnRT { get; set; }

        public decimal PeakHourFactor { get; set; }
        public string SignalType { get; set; }
        public string StartLostTime { get; set; }
        public string EffectGreenTime { get; set; }
        public decimal ArrivalType { get; set; }

        public decimal ApprochPedestrian { get; set; }
        public decimal ApprochBicycle { get; set; }
        public string IsParking { get; set; }
        public decimal ParkingVolumn { get; set; }

        public decimal Bus { get; set; }
        public decimal MinTimePedestrian { get; set; }

        public string LaneGroupLT { get; set; }
        
        public string LaneGroupTH { get; set; }
        public string LaneGroupRT { get; set; }

        public string LeftTurnLane { get; set; }
        public string LeftTurnPhase { get; set; }

        public string RightTurnLane { get; set; }
        public string RightTurnPhase { get; set; }


        //---------------------- for output --------------------------------//
        public decimal AdjustFlowRateLT { get; set; }
        public decimal AdjustFlowRateTH { get; set; }
        public decimal AdjustFlowRateRT { get; set; }

        public decimal AdjustFlowRateLaneGroup { get; set; }

        public decimal ProportionLT { get; set; }
        public decimal ProportionRT { get; set; }
        public decimal ProportionTH { get; set; }

        //saturation flow rate
        public decimal BaseSaturationFlow { get; set; }

        public decimal NumberOfLane { get; set; }

        //adjustment factor
        public decimal LaneWidthAF { get; set; }
        public decimal HeavyVehicleAF { get; set; }
        public decimal GradeAF { get; set; }
        public decimal ParkingAF { get; set; }
        public decimal BusBlocktageAF { get; set; }
        public decimal AreaTypeAF { get; set; }
        public decimal LaneUitilizeAF { get; set; }

        public decimal LeftTurnAF { get; set; }

        public decimal RightTurnAF { get; set; }

        public decimal LeftTurnPedBikeAF { get; set; }

        public decimal RightTurnPedBikeAF { get; set; }


        //amount of flow
        public decimal AsjustSaturationFlow { get; set; }
    }


}