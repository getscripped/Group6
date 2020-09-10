using AgriLogBackend.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Net.Mail;
using System.Data;
using System.Data.Entity;
using System.Web.Http.Description;
using System.Windows;


namespace test_db.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class Vehicle_RepairController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();


        //====================================Get all equipment============================================
        [HttpGet]
        [Route("api/Vehicle_Repair/{farmID}")]
        public IHttpActionResult getRepairLog(int farmID)
        {
            var queryRepairLog = from rep in db.Vehicle_Repair
                                 
                                 join veh in db.Vehicles on rep.Vehicle_ID
                                     equals veh.Vehicle_ID
                                 join prov in db.Vehicle_Service_Provider on rep.Provider_ID
                                 equals prov.Provider_ID
                                 join stat in db.Status on rep.Status_ID
                                 equals stat.Status_ID
                                 where rep.Vehicle.Section.Farm_ID == farmID
                                 select new
                                 {
                                     Vehicle_Repair_ID = rep.Vehicle_Repair_ID,
                          
                                     Vehicle_Registration_Number = veh.Vehicle_Registration_Number,
                                     Vehicle_Repair_Date_Ended = rep.Vehicle_Repair_Date_Ended,
                                     Vehicle_Repair_Cost = rep.Vehicle_Repair_Cost,
                                     Vehicle_Repair_Date_Started = rep.Vehicle_Repair_Date_Started,
                                     Status_Description = stat.Status_Description,
                                     Provider_Name = prov.Provider_Name
                                 };

            List<dynamic> repairLog = new List<dynamic>();
            try
            {
                repairLog = queryRepairLog.ToList<dynamic>(); // << convert to List
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
        
            }

            if (repairLog.Count() > 0) //<<< Check if any equipment found
            {


                return Content(HttpStatusCode.OK, repairLog);   // <<< return data
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified farm has no repair logs!"); //<< return if nothing found
            }
        }

        //====================================Get specific equipment details===============================
        [HttpGet]
        [Route("api/Vehicle_RepairDetails/{id}")]
        public IHttpActionResult GetRepairLogDetails(int id)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                var queryRepairLog = from rep in db.Vehicle_Repair
                                     where rep.Vehicle_Repair_ID == id
                                     select new
                                     {
                                         Vehicle_Repair_ID = rep.Vehicle_Repair_ID,
                                         Vehicle_Repair_Document = rep.Vehicle_Repair_Document,
                                         Vehicle_ID = rep.Vehicle_ID,
                                         Vehicle_Repair_Date_Ended = rep.Vehicle_Repair_Date_Ended,
                                         Vehicle_Repair_Cost = rep.Vehicle_Repair_Cost,
                                         Vehicle_Repair_Date_Started = rep.Vehicle_Repair_Date_Started,
                                         Status_ID = rep.Status_ID,
                                         Provider_ID = rep.Provider_ID
                                     };
                returnOBJ = queryRepairLog.ToList().FirstOrDefault();
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
                return Content(HttpStatusCode.BadRequest, "No repair log found with the specified ID");  // <<< Nothing found error
            }
        }


        //========================================Add new Equipment========================================
        [HttpPost]
        [Route("api/Vehicle_Repair/Add")]
        public IHttpActionResult AddRepairLog(Vehicle_Repair newVehicle_Repair)
        {
            if (newVehicle_Repair != null)
            {
                try
                {
                    newVehicle_Repair.Status_ID = 1;
                    db.Vehicle_Repair.Add(newVehicle_Repair);   // <<< try to add new equipment
                    db.SaveChanges();   // <<< Save new changes

               /*     var auditQuery = from veh in db.Vehicles
                                     join sec in db.Sections on veh.Section_ID equals sec.Section_ID
                                     join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                     join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                     join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                     where newVehicle_Repair.Vehicle.Section_ID == sec.Section_ID
                                     select new
                                     {
                                         Farm_ID = farms.Farm_ID,
                                         User_ID = farmUser.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail A_Log = new Audit_Trail();
                    A_Log.Farm_ID = auditDetails.Farm_ID;
                    A_Log.User_ID = auditDetails.User_ID;
                    A_Log.Affected_ID = newVehicle_Repair.Vehicle_Repair_ID;
                    A_Log.Action_DateTime = DateTime.Now;
                    A_Log.User_Action = "Added new vehicle repair";
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
        [Route("api/Vehicle_Repair/put/{id}")]
        public IHttpActionResult PutRepaiLog(int id, Vehicle_Repair putVehicle_Repair)
        {
            try
            {
                Vehicle_Repair toPut = db.Vehicle_Repair.Where(x => x.Vehicle_Repair_ID == id).FirstOrDefault();

                toPut.Vehicle_Repair_Document = putVehicle_Repair.Vehicle_Repair_Document; //<< re assign all values
                toPut.Vehicle.Section.Farm_ID = putVehicle_Repair.Vehicle.Section.Farm_ID;
                toPut.Provider_ID = putVehicle_Repair.Provider_ID;
                toPut.Vehicle_ID = putVehicle_Repair.Vehicle_ID;
                toPut.Vehicle_Repair_Date_Ended = putVehicle_Repair.Vehicle_Repair_Date_Ended;
                toPut.Vehicle_Repair_Date_Started = putVehicle_Repair.Vehicle_Repair_Date_Started;
                toPut.Status_ID = putVehicle_Repair.Status_ID;

                db.SaveChanges();

                var auditQuery = from veh in db.Vehicles
                                 join sec in db.Sections on veh.Section_ID equals sec.Section_ID
                                 join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                 join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                 join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                 where putVehicle_Repair.Vehicle.Section_ID == sec.Section_ID
                                 select new
                                 {
                                     Farm_ID = farms.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = putVehicle_Repair.Vehicle_Repair_ID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Vehicle repair updated";
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
        [Route("api/Vehicle_Repair/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var repairLog = db.Vehicle_Repair.Where(x => x.Vehicle_Repair_ID == id).FirstOrDefault(); // << find equipment

                db.Vehicle_Repair.Remove(repairLog);
                db.SaveChangesAsync();

                var auditQuery = from veh in db.Vehicles
                                 join sec in db.Sections on veh.Section_ID equals sec.Section_ID
                                 join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                 join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                 join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                 where repairLog.Vehicle.Section_ID == sec.Section_ID
                                 select new
                                 {
                                     Farm_ID = farms.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = repairLog.Vehicle_Repair_ID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Vehicle repair deleted";
                db.Audit_Trail.Add(A_Log);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed"); // <<< database delete failed
  
            }
            return Content(HttpStatusCode.OK, "1 row affected"); // <<< success
        }

        [HttpGet]
        [Route("api/Vehicle_Repair/getProv/{farmID}")]
        public IHttpActionResult getProvider(int farmID)
        {
            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from prov in db.Vehicle_Service_Provider
                            select new { Provider_ID = prov.Provider_ID, Provider_Name = prov.Provider_Name };
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
                return Content(HttpStatusCode.BadRequest, "No providers found");
            }


        }

        [HttpGet]
        [Route("api/Vehicle_Repair/getRepDoc/{farmID}")]
        public IHttpActionResult getVehicleRepairDocument(int farmID)
        {
            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from invoices in db.Vehicle_Repair_Document
                            select new { VRD_ID = invoices.VRD_ID, VRD_Invoice = invoices.VRD_Invoice };
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
                return Content(HttpStatusCode.BadRequest, "No invoices found");
            }


        }

        [HttpGet]
        [Route("api/Vehicle_Repair/getVeh/{farmID}")]
        public IHttpActionResult getVehicle(int farmID)
        {
            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from vehicles in db.Vehicles
                            join sections in db.Sections on vehicles.Section_ID equals sections.Section_ID
                            join farm in db.Farms on sections.Farm_ID equals farm.Farm_ID
                            where farm.Farm_ID == farmID
                            select new { Vehicle_ID = vehicles.Vehicle_ID, Vehicle_Registration_Number = vehicles.Vehicle_Registration_Number };
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
                return Content(HttpStatusCode.BadRequest, "No vehicles found");
            }


        }

        [HttpGet]
        [Route("api/Vehicle_Repair/getStat/{farmID}")]
        public IHttpActionResult getStatus(int farmID)
        {
            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from statusses in db.Status
                            select new { Status_ID = statusses.Status_ID, Status_Description = statusses.Status_Description };
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
                return Content(HttpStatusCode.BadRequest, "No statusses found");
            }
        }

        [Route("api/Vehicle_Repair/RepairRequest/{ProviderID}/{VehicleID}")]
        // [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult sendRepairRequest(int ProviderID, int VehicleID)
        {
            try
            {
                Vehicle vehicle = db.Vehicles.Where(v => v.Vehicle_ID == VehicleID).FirstOrDefault();
                Vehicle_Service_Provider SP = db.Vehicle_Service_Provider.Where(vs => vs.Provider_ID == ProviderID).FirstOrDefault();
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");


                mail.From = new MailAddress("agrilognotifications@gmail.com");
                mail.To.Add(SP.Provider_Email);
                mail.Subject = "You have been added to a farm!";
                mail.Body = VehicleID.ToString() + "The following vehicle needs to go in for a service:" +
                   vehicle.Vehicle_Brand + vehicle.Vehicle_Registration_Number;
                ;


                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("agrilognotifications@gmail.com", " AgriLog321 ");
                SmtpServer.EnableSsl = true;


                SmtpServer.Send(mail);
                
            }
            catch (Exception ex)
            {
                
            }

            return Ok();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Vehicle_RepairExists(int id)
        {
            return db.Vehicle_Repair.Count(e => e.Vehicle_Repair_ID == id) > 0;
        }

    }

}