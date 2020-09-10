using AgriLogBackend.CalculationFH;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AgriLogBackend.Controllers
{
    public class FarmHealthController : ApiController
    {
        [HttpGet]
        [Route("api/Health/{farmID}")]
        public IHttpActionResult Health(int farmID)
        {
            int Vehiclescore;
            int equipmentScore;
            int faultScore;
            int maintenanceScore;
            dynamic toReturn = new ExpandoObject();
            try
            {
                VehicleCalculator vehicleCalc = new VehicleCalculator();
                EquipmentCalculator equipmentCalc = new EquipmentCalculator();

                Vehiclescore = vehicleCalc.CalcScore(farmID)*100;
                equipmentScore = equipmentCalc.CalcScore(farmID);
                faultScore = 50;
                maintenanceScore = 50;

               

                if (Vehiclescore == 0)
                {
                    Vehiclescore = 50;
                }
                int average = (Vehiclescore + equipmentScore + faultScore + maintenanceScore) / 4;
                toReturn.VehicleScore = Vehiclescore;
                toReturn.EquipmentScore = equipmentScore;
                toReturn.FaultScore = faultScore;
                toReturn.MaintnanceScore = maintenanceScore;
                toReturn.Average = average;
            }
            catch (Exception)
            {

                return Content(HttpStatusCode.BadRequest,"error");
            }
            
            return Content(HttpStatusCode.OK,toReturn);
        }


        [HttpGet]
        [Route("api/HealthHistory/{farmID}")]
        public IHttpActionResult HealthHistory(int farmID)
        {

            return Ok(); ;
        }
    }
}
