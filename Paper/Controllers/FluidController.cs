using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Diagnostics;


namespace Default
{
    public class FluidController : ApiController
    {
        //step 1;
        public IHttpActionResult Volumn(Cart cart)
        {
            //simply valid
            if (cart == null)
            {
                return BadRequest();
            }

            if (cart.volmlist == null)
            {
                return BadRequest();
            }

            /*************** calculate sheet 2 volumn and sat flow***************/

            List<Volm> vol_list = new List<Volm>();
            //general data
            Genr gnr = cart.genr;
            int i = 0;
            foreach (Volm mm in cart.volmlist)
            {
                //geo and signal data by larg index
                Geome geo = cart.geolist[i];
                Signr sign = cart.signllist[i];

                //sat flow
                Volm temp = this.CalculateVolumn(mm, geo, gnr, i);

                vol_list.Add(temp);
                i++;
            }

            //fLT
            /*************** calculate sheet 6 and 7 suppre multilane ***************/


            List<Oppsr> oppose_list = new List<Oppsr>();
            int j = 0;
  
            foreach (Volm vm in vol_list)
            {
                Geome geo = cart.geolist[j];
                Volm vmOPPS = null;

                //select volumn
                if (j == 0)
                    vmOPPS = vol_list[1];
                else if (j == 1)
                    vmOPPS = vol_list[0];
                else if (j == 2)
                    vmOPPS = vol_list[3];
                else if (j == 3)
                    vmOPPS = vol_list[2];
                else { }

                Oppsr sm = new Oppsr();
                
                try
                {
                   sm = this.CalculateOppose(cart.signllist, gnr, geo, vm, vmOPPS, j);

                }
                catch (DivideByZeroException ex)
                {
                    return Ok("Premitted->" + ex.Message);
                }
            
                oppose_list.Add(sm);
                j++;
            }


            /*************** premitee left/right turn ped bike sh-8 ***************/
            //premitee left/right turn ped bike sh-8
            int k = 0;
            List<Bicycl> bicycl_list = new List<Bicycl>();
            foreach (Volm vs in vol_list)
            {
                Geome geo = cart.geolist[k];
                Oppsr opr = oppose_list[k];
                Volm vmOPPS = null;

                //select volumn
                if (k == 0)
                    vmOPPS = vol_list[1];
                else if (k == 1)
                    vmOPPS = vol_list[0];
                else if (k == 2)
                    vmOPPS = vol_list[3];
                else if (k == 3)
                    vmOPPS = vol_list[2];
                else
                { }

                Bicycl bc = CalculateBicycle(vs, geo, opr, vmOPPS);

                bicycl_list.Add(bc);
                k++;
            }

            /*************** set value to volumn list ***************/
            List<Volm> result_list = new List<Volm>();
            int l = 0;

            foreach (Volm vv in vol_list)
            {
                Geome geo = cart.geolist[l];
                Oppsr oppr = oppose_list[l];

                decimal NoLT = Convert.ToDecimal(geo.NlmLT);
                decimal NoTH = Convert.ToDecimal(geo.NlmTH);
                decimal NoRT = Convert.ToDecimal(geo.NlmRT);

                //
                vv.fLTLT = oppr.fLTLT;
                vv.fLTTH = oppr.fLTTH;
                vv.fLTRT = oppr.fLTRT;

                //overwrite if sh- 6  7 not occured
                if (vv.LTp == "1")
                {
                    //
                    vv.fLTLT = vv.NnLT > 0 ? 0.950M : 0;
                    vv.fLTTH = vv.NnTH > 0 ? 0.950M : 0;
                    vv.fLTRT = vv.NnRT > 0 ? 0.950M : 0;

                }
                else if (vv.LTp == "3")
                {
                    vv.fLTLT = vv.NnLT > 0 ? 1 / (1 + 0.5M * oppr.PltLT) : 0;
                    vv.fLTTH = vv.NnTH > 0 ? 1 / (1 + 0.5M * oppr.PltTH) : 0;
                    vv.fLTRT = vv.NnRT > 0 ? 1 / (1 + 0.5M * oppr.PltRT) : 0;
                }

                //
                vv.fLpbLT = bicycl_list[l].fLpbLT;
                vv.fLpbTH = bicycl_list[l].fLpbTH;
                vv.fLpbRT = bicycl_list[l].fLpbRT;

                //same
                vv.fRpbLT = bicycl_list[l].fRpbLT;
                vv.fRpbTH = bicycl_list[l].fRpbTH;
                vv.fRpbRT = bicycl_list[l].fRpbRT;

                //LT
                //final calculate
                if (NoLT != 0)
                {
                    //amount of flow adj
                    vv.sLT = vv.LGLT != "A" ?
                        (vv.S0 * vv.NnLT * vv.fwLT * vv.fHVLT
                          * vv.fgLT * vv.fpLT * vv.fbbLT * vv.faLT * vv.fLULT * vv.fLTLT * vv.fRTLT
                          * vv.fLpbLT * vv.fRpbLT) : 0;

                }
                else { vv.sLT = 0; }

                //TH
                //final calculate
                if (NoTH != 0)
                {
                    //amount of flow adj

                    vv.sTH = (vv.S0 * vv.NnTH * vv.fwTH * vv.fHVTH
                          * vv.fgTH * vv.fpTH * vv.fbbTH * vv.faTH * vv.fLUTH * vv.fLTTH * vv.fRTTH
                          * vv.fLpbTH * vv.fRpbTH);

                }
                else { vv.sTH = 0; }

                //poppostion v divide sum of v
                //propotionTH = 0

                /***************************************************************************/

                //LT
                //final calculate
                if (NoRT != 0)
                {
                    //amount of flow adj
                    vv.sRT = vv.LGRT != "A" ?
                        (vv.S0 * vv.NnRT * vv.fwRT * vv.fHVRT
                          * vv.fgRT * vv.fpRT * vv.fbbRT * vv.faRT * vv.fLURT * vv.fLTRT * vv.fRTRT
                          * vv.fLpbRT * vv.fRpbRT) : 0;

                }
                else { vv.sRT = 0; }

                result_list.Add(vv);
                l++;
            }

            /*************** calculate capacity sh-3 ***************/
            //calculate capacity
            List<Phsr> phase_list = new List<Phsr>();
            int m = 0;
            //total lost ime L(s)
            // decimal sumLs = 0;

            foreach (Volm vx in result_list)
            {
                Signr sig1 = cart.signllist[m];
                //Signr sig2 = cart.signllist[m + 1];

                //assign temp value !!!
                //sig2.TimeG = sig1.TimeG;
                //sig2.TimeY = sig1.TimeY;

                Oppsr opr = oppose_list[m];

                //prepare
                List<Signr> isig = new List<Signr>();
                isig.Add(sig1);
                //isig.Add(sig2);

                //volumn
               // List<Volm> vmlist = new List<Volm>();
                //vmlist.Add(result_list[m]);
                if (m % 2 == 0)
                {
                    //vmlist.Add(result_list[0]);
                   // vmlist.Add(result_list[1]);

                    
                }
                else
                {
                    //assign temp value !!!
                    sig1.TimeG = cart.signllist[m - 1].TimeG;
                    sig1.TimeY = cart.signllist[m - 1].TimeY;

                  //  vmlist.Add(result_list[2]);
                  //  vmlist.Add(result_list[3]);

                }

                int phase = 1;
                if(m > 1)
                {
                    phase = 2;
                }

                Phsr phs = CalculateCapacity(isig, vx, opr, phase);

                //sumLs = sumLs + Convert.ToDecimal(vx.l1);

                phase_list.Add(phs);
                m++;
            }

            //gobal value
            //sum of flow ratios for critical 
           // decimal sumyc = phase_list.Sum(c => c.Cvs);

            decimal cvs1 = phase_list[0].Cvs > phase_list[1].Cvs ? phase_list[0].Cvs : phase_list[1].Cvs;
            decimal cvs2 = phase_list[2].Cvs > phase_list[3].Cvs ? phase_list[2].Cvs : phase_list[3].Cvs;

            phase_list[0].sumYc = cvs1 + cvs2;


            //total lost time per cycle,
            //decimal sumLs = phase_list.Sum(c => c.Ls);
            phase_list[0].Ls = phase_list[0].Ls + phase_list[2].Ls;

            //xritical flow rate to capacity ratio,
            decimal sumXc = ((phase_list[0].sumYc * phase_list[0].Cs) / (phase_list[0].Cs - phase_list[0].Ls));
            phase_list[0].Xc = sumXc;


            //**************************************************************************/
            List<Loss> loss_lisy = new List<Loss>();
            int n = 0;
            foreach (Volm v in result_list)
            {
                Oppsr o = oppose_list[n];
                Phsr p = phase_list[n];

                Loss loss = new Loss();
            
                loss = CalculateLoss(v, p, o);
     
                loss_lisy.Add(loss);
                n++;
            }


            //all result
            Cart ctr = new Cart();

            //intersection delay
            decimal sumDA = loss_lisy.Sum(c => (c.dA * c.vA));
            decimal sumVA = loss_lisy.Sum(c => (c.vA));

            //decimal VCA = sumVA / 4;
            ctr.dI = sumDA / sumVA;

            ctr.sumDA = sumDA;
            ctr.sumVA = loss_lisy.Sum(c => c.XLT + c.XRT + c.XTH) / 4;


            if (ctr.sumVA > 1)
            {
                ctr.dISTR = "F";
            }
            else
            {
                if (ctr.dI <= 10)
                {
                    ctr.dISTR = "A";
                }
                else if (ctr.dI > 10 && ctr.dI <= 20)
                {
                    ctr.dISTR = "B";
                }
                else if (ctr.dI > 20 && ctr.dI <= 35)
                {
                    ctr.dISTR = "C";
                }
                else if (ctr.dI > 35 && ctr.dI <= 55)
                {
                    ctr.dISTR = "D";
                }
                else if (ctr.dI > 55 && ctr.dI <= 80)
                {
                    ctr.dISTR = "E";
                }
                else
                {
                    ctr.dISTR = "F";
                }
            }

            ctr.volmlist = result_list;
            ctr.opposelist = oppose_list;
            ctr.bicyclelist = bicycl_list;
            ctr.phaselist = phase_list;
            ctr.losslist = loss_lisy;

            Thread.Sleep(500);
            return Ok(ctr);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Volm CalculateVolumn(Volm mm, Geome geo, Genr gnr, int index)
        {
            decimal NoLT = Convert.ToDecimal(geo.NlmLT);
            decimal NoTH = Convert.ToDecimal(geo.NlmTH);
            decimal NoRT = Convert.ToDecimal(geo.NlmRT);

            Volm vl;

            vl = new Volm()
            {
                //PHF = mm.PHF,
                PA = mm.PA,
                l1 = mm.l1,
                e = mm.e,

                //new value
                T = mm.T,

                AT = mm.AT,

                Vped = mm.Vped,
                vbic = mm.vbic,
                Pyos = mm.Pyos,
                Nm = mm.Nm > 180 ? 180 : mm.Nm,

                NB = mm.NB,
                Gps = mm.Gps,

                Nlt = mm.Nlt,
                LTp = mm.LTp,

                Nrt = mm.Nrt,
                RTp = mm.RTp,

                //new Nrec, Nturn
                Nrec = mm.Nrec,
                Nturn = mm.Nturn,

                //volumn
                VLT = mm.VLT,
                VRT = mm.VRT,
                VTH = mm.VTH,

                //peak hour -- < 1.0
                PHF = mm.PHF > 1 ? 1 : mm.PHF,

                //adjust flow rate -- column / peak hour
                vpLT = mm.VLT / mm.PHF,
                vpRT = mm.VRT / mm.PHF,
                vpTH = mm.VTH / mm.PHF,

                //lane group
                LGLT = mm.LGLT,
                LGRT = mm.LGRT,
                LGTH = mm.LGTH,

                //sat flow LT TH RT 
                S0 = gnr.StationFlow,

                //No of Lane LT TH RT
                NnLT = NoLT,
                NnTH = NoTH,
                NnRT = NoRT,
            };

            //LT case
            if (NoLT != 0)
            {
                //lane width adj flow, 
                vl.fwLT = /*mm.LGLT == "A" ?*/ 1 + ((geo.Width - 3.6M) / 9); /*: 0;*/

                //heavey vehicle adj
                //L1 L2
                if (index < 2)
                {
                    vl.fHVLT = 100 / (100 + gnr.HvL1L2);
                }
                //L3 L4
                else
                {
                    vl.fHVLT = 100 / (100 + gnr.HvL3L4);
                }

                //adjust parking
                if (mm.Pyos == "YES")
                {
                    // >= 0.5
                    vl.fpLT = /*mm.LGLT == "A" ?*/ (NoLT - 0.1M - ((18M * mm.Nm) / 3600)) / NoLT; //: 0;
                }
                else
                {
                    vl.fpLT = 1.0M;
                }

                //bus blog adj cal
                if (mm.NB != 0)
                {
                    vl.fbbLT = (NoLT - ((14.4M * mm.NB) / 3600)) / NoLT;
                }
                else
                {
                    vl.fbbLT = 1.0M;
                }

                //area ajd factor
                if (gnr.Area.ToUpper() == "CBD")
                {
                    vl.faLT = 0.9M;
                }
                else
                {
                    vl.faLT = 1.0M;
                }

                //lane utilize adj factor
                if (NoLT == 1)
                {
                    vl.fLULT = 1.0M;
                }
                else
                {
                    vl.fLULT = 0.971M;
                }

                //grade
                vl.fgLT = 1 - (geo.Grade / 200);
            }
            else { }

            //TH case
            if (NoTH != 0)
            {
                vl.fwTH = 1 + ((geo.Width - 3.6M) / 9);

                //heavey vehicle adj
                //L1 L2
                if (index < 2)
                {
                    vl.fHVTH = 100 / (100 + gnr.HvL1L2);
                }
                //L3 L4
                else
                {
                    vl.fHVTH = 100 / (100 + gnr.HvL3L4);
                }

                if (mm.Pyos == "YES")
                {
                    // >= 0.5
                    vl.fpTH = (NoTH - 0.1M - ((18M * mm.Nm) / 3600)) / NoTH;
                }
                else
                {
                    vl.fpTH = 1.0M;
                }

                //bus blog adj cal
                if (mm.NB != 0)
                {
                    vl.fbbTH = (NoTH - ((14.4M * mm.NB) / 3600)) / NoTH;
                }
                else
                {
                    vl.fbbTH = 1.0M;
                }

                //area ajd factor
                if (gnr.Area.ToUpper() == "CBD")
                {
                    vl.faTH = 0.9M; ;
                }
                else
                {
                    vl.faTH = 1.0M;
                }

                //lane utilize adj factor
                if (NoTH == 1)
                {
                    vl.fLUTH = 1.0M;
                }
                else if (NoTH == 2)
                {
                    vl.fLUTH = 0.952M;
                }
                else
                {
                    vl.fLUTH = 0.908M;
                }

                //grade adj factor
                vl.fgTH = 1 - (geo.Grade / 200);
            }
            else { }

            //RT case
            if (NoRT != 0)
            {
                //lane width adj flow, 
                vl.fwRT = /*mm.LGRT == "A" ?*/ 1 + ((geo.Width - 3.6M) / 9); // : 0;

                //heavey vehicle adj
                //L1 L2
                if (index < 2)
                {
                    vl.fHVRT = /*mm.LGRT == "A" ?*/ 100 / (100 + gnr.HvL1L2); // : 0;
                }
                //L3 L4
                else
                {
                    vl.fHVRT = /*mm.LGRT == "A" ?*/ 100 / (100 + gnr.HvL3L4); // : 0;
                }

                //adjust parking
                if (mm.Pyos == "YES")
                {
                    // >= 0.5
                    vl.fpRT = (NoRT - 0.1M - ((18M * mm.Nm) / 3600)) / NoRT; // : 0;
                }
                else
                {
                    vl.fpRT = 1.0M;
                }

                //bus blog adj cal
                if (mm.NB != 0)
                {
                    vl.fbbRT = (NoRT - ((14.4M * mm.NB) / 3600)) / NoRT;
                }
                else
                {
                    vl.fbbRT = 1.0M;
                }

                //area ajd factor
                if (gnr.Area.ToUpper() == "CBD")
                {
                    vl.faRT = 0.9M;
                }
                else
                {
                    vl.faRT = 1.0M;
                }

                //lane utilize adj factor
                if (NoRT == 1)
                {
                    vl.fLURT = 1.0M;
                }
                else
                {
                    vl.fLURT = 0.971M;
                }

                //grade
                vl.fgRT = 1 - (geo.Grade / 200);
            }
            else { }

            /******************************************************************************************/

            //adj flow lane group     
            //  vl.Vv = vllegOPPOSITvm + vl.vpRT + vl.vpTH; 
            if (NoLT == 0 && NoRT == 0)
            {
                vl.VvLT = 0;
                vl.VvRT = 0;

                vl.VvTH = vl.vpLT + vl.vpRT + vl.vpTH;
            }
            else if (NoLT > 0 && NoRT == 0)
            {
                vl.VvRT = 0;

                vl.VvLT = vl.vpLT;
                vl.VvTH = vl.vpRT + vl.vpTH;
            }
            else if (NoLT == 0 && NoRT > 0)
            {
                vl.VvLT = 0;

                vl.VvRT = vl.vpLT;
                vl.VvTH = vl.vpLT + vl.vpTH;
            }
            else
            {
                vl.VvLT = vl.vpLT; //+ vv.vpRT + vv.vpTH; 
                vl.VvTH = vl.vpTH; //+ vv.vpRT + vv.vpTH; 
                vl.VvRT = vl.vpRT; //+ vv.vpRT + vv.vpTH; 
            }


            //final conclusion 
            //fRT
            switch (vl.RTp)
            {
                case "1":
                    vl.fRTLT = 0;
                    vl.fRTTH = 0;
                    vl.fRTRT = 0;
                    break;

                case "2":
                    vl.fRTLT = 0.850M;
                    vl.fRTTH = 0.850M;
                    vl.fRTRT = 0.850M;
                    break;

                case "3":
                    decimal t = 1 - (0.15M * vl.PRT);
                    vl.fRTLT = t;
                    vl.fRTTH = t;
                    vl.fRTRT = t;
                    break;

                case "4":
                    decimal s = 1 - (0.135M * vl.PRT);
                    vl.fRTLT = s;
                    vl.fRTTH = s;
                    vl.fRTRT = s;
                    break;

                case "5":
                    vl.fRTLT = 1;
                    vl.fRTTH = 1;
                    vl.fRTRT = 1;
                    break;
            }

            //popotion LT/RT
            decimal sumV = vl.VLT + vl.VRT + vl.VTH;
            vl.PLT = vl.LGLT == "A" ? vl.VLT / sumV : 1;
            vl.PRT = vl.LGRT == "A" ? vl.VRT / sumV : 1;


            return vl;

        }

        private Bicycl CalculateBicycle(Volm vs, Geome geo, Oppsr opr, Volm legOPPOSITvm)
        {
            decimal NoLT = Convert.ToDecimal(geo.NlmLT);
            decimal NoTH = Convert.ToDecimal(geo.NlmTH);
            decimal NoRT = Convert.ToDecimal(geo.NlmRT);

            Bicycl bc = new Bicycl()
            {
                gp = opr.Gs,
                vped = vs.Vped,

                //Nrec, Nturn
                NrecL = vs.Nrec,
                NturnL = vs.Nturn,

                NrecR = vs.Nrec,
                NturnR = vs.Nturn,

            };

            //LT
            if (NoLT != 0)
            {
                //Permitted Left Turns				
                bc.vpedg = bc.vped * (opr.Cs / bc.gp);

                if (bc.vpedg <= 1000)
                {
                    bc.OCCpedg = bc.vpedg / 2000;
                }
                // > 1000 
                else
                {
                    bc.OCCpedg = 0.4M + (bc.vpedg / 10000);
                }

                //sheet 6 / 7
                //if (vs.LTp == "2.1" || vs.LTp == "4.1")
                //else (vs.LTp == "2.2" || vs.LTp == "4.2")
                bc.gqLT = opr.GqLT;

                bc.fgqgpLT = bc.gqLT / bc.gp;

                bc.OCCpeduLT = bc.OCCpedg * (1 - (0.5M * (bc.fgqgpLT)));

                //use leg2
                bc.v0LT = legOPPOSITvm.VvLT;

                //
                double exp = Math.Exp(-5 * (double)bc.v0LT / 3600);
                bc.OCCrLLT = bc.OCCpeduLT * (decimal)exp;

                //PLT
                bc.PLT = vs.PLT;

                //Apbt
                if (bc.NrecL == bc.NturnL)
                {
                    bc.ApbTLLT = 1 - bc.OCCrLLT;
                }
                else
                {
                    bc.ApbTLLT = 1 - (0.6M * bc.OCCrLLT);
                }

                //PLTA
                if (vs.LTp == "1" || vs.LTp == "3")
                {
                    bc.PLTALT = (1 - vs.fLTLT) / 0.95M;
                }
                else //2.1 - 4.2
                {
                    bc.PLTALT = 0;
                }

                //final conclusion
                if (bc.gqLT > bc.gp)
                {
                    bc.fLpbLT = 1;
                }
                else
                {
                    bc.fLpbLT = (decimal)(1.0M - (bc.PLT * (1 - bc.ApbTLLT) * (1 - bc.PLTALT)));
                }

            }
            else { }

            //TH
            if (NoTH != 0)
            {
                //Permitted Left Turns				
                bc.vpedg = bc.vped * (opr.Cs / bc.gp);

                if (bc.vpedg <= 1000)
                {
                    bc.OCCpedg = bc.vpedg / 2000;
                }
                // > 1000 
                else
                {
                    bc.OCCpedg = 0.4M + (bc.vpedg / 10000);
                }

                //sheet 6 / 7
                //if (vs.LTp == "2.1" || vs.LTp == "4.1")
                //else (vs.LTp == "2.2" || vs.LTp == "4.2")
                bc.gqTH = opr.GqTH;

                bc.fgqgpTH = bc.gqTH / bc.gp;

                bc.OCCpeduTH = bc.OCCpedg * (1 - (0.5M * (bc.fgqgpTH)));

                //use leg2
                bc.v0TH = legOPPOSITvm.VvTH;

                //
                double exp = Math.Exp(-5 * (double)bc.v0TH / 3600);
                bc.OCCrLTH = bc.OCCpeduTH * (decimal)exp;

                //PLT
                bc.PLT = vs.PLT;

                //Apbt
                if (bc.NrecL == bc.NturnL)
                {
                    bc.ApbTLTH = 1 - bc.OCCrLTH;
                }
                else
                {
                    bc.ApbTLTH = 1 - (0.6M * bc.OCCrLTH);
                }

                //PLTA
                if (vs.LTp == "1" || vs.LTp == "3")
                {
                    bc.PLTATH = (1 - vs.fLTTH) / 0.95M;
                }
                else
                {
                    bc.PLTATH = 0;
                }

                //final conclusion
                if (bc.gqTH > bc.gp)
                {
                    bc.fLpbTH = 1;
                }
                else
                {
                    bc.fLpbTH = (decimal)(1.0M - (bc.PLT * (1 - bc.ApbTLTH) * (1 - bc.PLTATH)));
                }

            }
            else { }


            //RT
            if (NoRT != 0)
            {
                //Permitted Left Turns				
                bc.vpedg = bc.vped * (opr.Cs / bc.gp);

                if (bc.vpedg <= 1000)
                {
                    bc.OCCpedg = bc.vpedg / 2000;
                }
                // > 1000 
                else
                {
                    bc.OCCpedg = 0.4M + (bc.vpedg / 10000);
                }

                //sheet 6 / 7
                //if (vs.LTp == "2.1" || vs.LTp == "4.1")
                //else (vs.LTp == "2.2" || vs.LTp == "4.2")
                bc.gqRT = opr.GqRT;

                bc.fgqgpRT = bc.gqRT / bc.gp;

                bc.OCCpeduRT = bc.OCCpedg * (1 - (0.5M * (bc.fgqgpRT)));

                //use leg2
                bc.v0RT = legOPPOSITvm.VvRT;

                //
                double exp = Math.Exp(-5 * (double)bc.v0RT / 3600);
                bc.OCCrLRT = bc.OCCpeduRT * (decimal)exp;

                //PLT
                bc.PLT = vs.PLT;

                //Apbt
                if (bc.NrecL == bc.NturnL)
                {
                    bc.ApbTLRT = 1 - bc.OCCrLRT;
                }
                else
                {
                    bc.ApbTLRT = 1 - (0.6M * bc.OCCrLRT);
                }

                //PLTA
                if (vs.LTp == "1" || vs.LTp == "3")
                {
                    bc.PLTART = 1 - (vs.fLTRT / 0.95M);
                }
                else
                {
                    bc.PLTART = 0;
                }

                //final conclusion
                if (bc.gqRT > bc.gp)
                {
                    bc.fLpbRT = 1;
                }
                else
                {
                    bc.fLpbRT = (decimal)(1.0M - (bc.PLT * (1 - bc.ApbTLRT) * (1 - bc.PLTART)));
                }

            }
            else { }

            /***************** common right turn *****************************/

            bc.gs = bc.gp;
            bc.vbic = vs.vbic;

            bc.vbicg = bc.vbic * (opr.Cs / bc.gp);

            bc.OCCbicg = 0.02M + (bc.vbicg / 2700);

            bc.OCCrRLT = bc.OCCpedg + bc.OCCbicg - (bc.OCCpedg * bc.OCCbicg);
            bc.OCCrRTH = bc.OCCrRLT;
            bc.OCCrRRT = bc.OCCrRLT;

            //Apbt
            if (bc.NrecR == bc.NturnR)
            {
                bc.ApbTRLT = 1 - bc.OCCrRLT;
                bc.ApbTRTH = 1 - bc.OCCrRTH;
                bc.ApbTRRT = 1 - bc.OCCrRRT;
            }
            else
            {
                bc.ApbTRLT = 1 - (0.6M * bc.OCCrRLT);
                bc.ApbTRTH = 1 - (0.6M * bc.OCCrRTH);
                bc.ApbTRRT = 1 - (0.6M * bc.OCCrRRT);
            }

            bc.PRT = vs.PRT;

            bc.PRTALT = 0;
            bc.PRTATH = 0;
            bc.PRTART = 0;

            //final calculation
            bc.fRpbLT = 1.0M - (bc.PRT * (1 - bc.ApbTRLT) * (1 - bc.PRTALT));
            bc.fRpbTH = 1.0M - (bc.PRT * (1 - bc.ApbTRTH) * (1 - bc.PRTATH));
            bc.fRpbRT = 1.0M - (bc.PRT * (1 - bc.ApbTRRT) * (1 - bc.PRTART));

            //**********************************************************************/

            return bc;
        }

        private Oppsr CalculateOppose(List<Signr> signlist, Genr gnr, Geome geo, Volm vm, Volm legOPPOSITvm, int index)
        {
            //loop with signal.phase
            decimal SumCs = 0;
            decimal SumGs = 0;

            //*** sum from pahse 1 - 8
            foreach (Signr sl in signlist)
            {
                //sum of Cs
                SumCs = SumCs + sl.TimeG + sl.TimeY;

                if (sl.Leg == "1")
                {
                    SumGs = SumGs + sl.TimeG;
                }
            }

            ////0,5 1,6 2,7 3,8
            //fnd gg
            decimal gg = 0;
            if (index == 1 || index == 3)
            {
                 gg = signlist[index - 1].TimeG;
            }
            else
            {
                 gg = signlist[index].TimeG;
            }
   
            decimal NoLT = Convert.ToDecimal(geo.NlmLT);
            decimal NoTH = Convert.ToDecimal(geo.NlmTH);
            decimal NoRT = Convert.ToDecimal(geo.NlmRT);

            Oppsr sm = new Oppsr()
            {
                Cs = SumCs,

                //two are same value
                //Gs = SumGs,
                gs = gg,
                g0 = gg,

                NnLT = NoLT,
                NnTH = NoTH,
                NnRT = NoRT,

                //same value
                vltLT = vm.vpLT,
                vltTH = vm.vpLT,
                vltRT = vm.vpLT,

                //proprotion LT
                PltLT = vm.PLT,
                PltTH = vm.PLT,
                PltRT = vm.PLT,

                //of leg2
                Plt0LT = legOPPOSITvm.PLT,
                Plt0TH = legOPPOSITvm.PLT,
                Plt0RT = legOPPOSITvm.PLT,

                //adj flow rate in langgroup opposite
                v0LT = legOPPOSITvm.VvLT,
                v0TH = legOPPOSITvm.VvTH,
                v0RT = legOPPOSITvm.VvRT,

                tL = Convert.ToDecimal(vm.l1),
            };

            //Gs
            if (index > 1)
            {
                sm.Gs = sm.gs;
            }
            else
            {
                sm.Gs = SumGs;
            }

            //Opposing platoon ratio, Rpo (refer to Exhibit 16-11) 
            decimal arrivaltype = Convert.ToDecimal(legOPPOSITvm.AT);
            switch ((int)arrivaltype)
            {
                case 1:
                    sm.Rp0LT = 0.333M;
                    sm.Rp0TH = 0.333M;
                    sm.Rp0RT = 0.333M;
                    break;

                case 2:
                    sm.Rp0LT = 0.667M;
                    sm.Rp0TH = 0.667M;
                    sm.Rp0RT = 0.667M;
                    break;

                case 3:
                    sm.Rp0LT = 1M;
                    sm.Rp0TH = 1M;
                    sm.Rp0RT = 1M;
                    break;

                case 4:
                    sm.Rp0LT = 1.333M;
                    sm.Rp0TH = 1.333M;
                    sm.Rp0RT = 1.333M;
                    break;

                case 5:
                    sm.Rp0LT = 1.667M;
                    sm.Rp0TH = 1.667M;
                    sm.Rp0RT = 1.667M;
                    break;

                case 6:
                    sm.Rp0LT = 2M;
                    sm.Rp0TH = 2M;
                    sm.Rp0RT = 2M;
                    break;
                default:
                    break;
            }


            //LT
            //check geo.number of lane
            if (NoLT != 0)
            {
                //common factor
                sm.N0LT = Convert.ToDecimal(legOPPOSITvm.NnLT);

                //computaion
                sm.LTCLT = (sm.Cs * sm.vltLT) / 3600;

                sm.fLUoLT = legOPPOSITvm.fLULT;

                //
                decimal temp = 1 - (sm.Rp0LT * (sm.g0 / sm.Cs));
                if (temp > 0)
                {
                    sm.Qr0LT = temp;
                }
                else
                {
                    sm.Qr0LT = 0;
                }

                /*********************************** 2nd calculate --sheet6 ****************************************/
                //2nd calculate --sheet6
                if (vm.LTp == "2.1" || vm.LTp == "4.1")
                {

                    //number of lane oppossing
                    sm.N0LT = Convert.ToDecimal(vm.NnTH);

                    // lt th rt
                    sm.VolcLT = (sm.v0LT * sm.Cs) /
                        (3600 * sm.N0LT * sm.fLUoLT);


                    //gf
                    if (sm.NnLT == 0)
                    {
                        sm.GfLT = 0;
                    }
                    else
                    {
                        double ltc = Math.Pow((double)sm.LTCLT, 0.717D);
                        sm.GfLT = (decimal)(((double)sm.Gs * (Math.Exp((-0.882 * ltc)))) - (double)sm.tL);
                    }

                    //lt th rt
                    sm.GqLT = ((sm.VolcLT * sm.Qr0LT) / (0.5M - (((sm.VolcLT * (1 - sm.Qr0LT))) / sm.g0))) - -sm.tL;

                    //gu is same both
                    if (sm.GqLT >= sm.GfLT)
                    {
                        sm.GuLT = sm.gs - sm.GqLT;
                    }
                    else
                    {
                        sm.GuLT = sm.gs - sm.GfLT;
                    }

                    double temptc = 0;
                    if (vm.LTp == "2.1")
                    {
                        temptc = 2.5D;
                    }
                    else if (vm.LTp == "4.1")
                    {
                        temptc = 4.5D;
                    }
                    else
                    {
                        temptc = 0;
                    }

                    double voe = (double)(sm.v0LT / sm.fLUoLT);
                    double expFactorLT = Math.Exp(((double)-voe * temptc) / 3600);
                    double SLTTemp = (voe * expFactorLT) / (1 - expFactorLT);

                    if (vm.LTp == "2.1")
                    {
                        sm.El1LT = gnr.StationFlow / (decimal)SLTTemp;
                    }
                    else if (vm.LTp == "4.1")
                    {
                        sm.El1LT = (gnr.StationFlow / (decimal)SLTTemp) - 1;
                    }
                    else
                    {
                        sm.El1LT = 0;
                    }


                    //Pl
                    sm.PlLT = sm.PltLT *
                        ((1 + ((sm.NnLT - 1) * sm.gs)) / (sm.GfLT + (sm.GuLT / sm.El1LT) + 4.24M));

                    //fmin
                    sm.FminLT = (2 * (1 + sm.PlLT)) / sm.gs;

                    //fm
                    sm.FmLT =
                        (sm.GfLT / sm.gs)
                        + (sm.GuLT / sm.gs)
                        * (1 / (1 + (sm.PlLT * (sm.El1LT - 1))));

                    //last
                    sm.fLTLT = (sm.FmLT + (0.91M * (sm.NnLT - 1))) / sm.NnLT;
                }

                //2nd calculate --sheet7
                else if (vm.LTp == "2.2" || vm.LTp == "4.2")
                {

                        //lt th rt
                        sm.VolcLT = (sm.v0LT * sm.Cs) / 3600;

                        //gf
                        double ltc = Math.Pow((double)sm.LTCLT, 0.629D);
                        sm.GfLT = (decimal)(((double)sm.Gs * (Math.Exp((-0.860 * ltc)))) - (double)sm.tL);


                        //gq
                         sm.GqLT = (decimal)((0.4943D * Math.Pow((double)sm.VolcLT, 0.762D) * (Math.Pow((double)sm.Qr0LT, 1.061))) - (double)sm.tL);

            
                        //gu is same both
                        if (sm.GqLT >= sm.GfLT)
                        {
                            sm.GuLT = sm.gs - sm.GqLT;
                        }
                        else
                        {
                            sm.GuLT = sm.gs - sm.GfLT;
                        }


                        //n
                        decimal tempn = (sm.GqLT - sm.GfLT) / 2;
                        sm.nnLT = tempn > 0 ? tempn : 0;

                        //PTH0
                        sm.PTH0LT = 1 - sm.Plt0LT;

                        //Elt
                        //*******************************/

                        //Elt
                        double temptc = 0;
                        if (vm.LTp == "2.2")
                        {
                            temptc = 2.5D;
                        }
                        else if (vm.LTp == "4.2")
                        {
                            temptc = 4.5D;
                        }
                        else
                        {
                            temptc = 0;
                        }

                        double voe = (double)(sm.v0LT / sm.fLUoLT);
                        double expFactorLT = Math.Exp(((double)-voe * temptc) / 3600);
                        double SLTTemp = (voe * expFactorLT) / (1 - expFactorLT);

                        if (vm.LTp == "2.2")
                        {
                            sm.El1LT = gnr.StationFlow / (decimal)SLTTemp;
                        }
                        else if (vm.LTp == "4.2")
                        {
                            sm.El1LT = (gnr.StationFlow / (decimal)SLTTemp) - 1;
                        }
                        else
                        {
                            sm.El1LT = 0;
                        }

                        //El2
                        double tempEl2 = (1 - Math.Pow((double)sm.PTH0LT, (double)sm.nnLT)) / (double)sm.Plt0LT;
                        sm.El2LT = tempEl2 > 1 ? (decimal)tempEl2 : 1;

                        //no plt
                        //Pl
                        sm.PlLT = sm.PltLT *
                            ((1 + ((sm.NnLT - 1) * sm.gs)) / (sm.GfLT + (sm.GuLT / sm.El1LT) + 4.24M));


                        //fmin
                        sm.FminLT = (2 * (1 + sm.PlLT)) / sm.gs;

                        //gdiff
                        decimal temgf = sm.GqLT - sm.GfLT;
                        sm.gdiffLT = temgf > 0 ? temgf : 0;

                        //last flt = fm
                        decimal fEl1FactorLT = 1 + (sm.PlLT * (sm.El1LT - 1));
                        decimal fEl2FactorLT = 1 + (sm.PlLT * (sm.El2LT - 1));

                        sm.fLTLT = (sm.GfLT / sm.gs)
                                 + ((sm.GuLT / sm.gs) / fEl1FactorLT)
                                 + ((sm.gdiffLT / sm.gs) / fEl2FactorLT);

                        //flt = fm
                        sm.FmLT = sm.fLTLT;                   

                }

            }
            else { }

            //TH
            //check geo.number of lane
            if (NoTH != 0)
            {
                //common factor
                sm.N0TH = Convert.ToDecimal(legOPPOSITvm.NnRT);

                //computaion
                sm.LTCTH = (sm.Cs * sm.vltTH) / 3600;

                sm.fLUoTH = legOPPOSITvm.fLUTH;

                decimal temp = 1 - (sm.Rp0TH * (sm.g0 / sm.Cs));
                if (temp > 0)
                {
                    sm.Qr0TH = temp;
                }
                else
                {
                    sm.Qr0TH = 0;
                }

                //2nd calculate --sheet6
                if (vm.LTp == "2.1" || vm.LTp == "4.1")
                {

                    //number of lane oppossing
                    sm.N0TH = Convert.ToDecimal(vm.NnTH);

                    // lt th rt

                    sm.VolcTH = (sm.v0TH * sm.Cs) /
                        (3600 * sm.N0TH * sm.fLUoTH);

                    //gf
                    if (sm.NnTH == 0)
                    {
                        sm.GfTH = 0;
                    }
                    else
                    {
                        double ltc = Math.Pow((double)sm.LTCTH, 0.717D);
                        sm.GfTH = (decimal)(((double)sm.Gs * (Math.Exp((-0.882 * ltc)))) - (double)sm.tL);
                    }

                   
                    //lt th rt
                    sm.GqTH = ((sm.VolcTH * sm.Qr0TH) / (0.5M - (((sm.VolcTH * (1 - sm.Qr0TH))) / sm.g0))) - sm.tL;
             

                    //gu is same both
                    if (sm.GqTH >= sm.GfTH)
                    {
                        sm.GuTH = sm.gs - sm.GqTH;
                    }
                    else
                    {
                        sm.GuTH = sm.gs - sm.GfTH;
                    }

                    //*******************************/

                    //Elt
                    double temptc = 0;
                    if (vm.LTp == "2.1")
                    {
                        temptc = 2.5D;
                    }
                    else if (vm.LTp == "4.1")
                    {
                        temptc = 4.5D;
                    }
                    else
                    {
                        temptc = 0;
                    }

                    double voe = (double)(sm.v0TH / sm.fLUoTH);
                    double expFactorTH = Math.Exp(((double)-voe * temptc) / 3600);
                    double STHTemp = (voe * expFactorTH) / (1 - expFactorTH);

                    if (vm.LTp == "2.1")
                    {
                        sm.El1TH = gnr.StationFlow / (decimal)STHTemp;
                    }
                    else if (vm.LTp == "4.1")
                    {
                        sm.El1TH = (gnr.StationFlow / (decimal)STHTemp) - 1;
                    }
                    else
                    {
                        sm.El1TH = 0;
                    }

                    //Pl
                    sm.PlTH = sm.PltTH *
                        (1 + (((sm.NnTH - 1) * sm.gs) / (sm.GfTH + (sm.GuTH / sm.El1TH) + 4.24M)));

                    //fmin
                    sm.FminTH = (2 * (1 + sm.PlTH)) / sm.gs;

                    //fm
                    sm.FmTH =
                        (sm.GfTH / sm.gs)
                        + (sm.GuTH / sm.gs)
                        * (1 / (1 + (sm.PlTH * (sm.El1TH - 1))));

                    //last
                    sm.fLTTH = (sm.FmTH + (0.91M * (sm.NnTH - 1))) / sm.NnTH;

                }
                //2nd calculate --sheet7
                else if (vm.LTp == "2.2" || vm.LTp == "4.2")
                {
                   
                    // lt th rt
                    sm.VolcTH = (sm.v0TH * sm.Cs) / 3600;

                    //gf
                    double ltc = Math.Pow((double)sm.LTCTH, 0.629D);
                    sm.GfTH = (decimal)(((double)sm.Gs * (Math.Exp((-0.860 * ltc)))) - (double)sm.tL);

                   //gq
                    sm.GqTH = (decimal)((4.943D * Math.Pow((double)sm.VolcTH, 0.762D) * (Math.Pow((double)sm.Qr0TH, 1.061))) - (double)sm.tL);

             
                    //gu is same both
                    if (sm.GqTH >= sm.GfTH)
                    {
                        sm.GuTH = sm.gs - sm.GqTH;
                    }
                    else
                    {
                        sm.GuTH = sm.gs - sm.GfTH;
                    }


                    //n
                    decimal tempn = (sm.GqTH - sm.GfTH) / 2;
                    sm.nnTH = tempn > 0 ? tempn : 0;

                    //PTH0
                    sm.PTH0TH = 1 - sm.Plt0TH;

                    //Elt
                    double temptc = 0;
                    if (vm.LTp == "2.2")
                    {
                        temptc = 2.5D;
                    }
                    else if (vm.LTp == "4.2")
                    {
                        temptc = 4.5D;
                    }
                    else
                    {
                        temptc = 0;
                    }


                    double voe = (double)(sm.v0TH / sm.fLUoTH);
                    double expFactorTH = Math.Exp(((double)-voe * temptc) / 3600);
                    double STHTemp = (voe * expFactorTH) / (1 - expFactorTH);

                    if (vm.LTp == "2.2")
                    {
                        sm.El1TH = gnr.StationFlow / (decimal)STHTemp;
                    }
                    else if (vm.LTp == "4.2")
                    {
                        sm.El1TH = (gnr.StationFlow / (decimal)STHTemp) - 1;
                    }
                    else
                    {
                        sm.El1TH = 0;
                    }


                    //El2
                    double tempEl2 = (1 - Math.Pow((double)sm.PTH0TH, (double)sm.nnTH)) / (double)sm.Plt0TH;
                    sm.El2TH = tempEl2 > 1 ? (decimal)tempEl2 : 1;


                    //Pl
                    sm.PlTH = sm.PltTH *
                        (1 + (((sm.NnTH - 1) * sm.gs) / (sm.GfTH + (sm.GuTH / sm.El1TH) + 4.24M)));


                    //fmin
                    sm.FminTH = (2 * (1 + sm.PlTH)) / sm.gs;

                    //gdiff
                    decimal temgf = sm.GqTH - sm.GfTH;
                    sm.gdiffTH = temgf > 0 ? temgf : 0;

                    //last flt = fm
                    decimal fEl1FactorTH = 1 + (sm.PlTH * (sm.El1TH - 1));
                    decimal fEl2FactorTH = 1 + (sm.PlTH * (sm.El2TH - 1));

                    sm.fLTTH = (sm.GfTH / sm.gs)
                             + ((sm.GuTH / sm.gs) / fEl1FactorTH)
                             + ((sm.gdiffTH / sm.gs) / fEl2FactorTH);

                    //flt = fm
                    sm.FmTH = sm.fLTTH;

                }

            }
            else { }

            //RT
            //check geo.number of lane
            if (NoRT != 0)
            {
                //common factor
                sm.N0RT = Convert.ToDecimal(legOPPOSITvm.NnLT);

                //computaion
                sm.LTCRT = (sm.Cs * sm.vltRT) / 3600;

                sm.fLUoRT = legOPPOSITvm.fLURT;

                decimal temp = 1 - (sm.Rp0RT * (sm.g0 / sm.Cs));
                if (temp > 0)
                {
                    sm.Qr0RT = temp;
                }
                else
                {
                    sm.Qr0RT = 0;
                }

                //2nd calculate --sheet6
                if (vm.LTp == "2.1" || vm.LTp == "4.1")
                {

                    //number of lane oppossing
                    sm.N0RT = Convert.ToDecimal(vm.NnRT);

                    // lt th rt

                    sm.VolcRT = (sm.v0RT * sm.Cs) /
                        (3600 * sm.N0RT * sm.fLUoRT);

                    //gf
                    if (sm.NnRT == 0)
                    {
                        sm.GfRT = 0;
                    }
                    else
                    {
                        double ltc = Math.Pow(Convert.ToDouble(sm.LTCRT), 0.717D);
                        sm.GfRT = (decimal)(((double)sm.Gs * (Math.Exp((-0.882 * ltc)))) - (double)sm.tL);
                    }


                    //lt th rt
                    sm.GqRT = ((sm.VolcRT * sm.Qr0RT) / (0.5M - (((sm.VolcRT * (1 - sm.Qr0RT))) / sm.g0))) - sm.tL;

                    //gu is same both
                    if (sm.GqRT >= sm.GfRT)
                    {
                        sm.GuRT = sm.gs - sm.GqRT;
                    }
                    else
                    {
                        sm.GuRT = sm.gs - sm.GfRT;
                    }

                    //*******************************/

                    //Elt
                    double temptc = 0;
                    if (vm.LTp == "2.1")
                    {
                        temptc = 2.5D;
                    }
                    else if (vm.LTp == "4.1")
                    {
                        temptc = 4.5D;
                    }
                    else
                    {
                        temptc = 0;
                    }

                    double voe = (double)(sm.v0RT / sm.fLUoRT);
                    double expFactorRT = Math.Exp(((double)-voe * temptc) / 3600);
                    double SRTTemp = (voe * expFactorRT) / (1 - expFactorRT);

                    if (vm.LTp == "2.1")
                    {
                        sm.El1RT = gnr.StationFlow / (decimal)SRTTemp;
                    }
                    else if (vm.LTp == "4.1")
                    {
                        sm.El1RT = (gnr.StationFlow / (decimal)SRTTemp) - 1;
                    }
                    else
                    {
                        sm.El1RT = 0;
                    }

                    //Pl
                    sm.PlRT = sm.PltRT *
                       ((1 + ((sm.NnRT - 1) * sm.gs)) / (sm.GfRT + (sm.GuRT / sm.El1RT) + 4.24M));

                    //fmin
                    sm.FminRT = /*vm.LGRT == "Z" ? 0 :*/ (2 * (1 + sm.PlRT)) / sm.gs;

                    //fm
                    sm.FmRT = /*vm.LGRT == "Z" ? 0 :*/
                        (sm.GfRT / sm.gs)
                        + (sm.GuRT / sm.gs)
                        * (1 / (1 + (sm.PlRT * (sm.El1RT - 1))));

                    //last
                    sm.fLTRT = /*vm.LGRT == "Z" ? 0 :*/ (sm.FmRT + (0.91M * (sm.NnRT - 1))) / sm.NnRT;

                }
                //2nd calculate --sheet7
                else if (vm.LTp == "2.2" || vm.LTp == "4.2")
                {
               // lt th rt
                    sm.VolcRT = (sm.v0RT * sm.Cs) / 3600;

                    //gf
                    double ltc = Math.Pow(Convert.ToDouble(sm.LTCRT), 0.629D);
                    sm.GfRT = (decimal)((Convert.ToDouble(sm.Gs) * (Math.Exp((-0.860 * ltc))))
                            - Convert.ToDouble(sm.tL));

                    //gq
                    sm.GqRT = (decimal)((0.4943D * Math.Pow((double)sm.VolcRT, 0.762D) * (Math.Pow((double)sm.Qr0RT, 1.061))) - (double)sm.tL);
               

                    //gu is same both
                    if (sm.GqRT >= sm.GfRT)
                    {
                        sm.GuRT = sm.gs - sm.GqRT;
                    }
                    else
                    {
                        sm.GuRT = sm.gs - sm.GfRT;
                    }

                    //n
                    decimal tempn = (sm.GqRT - sm.GfRT) / 2;
                    sm.nnRT = tempn > 0 ? tempn : 0;

                    //PRT0
                    sm.PTH0RT = 1 - sm.Plt0RT;

                    //Elt
                    //*******************************/

                    //Elt
                    double temptc = 0;
                    if (vm.LTp == "2.2")
                    {
                        temptc = 2.5D;
                    }
                    else if (vm.LTp == "4.2")
                    {
                        temptc = 4.5D;
                    }
                    else
                    {
                        temptc = 0;
                    }

                    double voe = (double)(sm.v0RT / sm.fLUoRT);
                    double expFactorRT = Math.Exp(((double)-voe * temptc) / 3600);
                    double SRTTemp = (voe * expFactorRT) / (1 - expFactorRT);

                    if (vm.LTp == "2.2")
                    {
                        sm.El1RT = gnr.StationFlow / (decimal)SRTTemp;
                    }
                    else if (vm.LTp == "4.2")
                    {
                        sm.El1RT = (gnr.StationFlow / (decimal)SRTTemp) - 1;
                    }
                    else
                    {
                        sm.El1RT = 0;
                    }
                    //El2
                    double tempEl2 = (1 - Math.Pow((double)sm.PTH0RT, (double)sm.nnRT)) / (double)sm.Plt0RT;
                    sm.El2RT = tempEl2 > 1 ? (decimal)tempEl2 : 1;


                    //Pl
                    sm.PlRT = sm.PltRT *
                       ((1 + ((sm.NnRT - 1) * sm.gs)) / (sm.GfRT + (sm.GuRT / sm.El1RT) + 4.24M));


                    //fmin
                    sm.FminRT = (2 * (1 + sm.PlRT)) / sm.gs;

                    //gdiff
                    decimal temgf = sm.GqRT - sm.GfRT;
                    sm.gdiffRT = temgf > 0 ? temgf : 0;


                    //last flt = fm
                    decimal fEl1FactorRT = 1 + (sm.PlRT * (sm.El1RT - 1));
                    decimal fEl2FactorRT = 1 + (sm.PlRT * (sm.El2RT - 1));

                    sm.fLTRT = (sm.GfRT / sm.gs)
                             + ((sm.GuRT / sm.gs) / fEl1FactorRT)
                             + ((sm.gdiffRT / sm.gs) / fEl2FactorRT);

                    //flt = fm
                    sm.FmRT = sm.fLTRT;

                }

            }
            else { }

            return sm;
        }

        private Phsr CalculateCapacity( /* -> phase */ List<Signr> sig12, /* --> leg */Volm vm, Oppsr opr, int phase)
        {
            //assign
            Phsr ph = new Phsr()
            {
                PH = phase.ToString(),
                Cs = opr.Cs,
            };

            List<Capa> capalist = new List<Capa>();

            //loop phase
            int i = 0;
            foreach (Signr sig in sig12)
            {
                //Volm vm = vm12[i];
                Capa cp = new Capa()
                {
                    AP = sig.Leg,
                    TFD = sig.Tfd,
                };

                //adjust flow rate
                switch (cp.TFD)
                {
                    //Through
                    case "1":

                        //REPLACE TFD
                        cp.TFD = "Through";
                        cp.v = vm.vpTH;
                        cp.s = vm.sTH;

                        break;

                    //Right
                    case "2":

                        //REPLACE TFD
                        cp.TFD = "Right";
                        cp.v = vm.vpRT;
                        cp.s = vm.sRT;
                        break;

                    //Left
                    case "3":

                        //REPLACE TFD
                        cp.TFD = "Left";
                        cp.v = vm.vpLT;
                        cp.s = vm.sLT;

                        break;

                    //Through + Right
                    case "4":

                        //REPLACE TFD
                        cp.TFD = "Through + Right";
                        cp.v = vm.vpTH + vm.vpRT;
                        cp.s = vm.sTH + vm.sRT;
                        break;

                    //Left + Through
                    case "5":

                        //REPLACE TFD
                        cp.TFD = "Left + Through";
                        cp.v = vm.vpTH + vm.vpLT;
                        cp.s = vm.sTH + vm.sLT;
                        break;

                    //Left + Right
                    case "6":

                        //REPLACE TFD
                        cp.TFD = "Left + Right";
                        cp.v = vm.vpTH;
                        cp.s = vm.sTH;
                        break;

                    //Left + Right
                    case "7":

                        //REPLACE TFD
                        cp.TFD = "Left + Through + Right";
                        cp.v = vm.vpTH + vm.vpLT + vm.vpRT;
                        cp.s = vm.sTH + vm.sLT + vm.sRT;

                        break;

                    default:
                        cp.TFD = "-";
                        //0
                        break;

                }


                //los time with phase
                if ((ph.PH == "3" || ph.PH == "4") && cp.TFD == "-")
                {
                    /* if(cp.TFD == "-")
                     {     
                         //lost time
                         //cp.tL = 0;
                     }
                     else
                     {
                         //lost time
                         cp.tL = Convert.ToDecimal(vm.l1);

                         cp.gs = sig.TimeG;

                         //green ratio
                         cp.gC = cp.gs / opr.Cs;

                         //Lane group capacity
                         cp.c = cp.s * cp.gC;

                         //v/c ratio
                         cp.X = cp.v / cp.c;

                         //flow ratio
                         cp.vs = cp.v / cp.s;
                     }*/
                }
                else
                {
                    //lost time
                    cp.tL = Convert.ToDecimal(vm.l1);

                    cp.gs = sig.TimeG;

                    //green ratio
                    cp.gC = cp.gs / opr.Cs;

                    //Lane group capacity by phase
                    cp.c = cp.s * cp.gC;

                    //v/c ratio
                    cp.X = cp.v / cp.c;

                    //flow ratio
                    cp.vs = cp.v / cp.s;

                }

                capalist.Add(cp);

                ph.capalist = capalist;
                i++;
            }

            //Critical Flow ratio, v/s
            decimal vs0 = ph.capalist[0].vs.GetValueOrDefault();
            //decimal vs1 = ph.capalist[1].vs.GetValueOrDefault();

            ph.Cvs = vs0; // > vs1 ? vs0 : vs1;

            //ls 1st
            ph.Ls = capalist[0].tL;

            return ph;
        }


        private Loss CalculateLoss(Volm v, Phsr p, Oppsr o)
        {
            //assign
            Loss ls = new Loss()
            {
                vLT = v.VvLT,
                vTH = v.VvTH,
                vRT = v.VvRT,
            };

            decimal gs = p.capalist.Sum(g => g.gs);

            //lane group capacity by leg
            ls.cLT = v.sLT * (gs / o.Cs);
            ls.cTH = v.sTH * (gs / o.Cs);
            ls.cRT = v.sRT * (gs / o.Cs);

            //v/c ratio,
            ls.XLT = ls.cLT == 0 ? 0 : ls.vLT / ls.cLT;
            ls.XTH = ls.cTH == 0 ? 0 : ls.vTH / ls.cTH;
            ls.XRT = ls.cRT == 0 ? 0 : ls.vRT / ls.cRT;

            //total green ratio
            ls.gC = gs / o.Cs;

            //d1
            decimal fctFactor = 0.5M * o.Cs * (decimal)(Math.Pow((double)(1 - ls.gC), 2D));
            if (ls.vLT != 0)
            {
                decimal xLTFactor = ls.XLT < 1 ? ls.XLT : 1;
                ls.d1LT = fctFactor / (1 - (xLTFactor * ls.gC));
            }
            else
            {
                ls.d1LT = 0;
            }

            if (ls.vTH != 0)
            {
                decimal xTHFactor = ls.XTH < 1 ? ls.XTH : 1;
                ls.d1TH = fctFactor / (1 - (xTHFactor * ls.gC));
            }
            else
            {
                ls.d1TH = 0;
            }


            if (ls.vRT != 0)
            {
                decimal xRTFactor = ls.XRT < 1 ? ls.XRT : 1;
                ls.d1RT = fctFactor / (1 - (xRTFactor * ls.gC));
            }
            else
            {
                ls.d1RT = 0;
            }

            //incremental delay calibration , k
            ls.k = 0.5M;

            //d2
            decimal T = v.T;
    
            if(ls.cLT != 0)
            { 
                decimal sqrtFactorLT = ((decimal)Math.Pow((double)(ls.XLT - 1), 2)) + ((8M * ls.k * 1.0M * ls.XLT) / (ls.cLT * T));
                ls.d2LT = 900M * T * ((ls.XLT - 1M) + (decimal)Math.Sqrt((double)sqrtFactorLT));
            }
            else
            {
                ls.d2LT = 0;
            }

            if (ls.cTH != 0)
            {

                decimal sqrtFactorTH = ((decimal)Math.Pow((double)(ls.XTH - 1), 2)) + ((8M * ls.k * 1.0M * ls.XTH) / (ls.cTH * T));
                ls.d2TH = 900M * T * ((ls.XTH - 1M) + (decimal)Math.Sqrt((double)sqrtFactorTH));
            }
            else
            {
                ls.d2TH = 0;
            }

            if (ls.cRT != 0)
            {
                decimal sqrtFactorRT = ((decimal)Math.Pow((double)(ls.XRT - 1), 2)) + ((8M * ls.k * 1.0M * ls.XRT) / (ls.cRT * T));
                ls.d2RT = 900M * T * ((ls.XRT - 1M) + (decimal)Math.Sqrt((double)sqrtFactorRT));   
            }
            else
            {
                ls.d2RT = 0;
            }

            //d3
            ls.d3 = 0;

            //d ALL
            ls.dLT = ls.vLT != 0 ? ls.d1LT + ls.d2LT + ls.d3 : 0;
            ls.dTH = ls.vTH != 0 ? ls.d1TH + ls.d2TH + ls.d3 : 0;
            ls.dRT = ls.vRT != 0 ? ls.d1RT + ls.d2RT + ls.d3 : 0;

            //LOS by lane group
            if (ls.XLT > 1)
            {
                ls.LOSLT = ls.LOSTH = ls.LOSRT = "F";

            }
            else
            {
                //LT
                if (ls.dLT == 0)
                {
                    ls.LOSLT = "-";
                }
                else if (ls.dLT <= 10)
                {
                    ls.LOSLT = "A";
                }
                else if (ls.dLT > 10 && ls.dLT <= 20)
                {
                    ls.LOSLT = "B";
                }
                else if (ls.dLT > 20 && ls.dLT <= 35)
                {
                    ls.LOSLT = "C";
                }
                else if (ls.dLT > 35 && ls.dLT <= 55)
                {
                    ls.LOSLT = "D";
                }
                else if (ls.dLT > 55 && ls.dLT <= 80)
                {
                    ls.LOSLT = "E";
                }
                else
                {
                    ls.LOSLT = "F";
                }


                //TH
                if (ls.dTH == 0)
                {
                    ls.LOSTH = "-";
                }
                else if (ls.dTH <= 10)
                {
                    ls.LOSTH = "A";
                }
                else if (ls.dTH > 10 && ls.dTH <= 20)
                {
                    ls.LOSTH = "B";
                }
                else if (ls.dTH > 20 && ls.dTH <= 35)
                {
                    ls.LOSTH = "C";
                }
                else if (ls.dTH > 35 && ls.dTH <= 55)
                {
                    ls.LOSTH = "D";
                }
                else if (ls.dTH > 55 && ls.dTH <= 80)
                {
                    ls.LOSTH = "E";
                }
                else
                {
                    ls.LOSTH = "F";
                }

                //RT
                if (ls.dRT == 0)
                {
                    ls.LOSRT = "-";
                }
                else if (ls.dRT <= 10)
                {
                    ls.LOSRT = "A";
                }
                else if (ls.dRT > 10 && ls.dRT <= 20)
                {
                    ls.LOSRT = "B";
                }
                else if (ls.dRT > 20 && ls.dRT <= 35)
                {
                    ls.LOSRT = "C";
                }
                else if (ls.dRT > 35 && ls.dRT <= 55)
                {
                    ls.LOSRT = "D";
                }
                else if (ls.dRT > 55 && ls.dRT <= 80)
                {
                    ls.LOSRT = "E";
                }
                else
                {
                    ls.LOSRT = "F";
                }
            }

            //Approach flow rate,
            ls.vA = ls.vLT + ls.vTH + ls.vRT;

           //delay by approach,
            ls.dA = ((ls.dLT * ls.vLT) + (ls.dTH * ls.vTH) + (ls.dRT * ls.vRT)) / (ls.vLT + ls.vTH + ls.vRT);

            decimal cA = ls.cLT + ls.cTH + ls.cRT;

            if ((ls.vA / cA) > 1)
            {
                ls.LOSSA = "F";
            }
            else
            {
                if (ls.dA <= 10)
                {
                    ls.LOSSA = "A";
                }
                else if (ls.dA > 10 && ls.dA <= 20)
                {
                    ls.LOSSA = "B";
                }
                else if (ls.dA > 20 && ls.dA <= 35)
                {
                    ls.LOSSA = "C";
                }
                else if (ls.dA > 35 && ls.dA <= 55)
                {
                    ls.LOSSA = "D";
                }
                else if (ls.dA > 55 && ls.dA <= 80)
                {
                    ls.LOSSA = "E";
                }
                else
                {
                    ls.LOSSA = "F";
                }

            }

            return ls;
        }
    }
}
