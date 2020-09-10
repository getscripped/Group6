using AgriLogBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgriLogBackend.CalculationFH
{
	public class VehicleCalculator : Calculator
        
	{
        private AgriLogDBEntities db = new AgriLogDBEntities();
        public override int GetTotal(int farm_ID)
        {
            try
            {
                var query = from vehicles in db.Vehicles
                            where vehicles.Farm_ID == farm_ID
                            select new
                            {
                                Vehicle_ID = vehicles.Vehicle_ID,
                                Vehicle_Mileage_Date_Of_Purchase = vehicles.Vehicle_Mileage_Date_Of_Purchase,
                                Vehicle_Last_Service_Mileage = vehicles.Vehicle_Last_Service_Mileage,
                                Vehicle_Service_Intervals = vehicles.Vehicle_Service_Intervals,
                                Vehicle_Mileage_Current = vehicles.Vehicle_Mileage_Current

                            };



                List<dynamic> vehicleList = new List<dynamic>();
                List<dynamic> services = new List<dynamic>();
                List<dynamic> isUpToDate = new List<dynamic>();
                int lastMileage = 0;
                int intervals = 0;
                int startMileage = 0;
                int vehicleID = 0;
                int currentMileage = 0;
                int yes = 0;
                int no = 0;
                int lastServiceMileage;
                vehicleList = query.ToList<dynamic>();
                foreach (var vehicle in vehicleList)
                {
                    lastMileage = Convert.ToInt32(vehicle.Vehicle_Last_Service_Mileage);
                    intervals = Convert.ToInt32(vehicle.Vehicle_Service_Intervals);
                    startMileage = Convert.ToInt32(vehicle.Vehicle_Mileage_Date_Of_Purchase);
                    vehicleID = vehicle.Vehicle_ID;
                    currentMileage = Convert.ToInt32(vehicle.Vehicle_Mileage_Current);
                    var serviceRecords = from serviceRecord in db.Vehicle_Service
                                         where serviceRecord.Vehicle_ID == vehicleID
                                         select new
                                         {
                                             Vehicle_Service_ID = serviceRecord.Vehicle_Service_ID,
                                             Vehicle_Service_Mileage = serviceRecord.Vehicle_Service_Mileage,
                                             Vehicle_Service_Start_Date = serviceRecord.Vehicle_Service_Start_Date
                                         };
                    services = serviceRecords.OrderByDescending(x=> x.Vehicle_Service_Start_Date).ToList<dynamic>();
                    if (services.Count() == 0)
                    {
                        int serviceGaps = currentMileage - lastMileage;

                        if (serviceGaps > intervals)
                        {
                            isUpToDate.Add(0);
                        }
                        else
                        {
                            isUpToDate.Add(1);
                        }
                    }
                    else
                    {

                        var lastService = services[0];
                        lastServiceMileage = lastService.Vehicle_Service_Mileage;

                        int serviceGap = currentMileage - lastMileage;

                        if (serviceGap > intervals)
                        {
                            isUpToDate.Add(0);
                        }
                        else
                        {
                            isUpToDate.Add(1);
                        }
                    }
                }

                if(isUpToDate.Where(x => x == 1).ToList().Count() == 0)
                {
                    return 1;
                }

                return isUpToDate.Where(x => x == 1).ToList().Count();
            }
            catch (Exception e)
            {

                return 1;
            }
            
        }

        public override int GetDenom(int farm_ID)
        {
            try
            {
                var query = from vehicles in db.Vehicles
                            where vehicles.Farm_ID == farm_ID
                            select new
                            {
                                Vehicle_ID = vehicles.Vehicle_ID,
                                Vehicle_Mileage_Date_Of_Purchase = vehicles.Vehicle_Mileage_Date_Of_Purchase,
                                Vehicle_Last_Service_Mileage = vehicles.Vehicle_Last_Service_Mileage,
                                Vehicle_Service_Intervals = vehicles.Vehicle_Service_Intervals

                            };
                List<dynamic> vehicleList = new List<dynamic>();
                vehicleList = query.ToList<dynamic>();

                if(vehicleList.Count == 0)
                {
                    return 1;
                }

                return vehicleList.Count();
            }
            catch (Exception)
            {

                return 100;
            }
            
        }

        public override int CalcScore(int farmID)
        {
            int denom = GetDenom(farmID);
            int total = GetTotal(farmID);
            if(total == 1000)
            {
                return 100;
            }
            double calc = total / denom;
            double percentage = Math.Floor(calc);


            return Convert.ToInt32(percentage);
        }

    }
}