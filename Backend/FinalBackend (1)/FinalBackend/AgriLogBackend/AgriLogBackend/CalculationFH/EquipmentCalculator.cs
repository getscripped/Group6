using AgriLogBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace AgriLogBackend.CalculationFH
{
    public class EquipmentCalculator : Calculator
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();
        public override int GetTotal(int farm_ID)
        {
            try
            {
                var query = from equipment in db.Equipments
                            where equipment.Farm_ID == farm_ID
                            select new
                            {
                                Equipment_ID = equipment.Equipment_ID,
                                Equipment_Condition = equipment.Equipment_Condition
                            };

                List<dynamic> equipmentCount = query.ToList<dynamic>();
                if(equipmentCount.Count() == 0)
                {
                    return 0;
                }

                return equipmentCount.Where(x => x.Equipment_Condition == "good").Count();
            }
            catch (Exception e)
            {

                return 0;
            }
            
        }

        public override int GetDenom(int farm_ID)
        {
            try
            {
                var query = from equipment in db.Equipments
                            where equipment.Farm_ID == farm_ID
                            select new
                            {
                                Equipment_ID = equipment.Equipment_ID,
                                Equipment_Condition = equipment.Equipment_Condition
                            };

                List<dynamic> equipmentCount = query.ToList<dynamic>();
                if (equipmentCount.Count() == 0)
                {
                    return 0;
                }
                return equipmentCount.Count();
            }
            catch (Exception e)
            {

                return 0;
            }
            
        }

        public override int CalcScore(int farmID)
        {
            return 50;
        }

    }
}