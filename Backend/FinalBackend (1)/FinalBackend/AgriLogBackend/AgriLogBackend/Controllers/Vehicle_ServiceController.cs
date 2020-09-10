using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

using AgriLogBackend.Models;

namespace AgriLogBackend.Controllers
{
    public class Vehicle_ServiceController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();

        //=========================================getvehicleservice==========================================================

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/VehicleService/{FarmID}")]
        [HttpGet]
        public IHttpActionResult getVehicleService(int FarmID)  //<<<<< this returns all service instances on a farm
        {
            var query = from vs in db.Vehicle_Service
                        join veh in db.Vehicles on vs.Vehicle_ID equals veh.Vehicle_ID
                        join pro in db.Vehicle_Service_Provider on vs.Provider_ID equals pro.Provider_ID
                        join stat in db.Status on vs.Status_ID equals stat.Status_ID
                        join vehic in db.Vehicles on vs.Vehicle_ID equals vehic.Vehicle_ID
                        join farm in db.Farms on vehic.Farm_ID equals farm.Farm_ID
                        where farm.Farm_ID == FarmID
                        select new
                        {
                            Vehicle_Service_ID = vs.Vehicle_Service_ID,
                            Vehicle_Service_Mileage = vs.Vehicle_Service_Mileage,
                            Vehicle_Service_Start_Date = vs.Vehicle_Service_Start_Date,
                            Vehicle_Service_End_Date = vs.Vehicle_Service_End_Date,
                            Vehicle_Service_Description = vs.Vehicle_Service_Description,
                            Provider_Name = pro.Provider_Name,
                            Status_Description = stat.Status_Description,
                            Vehicle_ID = vehic.Vehicle_ID,
                            Vehicle_Service_Document = vs.Vehicle_Service_Document,
                            Provider_ID = pro.Provider_ID

                        };

            List<dynamic> vehicle_service = new List<dynamic>();
            try
            {
                vehicle_service = query.ToList<dynamic>();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }
            if (vehicle_service.Count > 0)
            {
                dynamic toReturn = new ExpandoObject();
                toReturn.vsList = vehicle_service;

                return Content(HttpStatusCode.OK, toReturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified vehicle service doesnt exist");
            }
        }

        //============================================getvehicleservicedetails=========================================

        // GET: api/Vehicle_Service/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/VehicleServiceDetails/{id}")]
        [ResponseType(typeof(Vehicle_Service))]
        public IHttpActionResult getVehicleServiceDetails(int id)//<< this takes the selected service instance and returns everything with no join, the ID's are used in the front end
        {
            dynamic toReturn = new ExpandoObject();
            try
            {
                var sql = from services in db.Vehicle_Service
                          where services.Vehicle_Service_ID == id
                          select new
                          {
                              Vehicle_Service_ID = services.Vehicle_Service_ID,
                              Vehicle_Service_Mileage = services.Vehicle_Service_Mileage,
                              Vehicle_Service_Start_Date = services.Vehicle_Service_Start_Date,
                              Vehicle_Service_End_Date = services.Vehicle_Service_End_Date,
                              Vehicle_Service_Description = services.Vehicle_Service_Description,
                              Provider_ID = services.Provider_ID,
                              Status_ID = services.Status_ID,
                              Vehicle_ID = services.Vehicle_ID,
                              Vehicle_Service_Document = services.Vehicle_Service_Document
                          };
                toReturn = sql.ToList().FirstOrDefault();
                return Content(HttpStatusCode.OK, toReturn);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Null entry error");
            }
            if (toReturn != null)
            {
                return Content(HttpStatusCode.OK, toReturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "No Vehicle Service was found with specified ID");
            }
        }

        //==========================================edtvehicleservice============================================================

        // PUT: api/Vehicle_Service/5
        [ResponseType(typeof(void))]
        [Route("api/VehicleService/put/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult putVehicleService(int id, Vehicle_Service putVehicleService)
        {
            try
            {
                Vehicle_Service vs = db.Vehicle_Service.Where(v => v.Vehicle_Service_ID == id).FirstOrDefault();
                vs.Vehicle_ID = putVehicleService.Vehicle_ID;
                vs.Status_ID = putVehicleService.Status_ID;
                vs.Vehicle_Service_Description = putVehicleService.Vehicle_Service_Description;
                vs.Vehicle_Service_Mileage = putVehicleService.Vehicle_Service_Mileage;
                vs.Vehicle_Service_Provider = putVehicleService.Vehicle_Service_Provider;
                vs.Vehicle_Service_Start_Date = putVehicleService.Vehicle_Service_Start_Date;
                vs.Vehicle_Service_End_Date = putVehicleService.Vehicle_Service_End_Date;
                vs.Vehicle_Service_ID = putVehicleService.Vehicle_Service_ID;
                vs.Vehicle_Service_Document = putVehicleService.Vehicle_Service_Document;
                db.SaveChanges();

                var auditQuery = from ves in db.Vehicle_Service
                                 join veh in db.Vehicles on ves.Vehicle_ID equals veh.Vehicle_ID
                                 join pro in db.Vehicle_Service_Provider on ves.Provider_ID equals pro.Provider_ID
                                 join stat in db.Status on ves.Status_ID equals stat.Status_ID
                                 join vehic in db.Vehicles on ves.Vehicle_ID equals vehic.Vehicle_ID
                                 join farm in db.Farms on vehic.Farm_ID equals farm.Farm_ID
                                 join fuup in db.Farm_User_User_Position on farm.Farm_ID equals fuup.Farm_ID
                                 join fu in db.Farm_User on fuup.Farm_User_ID equals fu.Farm_User_ID
                                 join u in db.Users on fu.User_ID equals u.User_ID
                                 select new
                                 {
                                     Vehicle_Service_ID = vs.Vehicle_Service_ID,
                                     User_ID = u.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Vehicle_Service_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = id;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Updated vehicle service";
                db.Audit_Trail.Add(A_Log);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Edit failed");
            }
            return Content(HttpStatusCode.OK, "1 Row affected");

        }

        //====================================================addVehicleService================================================== 

        // POST: api/Vehicle_Service
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [ResponseType(typeof(Vehicle_Service))]
        [Route("api/VehicleService/add")]
        public IHttpActionResult postVehicleService(Vehicle_Service newvehicleService)
        {

            if (newvehicleService != null)
            {
                try
                {
                    db.Vehicle_Service.Add(newvehicleService);
                    db.SaveChanges();

              /*      var auditQuery = from vs in db.Vehicle_Service
                                     join veh in db.Vehicles on vs.Vehicle_ID equals veh.Vehicle_ID
                                     join pro in db.Vehicle_Service_Provider on vs.Provider_ID equals pro.Provider_ID
                                     join stat in db.Status on vs.Status_ID equals stat.Status_ID
                                     join vehic in db.Vehicles on vs.Vehicle_ID equals vehic.Vehicle_ID
                                     join farm in db.Farms on vehic.Farm_ID equals farm.Farm_ID
                                     join fuup in db.Farm_User_User_Position on farm.Farm_ID equals fuup.Farm_ID
                                     join fu in db.Farm_User on fuup.Farm_User_ID equals fu.Farm_User_ID
                                     join u in db.Users on fu.User_ID equals u.User_ID
                                     select new
                                     {
                                         Vehicle_Service_ID = vs.Vehicle_Service_ID,
                                         User_ID = u.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail A_Log = new Audit_Trail();
                    A_Log.Farm_ID = auditDetails.Vehicle_Service_ID;
                    A_Log.User_ID = auditDetails.User_ID;
                    A_Log.Affected_ID = newvehicleService.Vehicle_Service_ID;
                    A_Log.Action_DateTime = DateTime.Now;
                    A_Log.User_Action = "Added new vehicle service";
                    db.Audit_Trail.Add(A_Log);
                    db.SaveChanges();
                    */
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.BadRequest, "POST failed");
                }
                return Content(HttpStatusCode.OK, "1 Row affected");
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Post item cannot be empty");
            }
        }

        //================================================deleteVehicleService==========================================================

        // DELETE: api/Vehicle_Service/5
        [ResponseType(typeof(Vehicle_Service))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/VehicleService/delete/{id}")]
        public IHttpActionResult deleteVehicleService(int id)
        {
            try
            {
                Vehicle_Service vsdelete = db.Vehicle_Service.Where(v => v.Vehicle_Service_ID == id).FirstOrDefault();
                db.Vehicle_Service.Remove(vsdelete);
                db.SaveChanges();

                var auditQuery = from vs in db.Vehicle_Service
                                 join veh in db.Vehicles on vs.Vehicle_ID equals veh.Vehicle_ID
                                 join pro in db.Vehicle_Service_Provider on vs.Provider_ID equals pro.Provider_ID
                                 join stat in db.Status on vs.Status_ID equals stat.Status_ID
                                 join vehic in db.Vehicles on vs.Vehicle_ID equals vehic.Vehicle_ID
                                 join farm in db.Farms on vehic.Farm_ID equals farm.Farm_ID
                                 join fuup in db.Farm_User_User_Position on farm.Farm_ID equals fuup.Farm_ID
                                 join fu in db.Farm_User on fuup.Farm_User_ID equals fu.Farm_User_ID
                                 join u in db.Users on fu.User_ID equals u.User_ID
                                 select new
                                 {
                                     Vehicle_Service_ID = vs.Vehicle_Service_ID,
                                     User_ID = u.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Vehicle_Service_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = id;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Deleted vehicle service";
                db.Audit_Trail.Add(A_Log);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed");
            }
            return Content(HttpStatusCode.OK, "1 Row affected");
        }

        [Route("api/VehicleService/ServiceRequest/{ProviderID}/{VehicleID}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult sendServiceRequest(int ProviderID, int VehicleID)
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
                mail.Body = VehicleID.ToString() + "The following vehicle needs to go in for a service:" + vehicle.Vehicle_Mileage_Current +
                   vehicle.Vehicle_Brand + vehicle.Vehicle_Registration_Number;
                ;


                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("agrilognotifications@gmail.com", " AgriLog321 ");
                SmtpServer.EnableSsl = true;


                SmtpServer.Send(mail);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Failed to send");
            }

            return Ok();
        }

        //=====Try it again quick

        [Route("api/Vehicles")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult getVehicles()
        {
            List<dynamic> vehicles = new List<dynamic>();
            try
            {
                var query = from v in db.Vehicles
                            select new
                            {
                                Vehicle_ID = v.Vehicle_ID,
                                Vehicle_Registration_Number = v.Vehicle_Registration_Number
                            };
                vehicles = query.ToList<dynamic>();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }
            if (vehicles.Count > 0)
            {
                return Content(HttpStatusCode.OK, vehicles);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The is no vehicle");
            }

        }

        [Route("api/ServiceProviders")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult getServiceProviders()
        {
            List<dynamic> VSP = new List<dynamic>();
            try
            {
                var query = from sp in db.Vehicle_Service_Provider
                            select new
                            {
                                Provider_ID = sp.Provider_ID,
                                Provider_Name = sp.Provider_Name
                            };
                VSP = query.ToList<dynamic>();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }
            if (VSP.Count > 0)
            {
                return Content(HttpStatusCode.OK, VSP);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The is no service provider");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Vehicle_ServiceExists(int id)
        {
            return db.Vehicle_Service.Count(e => e.Vehicle_Service_ID == id) > 0;
        }
    }
}