using AgriLogBackend.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;



namespace AgriLogBackend.Controllers
{
    [Authorize]
    public class EquipmentController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();


        //====================================Get all equipment============================================
        [HttpGet]
        [Route("api/Equipment/{farmID}")]
        public IHttpActionResult GetEquipment(int farmID)
        {
            var queryEquipment = from eq in db.Equipments
                                 
                                 join Sec in db.Sections on eq.Section_ID equals Sec.Section_ID
                                 where eq.Farm_ID == farmID
                                 select new
                                 {
                                     Equipment_ID = eq.Equipment_ID,
                                     Equipment_Type_Description = eq.Equipment_Type.Equipment_Type_Description,
                                     Equipment_Description = eq.Equipment_Description,
                                     Infrastructure_Name = eq.Infrastructure.Infrastructure_Name,
                                     Section_Name = Sec.Section_Name, 
                                     Equipment_Condition = eq.Equipment_Condition,
                                     Equipment_Cost = eq.Equipment_Cost,
                                     Is_Active = eq.Is_Active
                                 };

            List<dynamic> equipment = new List<dynamic>();
            try
            {
                equipment = queryEquipment.ToList<dynamic>(); // << convert to List
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
           
            }

            if (equipment.Count() > 0) //<<< Check if any equipment found
            {


                return Content(HttpStatusCode.OK, equipment);   // <<< return data
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified farm has no equipment!"); //<< return if nothing found
            }
        }

        //====================================Get specific equipment details===============================
        [HttpGet]
        [Route("api/EquipmentDetails/{id}")]
        public IHttpActionResult GetEquipmentDetails(int id)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                var query = from equipment in db.Equipments
                            where equipment.Equipment_ID == id
                            select new
                            {
                                Equipment_ID = equipment.Equipment_ID,
                                Equipment_Type_ID = equipment.Equipment_Type_ID,
                                Infrastructure_ID = equipment.Infrastructure_ID,
                                Section_ID = equipment.Section_ID,
                                Equipment_Description = equipment.Equipment_Description,
                                Equipment_Cost = equipment.Equipment_Cost,
                                Equipment_Condition = equipment.Equipment_Condition,
                                Is_Active = equipment.Is_Active
                            };
                returnOBJ = query.ToList().FirstOrDefault(); // << find equipment
            }
            catch (Exception)
            {

                return Content(HttpStatusCode.BadRequest, "Null entry error:"); // Return empty request error
            }

            if (returnOBJ != null)
            {

                return Content(HttpStatusCode.OK, returnOBJ);  // <<< Return equipment object
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "No Equipment found with the specified ID");  // <<< Nothing found error
            }
        }


        //========================================Add new Equipment========================================
        [HttpPost]
        [Route("api/Equipment/Add")]
        public IHttpActionResult AddEquipment(Equipment newEquipment)
        {
            if (newEquipment != null)
            {
                try
                {
                    /*var query = from sec in db.Sections
                                join infra in db.Infrastructures on sec.Section_ID equals infra.Section_ID
                                where newEquipment.Infrastructure_ID == newEquipment.Infrastructure_ID
                                select new
                                {
                                    Section_ID = sec.Section_ID
                                };
                    newEquipment.Section_ID = query.FirstOrDefault().Section_ID;*/
                    newEquipment.Is_Active = "true";
                    db.Equipments.Add(newEquipment);   // <<< try to add new equipment
                    db.SaveChanges();   // <<< Save new changes

                    /*
                    var auditQuery = from infra in db.Infrastructures
                                     join sec in db.Sections on infra.Section_ID equals sec.Section_ID
                                     join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                     join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                     join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID
                                     
                                     where newEquipment.Section_ID == sec.Section_ID
                                     select new
                                     {
                                         Farm_ID = farms.Farm_ID,
                                         User_ID = farmUser.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail A_Log = new Audit_Trail();
                    A_Log.Farm_ID = auditDetails.Farm_ID;
                    A_Log.User_ID = auditDetails.User_ID;
                    A_Log.Affected_ID = newEquipment.Equipment_ID;
                    A_Log.Action_DateTime = DateTime.Now;
                    A_Log.User_Action = "Added new equipment";
                    db.Audit_Trail.Add(A_Log);
                    db.SaveChanges();
                    */
                }
                catch (Exception e)
                {

                    return Content(HttpStatusCode.BadRequest, "POST failed");  // <<< changes saved failed
                }

                return Content(HttpStatusCode.OK, "1 row affected");   // <<<< return success message
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Post item cannot be empty");   //<<< empty request error
            }


        }


        //=========================================+Edit equipment========================================
        [HttpPut]
        [Route("api/Equipment/put/{id}")]
        public IHttpActionResult PutEquipment(int id, Equipment putEquipment)
        {
            try
            {
                var toPut = db.Equipments.Where(x => x.Equipment_ID == id).FirstOrDefault();

                toPut.Equipment_Type_ID = putEquipment.Equipment_Type_ID; //<< re assign all values
                toPut.Farm_ID = putEquipment.Farm_ID;
                toPut.Infrastructure_ID = putEquipment.Infrastructure_ID;
                toPut.Is_Active = putEquipment.Is_Active;
                toPut.Equipment_Description = putEquipment.Equipment_Description;
                toPut.Equipment_Condition = putEquipment.Equipment_Condition;
                toPut.Equipment_Cost = putEquipment.Equipment_Cost;

                db.SaveChanges();


                var auditQuery = from infra in db.Infrastructures
                                 join sec in db.Sections on infra.Section_ID equals sec.Section_ID
                                 join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                 join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                 join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                 where toPut.Section_ID == sec.Section_ID
                                 select new
                                 {
                                     Farm_ID = farms.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = toPut.Equipment_ID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Updated equipment details";
                db.Audit_Trail.Add(A_Log);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Edit failed");  // <<< Database save failed
            
            }

            return Content(HttpStatusCode.OK, "1 row affected");  // <<< success
        }

        //=========================================Delete Equipment=======================================
        [HttpDelete]
        [Route("api/Equipment/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var equipment = db.Equipments.Where(x => x.Equipment_ID == id).FirstOrDefault(); // << find equipment

                db.Equipments.Remove(equipment);
                db.SaveChangesAsync();

                var auditQuery = from infra in db.Infrastructures
                                 join sec in db.Sections on infra.Section_ID equals sec.Section_ID
                                 join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                 join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                 join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                 where equipment.Section_ID == sec.Section_ID
                                 select new
                                 {
                                     Farm_ID = farms.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = equipment.Equipment_ID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Updated equipment details";
                db.Audit_Trail.Add(A_Log);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed"); // <<< database delete failed
               
            }
            return Content(HttpStatusCode.OK, "1 row affected"); // <<< success
        }





        //=======================================Types==================================================

        //============================Get all equipment types===========================================
        [HttpGet]
        [Route("api/EquipmentTypes/{farmID}")]
        public IHttpActionResult GetEquipmentType(int farmID)
        {

            List<Equipment_Type> equipment_types = new List<Equipment_Type>();

            try
            {
                equipment_types = db.Equipment_Type.Where(x => x.Farm_ID == farmID).ToList(); // << find equipment types
            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.BadRequest, "Error");
            }

            if (equipment_types.Count() > 0) // << results length to check if empty
            {
               

                List<dynamic> addList = new List<dynamic>(); // <<< Return List
                foreach (var temp in equipment_types)  // <<< populate list
                {
                    dynamic addtoList = new ExpandoObject(); // <<< initiate objexct for each list entry
                    addtoList.Equipment_Type_ID = temp.Equipment_Type_ID;
                    addtoList.Equipment_Type_Description = temp.Equipment_Type_Description;
                    addtoList.Farm_ID = temp.Farm_ID;

                    addList.Add(addtoList);

                }




                return Content(HttpStatusCode.OK, addList);  // <<< success and return object
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified farm has no equipment types!");   // <<< no types found error
            }
        }

        //================================Get specific equipment type===================================
        [HttpGet]
        [Route("api/EquipmentTypeDetails/{id}")]
        public IHttpActionResult GetEquipmentTypeDetails(int id)
        {

            dynamic returnOBJ = new ExpandoObject();

            try
            {
                var query = from type in db.Equipment_Type
                            where type.Equipment_Type_ID == id
                            select new
                            {
                                Equipment_Type_ID = type.Equipment_Type_ID,
                                Equipment_Type_Description = type.Equipment_Type_Description,
                                Farm_ID = type.Farm_ID
                            };
                returnOBJ = query.ToList().FirstOrDefault(); // << find equipment types
            }
            catch (Exception)
            {

                return Content(HttpStatusCode.BadRequest, "Null entry error:");
            }

            if (returnOBJ != null)
            {

                return Content(HttpStatusCode.OK, returnOBJ); // <<< success and return object
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "No Equipment Type found with the specifieed ID"); // <<< no types found error
            }
        }


        //=====================================Add new equipment type==================================
        [HttpPost]
        [Route("api/EquipmentType/Add/{farmID}")]
        public IHttpActionResult AddEquipmentType(int farmID,HttpRequestMessage typeDesc)
        {

            if (typeDesc != null)
            {
                try
                {
                    Equipment_Type newType = new Equipment_Type();
                    newType.Equipment_Type_Description =  typeDesc.Content.ReadAsStringAsync().Result;
                    newType.Farm_ID = farmID;
                    db.Equipment_Type.Add(newType);
                    db.SaveChanges();
                }
                catch (Exception)
                {

                    return Content(HttpStatusCode.BadRequest, "POST failed");  // <<< database save failed
                }

                return Content(HttpStatusCode.OK, "1 row affected"); // <<< success
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Post item cannot be empty"); // <<< empty object request error
            }
        }


        //=====================================Edit equipment=========================================
        [HttpPut]
        [Route("api/EquipmentType/put/{id}")]
        public IHttpActionResult PutEquipmentType(int id, HttpRequestMessage typeDesc)
        {

            try
            {
                Equipment_Type editable = db.Equipment_Type.Where(x => x.Equipment_Type_ID == id).FirstOrDefault(); // << find equipment types

                editable.Equipment_Type_Description = typeDesc.Content.ReadAsStringAsync().Result; // <<< Edit data

                db.SaveChangesAsync();  // <<< save data
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Edit failed");  // <<< database save failed
             
            }

            return Content(HttpStatusCode.OK, "1 row affected"); // <<< success
        }



        [HttpGet]
        [Route("api/Equipment/getInfras/{farmID}")]
        public IHttpActionResult getInfrastructures(int farmID)
        {
            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from infras in db.Infrastructures
                            join sections in db.Sections on infras.Section_ID equals sections.Section_ID
                            join farm in db.Farms on sections.Farm_ID equals farm.Farm_ID
                            where farm.Farm_ID == farmID
                            select new { Infrastructure_ID = infras.Infrastructure_ID, Infrastructure_Name = infras.Infrastructure_Name };
                toreturn = query.ToList<dynamic>();
            }
            catch (Exception)
            {

                return Content(HttpStatusCode.BadRequest, "Db error");
            }

            if (toreturn.Count() > 0)
            {
                return Content(HttpStatusCode.OK, toreturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "No types found");
            }


        }


    }
}
