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
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

using AgriLogBackend.Models;

namespace AgriLogBackend.Controllers
{
    public class FarmsController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();

        //================================================getFarm===============================================
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/Farm/{UserID}")]
        [HttpGet]
        public IHttpActionResult getFarm(int UserID)
        {
            var query = from fm in db.Farms
                        join prvnce in db.Provinces on fm.Province_ID equals prvnce.Province_ID
                        join types in db.Farm_Type on fm.Farm_Type_ID equals types.Farm_Type_ID
                        join IP in db.Insurance_Provider
                        on fm.IP_ID equals IP.IP_ID
                        join userfarm in db.Farm_User_User_Position on fm.Farm_ID equals userfarm.Farm_ID
                        join farmuser in db.Farm_User on userfarm.Farm_User_ID equals farmuser.Farm_User_ID
                        join user in db.Users on farmuser.User_ID equals user.User_ID
                        where user.User_ID == UserID
                        select new
                        {
                            Farm_ID = fm.Farm_ID,
                            Farm_Name = fm.Farm_Name,
                            Farm_Size = fm.Farm_Size,
                            Farm_Email = fm.Farm_Email,
                            Farm_ContactNumber = fm.Farm_ContactNumber,
                            Farm_Address = fm.Farm_Address,
                            Farm_Type_Description = types.Farm_Type_Description,
                            Province_Description = prvnce.Province_Description,
                            Is_Active = fm.Is_Active
                        };


            List<dynamic> farm = new List<dynamic>();
            try
            {
                farm = query.ToList<dynamic>();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
                
            }
            if (farm.Count > 0)
            {
                dynamic toReturn = new ExpandoObject();
                toReturn.farmList = farm;

                return Content(HttpStatusCode.OK, toReturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified user has no farm");
            }
        }

        //======================================================getfarmsforapp===============================================================
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/Farm")]
        [HttpGet]
        public IHttpActionResult getFarm()
        {
            var query = from fm in db.Farms
                        join prvnce in db.Provinces on fm.Province_ID equals prvnce.Province_ID
                        join types in db.Farm_Type on fm.Farm_Type_ID equals types.Farm_Type_ID
                        join IP in db.Insurance_Provider
                        on fm.IP_ID equals IP.IP_ID
                        join userfarm in db.Farm_User_User_Position on fm.Farm_ID equals userfarm.Farm_ID
                        join farmuser in db.Farm_User on userfarm.Farm_User_ID equals farmuser.Farm_User_ID
                        join user in db.Users on farmuser.User_ID equals user.User_ID
                        select new
                        {
                            Farm_ID = fm.Farm_ID,
                            Farm_Name = fm.Farm_Name,
                            Farm_Size = fm.Farm_Size,
                            Farm_Email = fm.Farm_Email,
                            Farm_ContactNumber = fm.Farm_ContactNumber,
                            Farm_Address = fm.Farm_Address,
                            Farm_Type_Description = types.Farm_Type_Description,
                            Province_Description = prvnce.Province_Description,
                            Is_Active = fm.Is_Active
                        };


            List<dynamic> farm = new List<dynamic>();
            try
            {
                farm = query.ToList<dynamic>();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }
            if (farm.Count > 0)
            {
                dynamic toReturn = new ExpandoObject();
                toReturn.farmList = farm;

                return Content(HttpStatusCode.OK, toReturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified user has no farm");
            }
        }


        //==========================================getSelectedFarmDetails====================================

        // GET: api/Farms/5
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/FarmDetails/{id}")]
        public IHttpActionResult getFarmDetails(int id)
        {
            dynamic toReturn = new ExpandoObject();
            try
            {
                var query = from farms in db.Farms
                            where farms.Farm_ID == id
                            select new
                            {
                                Farm_ID = farms.Farm_ID,
                                Farm_Name = farms.Farm_Name,
                                Farm_Email = farms.Farm_Email,
                                Farm_ContactNumber = farms.Farm_ContactNumber,
                                Farm_Address = farms.Farm_Address,
                                Farm_Size = farms.Farm_Size,
                                Farm_Type_ID = farms.Farm_Type_ID,
                                Province_ID = farms.Province_ID,
                                Is_Active = farms.Is_Active
                            };

                toReturn = query.ToList().FirstOrDefault();
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
                return Content(HttpStatusCode.BadRequest, "No farm was found with specified ID");
            }
        }

        //====================================EditFarm===============================================

        // PUT: api/Farms/5
        [Route("api/Farm/put/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult putFarm(int id, Farm putfarm)
        {
            Farm toUpdate = new Farm();
            try
            {
                toUpdate = db.Farms.Where(f => f.Farm_ID == id).FirstOrDefault();
                toUpdate.Farm_Name = putfarm.Farm_Name;
                toUpdate.Farm_Size = putfarm.Farm_Size;
                toUpdate.Farm_Email = putfarm.Farm_Email;
                toUpdate.Farm_ContactNumber = putfarm.Farm_ContactNumber;
                toUpdate.Farm_Address = putfarm.Farm_Address;
                toUpdate.Farm_Type_ID = putfarm.Farm_Type_ID;
                toUpdate.Province_ID = putfarm.Province_ID;
                toUpdate.Is_Active = putfarm.Is_Active;
                db.SaveChanges();

                var auditQuery = from fm in db.Farms
                                 join prvnce in db.Provinces on fm.Province_ID equals prvnce.Province_ID
                                 join types in db.Farm_Type on fm.Farm_Type_ID equals types.Farm_Type_ID
                                 join IP in db.Insurance_Provider
                                 on fm.IP_ID equals IP.IP_ID
                                 join userfarm in db.Farm_User_User_Position on fm.Farm_ID equals userfarm.Farm_ID
                                 join farmuser in db.Farm_User on userfarm.Farm_User_ID equals farmuser.Farm_User_ID
                                 join user in db.Users on farmuser.User_ID equals user.User_ID
                                 where fm.Farm_ID == id
                                 select new
                                 {
                                     Farm_ID = fm.Farm_ID,
                                     User_ID = farmuser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = id;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Updated current farm";
                db.Audit_Trail.Add(A_Log);
                db.SaveChanges();

                return Content(HttpStatusCode.OK, "1 Row Affected");
            }
            catch (Exception e)
            {
                if (!FarmExists(id))
                {

                    return Content(HttpStatusCode.BadRequest, "Farm doesnt exist");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "Edit Failed");
                }
            }
        }

        //=======================================AddFarm==================================================

        // POST: api/Farms
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("api/Farm/add/{UserID}")]
        public IHttpActionResult postFarm(int userID, Farm newfarm)
        {
            Farm_User_User_Position newEntry = new Farm_User_User_Position();
            if (newfarm != null)
            {
                try
                {
                    newfarm.IP_ID = 4;
                    db.Farms.Add(newfarm);
                    db.SaveChanges();

                    var addedFarmID = newfarm.Farm_ID;

                    var query = from fuser in db.Farm_User
                                join tUser in db.Users on fuser.User_ID equals tUser.User_ID
                                where tUser.User_ID == userID
                                select new
                                {
                                    Farm_User_ID = fuser.Farm_User_ID,
                                    User_ID = tUser.User_ID
                                };

                    var fUserID = query.FirstOrDefault().Farm_User_ID;

                    newEntry.Farm_ID = addedFarmID;
                    newEntry.Farm_User_ID = fUserID;
                    newEntry.User_Type_ID = 1;

                    db.Farm_User_User_Position.Add(newEntry);
                    db.SaveChanges();

                    var auditQuery = from fm in db.Farms
                                     join prvnce in db.Provinces on fm.Province_ID equals prvnce.Province_ID
                                     join types in db.Farm_Type on fm.Farm_Type_ID equals types.Farm_Type_ID
                                     join IP in db.Insurance_Provider
                                     on fm.IP_ID equals IP.IP_ID
                                     join userfarm in db.Farm_User_User_Position on fm.Farm_ID equals userfarm.Farm_ID
                                     join farmuser in db.Farm_User on userfarm.Farm_User_ID equals farmuser.Farm_User_ID
                                     join user in db.Users on farmuser.User_ID equals user.User_ID
                                     where farmuser.User_ID == userID
                                     select new
                                     {
                                         Farm_ID = fm.Farm_ID,
                                         User_ID = user.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail A_Log = new Audit_Trail();
                    A_Log.Farm_ID = auditDetails.Farm_ID;
                    A_Log.User_ID = auditDetails.User_ID;
                    A_Log.Affected_ID = userID;
                    A_Log.Action_DateTime = DateTime.Now;
                    A_Log.User_Action = "Added new Farm";
                    db.Audit_Trail.Add(A_Log);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    db.Farms.Remove(newfarm);
                    try
                    {
                        db.Farm_User_User_Position.Remove(newEntry);
                    }
                    catch (Exception x)
                    {

                        return Content(HttpStatusCode.BadRequest, "POST failed");
                    }

                    return Content(HttpStatusCode.BadRequest, "POST failed");
                }
                return Content(HttpStatusCode.OK, "1 Row affected");
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Post item cannot be empty");
            }
        }

        //=====================================deleteFarm=============================================================

        // DELETE: api/Farms/5

        [Route("api/Farm/delete/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult deleteFarm(int id)
        {
            try
            {
                var farm = db.Farms.Where(f => f.Farm_ID == id).FirstOrDefault();
                var userPos = db.Farm_User_User_Position.Where(x => x.Farm_ID == id).FirstOrDefault();
                db.Farm_User_User_Position.Remove(userPos);
                db.Farms.Remove(farm);
                db.SaveChanges();

                        var auditQuery = from fm in db.Farms
                                        join prvnce in db.Provinces on fm.Province_ID equals prvnce.Province_ID
                                        join types in db.Farm_Type on fm.Farm_Type_ID equals types.Farm_Type_ID
                                        join IP in db.Insurance_Provider
                                        on fm.IP_ID equals IP.IP_ID
                                        join userfarm in db.Farm_User_User_Position on fm.Farm_ID equals userfarm.Farm_ID
                                        join farmuser in db.Farm_User on userfarm.Farm_User_ID equals farmuser.Farm_User_ID
                                        join user in db.Users on farmuser.User_ID equals user.User_ID
                                        where fm.Farm_ID == id
                                        select new
                                        {
                                            Farm_ID = fm.Farm_ID,
                                            User_ID = farmuser.User_ID
                                        };
                       var auditDetails = auditQuery.ToList().FirstOrDefault();
                       Audit_Trail A_Log = new Audit_Trail();
                       A_Log.Farm_ID = auditDetails.Farm_ID;
                       A_Log.User_ID = auditDetails.User_ID;
                       A_Log.Affected_ID = id;
                       A_Log.Action_DateTime = DateTime.Now;
                       A_Log.User_Action = "Deleted Farm";
                       db.Audit_Trail.Add(A_Log);
                       db.SaveChanges();

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed");
            }
            return Content(HttpStatusCode.OK, "1 Row affected");
        }


        [Route("api/FarmTypes")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult getFarmTypes()
        {
            List<dynamic> farmtypes = new List<dynamic>();
            try
            {
                var query = from ft in db.Farm_Type
                            select new
                            {
                                Farm_Type_ID = ft.Farm_Type_ID,
                                Farm_Type_Description = ft.Farm_Type_Description
                            };
                farmtypes = query.ToList<dynamic>();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "The is no farmtype");
            }
            if (farmtypes.Count > 0)
            {
                return Content(HttpStatusCode.OK, farmtypes);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The is no farmtype");
            }

        }

        [Route("api/Provinces")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult getProvinces()
        {
            List<dynamic> provinces = new List<dynamic>();
            try
            {
                var query = from p in db.Provinces
                            select new
                            {
                                Province_ID = p.Province_ID,
                                Province_Description = p.Province_Description
                            };
                provinces = query.ToList<dynamic>();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }
            if (provinces.Count > 0)
            {
                return Content(HttpStatusCode.OK, provinces);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The is no province");
            }
        }

        [Route("api/Farm/Foreman/{FarmID}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        public IHttpActionResult getForeman(int FarmID)
        {
            var query = from f in db.Farms
                        join fuup in db.Farm_User_User_Position on f.Farm_ID equals fuup.Farm_ID
                        join fu in db.Farm_User on fuup.Farm_User_ID equals fu.Farm_User_ID
                        join u in db.Users on fu.User_ID equals u.User_ID
                        where fuup.Farm_ID == FarmID
                        select new
                        {
                            User_Email = u.User_Email
                        };
            dynamic email = new ExpandoObject();
            try
            {
                email = query.ToList().FirstOrDefault();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Null entry error: ");
            }
            if (email != null)
            {
                return Content(HttpStatusCode.OK, email);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "No foreman was found with specified ID");
            }
        }


        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/Contacts/Foreman/{FarmID}")]
        public IHttpActionResult postForeman(int FarmID, HttpRequestMessage EmailAddress)
        {
            var emails = EmailAddress.Content.ReadAsStringAsync().Result;
            var check = db.Users.Where(e => e.User_Email == emails).FirstOrDefault();
            var UserID = 0;
            if (check == null)
            {
                return Unauthorized();
            }
            else
            {
                UserID = check.User_ID;
            }

            var link = "http://35.178.156.37/api/addForeman/" + UserID.ToString() + "/" + FarmID.ToString();

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");


                mail.From = new MailAddress("agrilognotifications@gmail.com");
                mail.To.Add(emails);
                mail.Subject = "You have been added to a farm!";
                mail.Body = FarmID.ToString() + " Has added you as a foreman to their farm! Please click on the   following link to add your profile to the farm! " + link;


                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("agrilognotifications@gmail.com", "AgriLog321");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }

                var auditQuery = from fm in db.Farms
                                 join prvnce in db.Provinces on fm.Province_ID equals prvnce.Province_ID
                                 join types in db.Farm_Type on fm.Farm_Type_ID equals types.Farm_Type_ID
                                 join IP in db.Insurance_Provider
                                 on fm.IP_ID equals IP.IP_ID
                                 join userfarm in db.Farm_User_User_Position on fm.Farm_ID equals userfarm.Farm_ID
                                 join farmuser in db.Farm_User on userfarm.Farm_User_ID equals farmuser.Farm_User_ID
                                 join user in db.Users on farmuser.User_ID equals user.User_ID
                                 where fm.Farm_ID == FarmID
                                 select new
                                 {
                                     Farm_ID = fm.Farm_ID,
                                     User_ID = farmuser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = FarmID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Added new Foreman";
                db.Audit_Trail.Add(A_Log);
                db.SaveChanges();

                return Content(HttpStatusCode.OK, "Email sent");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Message failed");
            }

        }


        [Route("api/Farm/Foreman/delete/{FarmID}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult deleteForeman(int FarmID, HttpRequestMessage email)
        {
            try
            {
                var foreman = db.Farm_User_User_Position.Where(x => x.Farm_ID == FarmID && x.User_Type_ID == 2).FirstOrDefault();
                db.Farm_User_User_Position.Remove(foreman);
                db.SaveChanges();
                var auditQuery = from fm in db.Farms
                                 join prvnce in db.Provinces on fm.Province_ID equals prvnce.Province_ID
                                 join types in db.Farm_Type on fm.Farm_Type_ID equals types.Farm_Type_ID
                                 join IP in db.Insurance_Provider
                                 on fm.IP_ID equals IP.IP_ID
                                 join userfarm in db.Farm_User_User_Position on fm.Farm_ID equals userfarm.Farm_ID
                                 join farmuser in db.Farm_User on userfarm.Farm_User_ID equals farmuser.Farm_User_ID
                                 join user in db.Users on farmuser.User_ID equals user.User_ID
                                 where fm.Farm_ID == FarmID
                                 select new
                                 {
                                     Farm_ID = fm.Farm_ID,
                                     User_ID = farmuser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = FarmID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Deleted Foreman";
                db.Audit_Trail.Add(A_Log);
                db.SaveChanges();

            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed");
            }
            return Content(HttpStatusCode.OK, "1 Row affected");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FarmExists(int id)
        {
            return db.Farms.Count(e => e.Farm_ID == id) > 0;
        }

        private bool ForemanExists(int id)
        {
            return db.Users.Count(f => f.User_ID == id) > 0;
        }
    }
}