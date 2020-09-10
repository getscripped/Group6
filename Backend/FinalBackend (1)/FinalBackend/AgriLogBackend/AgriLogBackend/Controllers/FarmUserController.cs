using System;
using System.Collections.Generic;
using System.Dynamic;
using AgriLogBackend.Models;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using Microsoft.Ajax.Utilities;
using System.Net;

namespace CelineAgriLog.Controllers
{
    public class FarmUserController : ApiController
    {
        AgriLogDBEntities db = new AgriLogDBEntities();

        //==================== get specific farm user details ======================
        [HttpGet]
        [Route("api/FarmUserDetails/{id}")]
        public IHttpActionResult getFarmUser(int id)
        {
            try
            {
                var FarmUser = from farmUser in db.Farm_User
                               join user in db.Users on farmUser.User_ID equals user.User_ID
                               where farmUser.User_ID == id
                               select new
                               {
                                   Farm_User_ID = farmUser.Farm_User_ID,
                                   Farm_User_Name = farmUser.Farm_User_Name,
                                   Farm_User_Surname = farmUser.Farm_User_Surname,
                                   Farm_User_DOB = farmUser.Farm_User_DOB,
                                   Farm_User_Phone_Number = farmUser.Farm_User_Phone_Number,
                                   Farm_User_Address = farmUser.Farm_User_Address,
                                   Farm_User_Image = farmUser.Farm_User_Image,
                                   Farm_User_User_Position = farmUser.Farm_User_User_Position,
                                   User_ID = farmUser.User_ID,
                                   Is_Active = farmUser.User.Is_Active,
                                   User_Email = user.User_Email

                               };

                dynamic toReturn = FarmUser.ToList<dynamic>().FirstOrDefault();
                return Content(HttpStatusCode.OK, toReturn);

            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Null entry error:"); //return empty request
            }
        }

        //===================== update farm user ====================================
        [HttpPut]
        [Route("api/FarmUser/UpdateFarmUser/{id}")]
        public IHttpActionResult UpdateFarmUser(int id, [FromBody] Farm_User updateFarmUser)
        {

            try
            {
                Farm_User temp = db.Farm_User.Where(x => x.User_ID == id).FirstOrDefault(); //find skill
                temp.Farm_User_Name = updateFarmUser.Farm_User_Name;
                temp.Farm_User_Surname = updateFarmUser.Farm_User_Surname;
                temp.Farm_User_DOB = updateFarmUser.Farm_User_DOB;
                temp.Farm_User_Phone_Number = updateFarmUser.Farm_User_Phone_Number;
                temp.Farm_User_Image = updateFarmUser.Farm_User_Image;
                temp.Farm_User_Address = updateFarmUser.Farm_User_Address;

                db.SaveChanges();



                Audit_Trail auditLog = new Audit_Trail();
                auditLog.Farm_ID = null;
                auditLog.User_ID = updateFarmUser.User_ID;
                auditLog.Affected_ID = Convert.ToInt32(updateFarmUser.Farm_User_ID);
                auditLog.Action_DateTime = DateTime.Now;
                auditLog.User_Action = "Updated farm user profile";
                db.Audit_Trail.Add(auditLog);
                db.SaveChanges();

            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Update failed"); //update failed

            }
            return Content(HttpStatusCode.OK, "1 row affected");
            //success

        }

        //========================= remove farm user -- update isActive status =================
        [HttpPut]
        [Route("api/FarmUser/RemoveFarmUser/{id}")]
        public IHttpActionResult RemoveFarmUser(int id, Farm_User updateFarmUser)
        {
            try
            {
                Farm_User farmUser = db.Farm_User.Where(x => x.User_ID == id).FirstOrDefault(); //find staff
                farmUser.User.Is_Active = "false";
                db.SaveChanges();

                Audit_Trail auditLog = new Audit_Trail();
                auditLog.Farm_ID = null;
                auditLog.User_ID = updateFarmUser.User_ID;
                auditLog.Affected_ID = Convert.ToInt32(updateFarmUser.Farm_User_ID);
                auditLog.Action_DateTime = DateTime.Now;
                auditLog.User_Action = "Removed farm user profile";
                db.Audit_Trail.Add(auditLog);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed");
            }
            return Content(HttpStatusCode.OK, "row affected"); //success

        }

    }
}
