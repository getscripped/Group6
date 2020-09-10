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
    public class StaffController : ApiController
    {
        AgriLogDBEntities db = new AgriLogDBEntities();

        //=========================== get staff members =================================
        [HttpGet]
        [Route("api/Staff/{farmID}")]
        public IHttpActionResult getStaff(int farmID)
        {

            List<dynamic> dynamicStaffMembers = new List<dynamic>();
            List<dynamic> dynamicStaffSkills = new List<dynamic>();
            dynamic newExpando = new ExpandoObject();


            foreach (Staff staff in db.Staffs)
            {
                if (staff.Farm_ID == farmID && staff.Is_Active == "true")
                {
                    dynamic dynamicStaff = new ExpandoObject();
                    dynamicStaff.Staff_ID = staff.Staff_ID;
                    dynamicStaff.Staff_Name = staff.Staff_Name;
                    dynamicStaff.Staff_Surname = staff.Staff_Surname;
                    dynamicStaff.Staff_DoB = staff.Staff_DoB;
                    dynamicStaff.Staff_Phone_Number = staff.Staff_Phone_Number;
                    dynamicStaff.Staff_Address = staff.Staff_Address;
                    dynamicStaff.Staff_Photo = staff.Staff_Photo;
                    dynamicStaff.Is_Active = staff.Is_Active;
                    dynamicStaffMembers.Add(dynamicStaff);
                    newExpando.staff = dynamicStaffMembers;

                    foreach (Staff_Skill staffSkill in db.Staff_Skill)
                    {
                        if (staff.Staff_ID == staffSkill.Staff_ID)
                        {
                            dynamic dynamicStaffSkill = new ExpandoObject();
                            foreach (Skill skill in db.Skills)
                            {
                                if (staffSkill.Skill_ID == skill.Skill_ID)
                                {

                                    dynamicStaffSkill.Staff_ID = staffSkill.Staff_ID;
                                    dynamicStaffSkill.Skill_ID = skill.Skill_ID;
                                    dynamicStaffSkill.Skill_Description = skill.Skill_Description;
                                    dynamicStaffSkills.Add(dynamicStaffSkill);
                                    newExpando.skills = dynamicStaffSkills;
                                }
                            }
                        }
                    }
                }
            }
            try
            {
                return Ok(newExpando);
                //return Ok(myObjectList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //==================== get specific skill details ======================
        [HttpGet]
        [Route("api/StaffDetails/{id}")]
        public IHttpActionResult StaffDetails(string id)
        {
            try
            {
                var Staff = from staff in db.Staffs
                            where staff.Staff_ID == id
                            select new
                            {
                                Staff_ID = staff.Staff_ID,
                                Staff_Name = staff.Staff_Name,
                                Staff_Surname = staff.Staff_Surname,
                                Staff_DoB = staff.Staff_DoB,
                                Staff_Phone_Number = staff.Staff_Phone_Number,
                                Staff_Address = staff.Staff_Address,
                                Staff_Photo = staff.Staff_Photo,
                                Is_Active = staff.Is_Active,

                            };
                var StaffSkill = from staffSkill in db.Staff_Skill
                                 where staffSkill.Staff_ID == id
                                 select new
                                 {
                                     Staff_ID = staffSkill.Staff_ID,
                                     Skill_ID = staffSkill.Skill_ID
                                 };
                var skills = from staffSkill in db.Staff_Skill
                             join
skill in db.Skills on staffSkill.Skill_ID equals skill.Skill_ID
                             where staffSkill.Staff_ID == id
                             select new
                             {
                                 Skill_ID = skill.Skill_ID,
                                 Skill_Description = skill.Skill_Description
                             };

                dynamic staffReturn = Staff.ToList<dynamic>().FirstOrDefault();
                dynamic staffSkillReturn = StaffSkill.ToList<dynamic>();
                dynamic skillReturn = skills.ToList<dynamic>();
                dynamic myObj = new ExpandoObject();
                myObj.Staff = staffReturn;
                myObj.StaffSkills = staffSkillReturn;
                myObj.Skills = skillReturn;
                return Content(HttpStatusCode.OK, myObj);

            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Null entry error:"); //return empty request
            }
        }


        //============================= add skill =============================
        [HttpPost]
        [Route("api/Staff/AddStaff")]
        public IHttpActionResult AddStaff([FromBody] CompositeObject staffSkill)
        {

            if (staffSkill != null)
            {

                try
                {
                    db.Staffs.Add(staffSkill.Staff); //add staff
                    if (staffSkill.Skill != null)
                    {
                        foreach (Skill skill in staffSkill.Skill)
                        {
                            Staff_Skill newStaffSkill = new Staff_Skill();
                            newStaffSkill.Staff_ID = staffSkill.Staff.Staff_ID;
                            newStaffSkill.Skill_ID = skill.Skill_ID;
                            newStaffSkill.StaffSkill_ID = 0;
                            db.Staff_Skill.Add(newStaffSkill);


                        }
                    }

                    db.SaveChanges(); //save changes

                    var auditQuery = from staff in db.Staffs
                                     join farm in db.Farms on staff.Farm_ID equals farm.Farm_ID
                                     join farmUserPosition in db.Farm_User_User_Position on farm.Farm_ID equals farmUserPosition.Farm_ID
                                     join farmUser in db.Farm_User on farmUserPosition.Farm_User_ID equals farmUser.Farm_User_ID
                                     where staff.Staff_ID == staffSkill.Staff.Staff_ID
                                     select new
                                     {
                                         Farm_ID = staff.Farm_ID,
                                         User_ID = farmUser.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail auditLog = new Audit_Trail();
                    auditLog.Farm_ID = auditDetails.Farm_ID;
                    auditLog.User_ID = auditDetails.User_ID;
                    auditLog.Affected_ID = Convert.ToInt64(staffSkill.Staff.Staff_ID);
                    auditLog.Action_DateTime = DateTime.Now;
                    auditLog.User_Action = "Added new staff member";
                    db.Audit_Trail.Add(auditLog);
                    db.SaveChanges();

                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.BadRequest, "POST failed"); //save failed
                }

                return Content(HttpStatusCode.OK, "rows affected"); //success msg

            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Post item cannot be empty");
            }
        }

        //===================== update skill ====================================
        [HttpPut]
        [Route("api/Staff/UpdateStaff/{id}")]
        public IHttpActionResult UpdateStaff(string id, CompositeObject updateStaff)
        {

            try
            {
                foreach (Staff_Skill staffSkill in db.Staff_Skill)
                {
                    if (staffSkill.Staff_ID == updateStaff.Staff.Staff_ID)
                    {
                        db.Staff_Skill.Remove(staffSkill);

                    }
                }
                Staff temp = db.Staffs.Where(x => x.Staff_ID == id).FirstOrDefault(); //find skill
                temp.Staff_ID = updateStaff.Staff.Staff_ID;
                temp.Staff_Name = updateStaff.Staff.Staff_Name;
                temp.Staff_Surname = updateStaff.Staff.Staff_Surname;
                temp.Staff_Phone_Number = updateStaff.Staff.Staff_Phone_Number;
                temp.Staff_DoB = updateStaff.Staff.Staff_DoB;
                temp.Staff_Address = updateStaff.Staff.Staff_Address;
                temp.Staff_Photo = updateStaff.Staff.Staff_Photo;

                if (updateStaff.Skill != null)
                {

                    foreach (Skill skill in updateStaff.Skill)
                    {
                        Staff_Skill newStaffSkill = new Staff_Skill();
                        newStaffSkill.Staff_ID = updateStaff.Staff.Staff_ID;
                        newStaffSkill.Skill_ID = skill.Skill_ID;
                        newStaffSkill.StaffSkill_ID = 0;
                        db.Staff_Skill.Add(newStaffSkill);
                    }
                }

                db.SaveChanges();

                var auditQuery = from staff in db.Staffs
                                 join farm in db.Farms on staff.Farm_ID equals farm.Farm_ID
                                 join farmUserPosition in db.Farm_User_User_Position on farm.Farm_ID equals farmUserPosition.Farm_ID
                                 join farmUser in db.Farm_User on farmUserPosition.Farm_User_ID equals farmUser.Farm_User_ID
                                 where staff.Staff_ID == updateStaff.Staff.Staff_ID
                                 select new
                                 {
                                     Farm_ID = staff.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail auditLog = new Audit_Trail();

                auditLog.Farm_ID = auditDetails.Farm_ID;
                auditLog.User_ID = auditDetails.User_ID;
                auditLog.Affected_ID = Convert.ToInt64(updateStaff.Staff.Staff_ID);
                auditLog.Action_DateTime = DateTime.Now;
                auditLog.User_Action = "Updated staff member";
                db.Audit_Trail.Add(auditLog);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Update failed"); //update failed
         
            }
            return Content(HttpStatusCode.OK, "1 row affected"); //success

        }


        //========================= remove staff -- update isActive status =================
        [HttpPut]
        [Route("api/Staff/DeleteStaff/{id}")]
        public IHttpActionResult DeleteStaff(string id, CompositeObject updateStaff)
        {
            try
            {
                Staff staff = db.Staffs.Where(x => x.Staff_ID == id).FirstOrDefault(); //find staff
                staff.Is_Active = "False";
                db.SaveChanges();

                var auditQuery = from staffs in db.Staffs
                                 join farm in db.Farms on staff.Farm_ID equals farm.Farm_ID
                                 join farmUserPosition in db.Farm_User_User_Position on farm.Farm_ID equals farmUserPosition.Farm_ID
                                 join farmUser in db.Farm_User on farmUserPosition.Farm_User_ID equals farmUser.Farm_User_ID
                                 where staffs.Staff_ID == id
                                 select new
                                 {
                                     Farm_ID = staff.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail auditLog = new Audit_Trail();
                auditLog.Farm_ID = auditDetails.Farm_ID;
                auditLog.User_ID = auditDetails.User_ID;
                auditLog.Affected_ID = Convert.ToInt64(id);
                auditLog.Action_DateTime = DateTime.Now;
                auditLog.User_Action = "Removed staff member";
                db.Audit_Trail.Add(auditLog);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed");
            }
            return Content(HttpStatusCode.OK, "1 row affected"); //success

        }

        /* [HttpPost]
         [Route("api/Staff/StaffExists/{id}")]
         public IHttpActionResult StaffExists(int id, HttpRequestMessage staff)
         {
             try
             {

                 var someText = staff.Content.ReadAsStringAsync().Result;
                 var someTextConvert = Convert.ToInt64(someText);
                 //var ID1 = db.Staffs.Where(x => x.Farm_ID == id && x.Staff_ID == someTextConvert).FirstOrDefault();
                 if (ID1 != null)
                 {
                     return Unauthorized();
                 }
                 else
                 {
                     return Ok();
                 }
             }
             catch (Exception e) {
                 return Content(HttpStatusCode.BadRequest, e);
             }

         }*/

        //------------------------------- CLOCKED -----------------------------------------------
        //=========================== get staff members =================================
        [HttpGet]
        [Route("api/Staff/CheckedIn/{farmID}")]
        public IHttpActionResult getCheckedInStaff(int farmID)
        {

            List<dynamic> dynamicStaffMembers = new List<dynamic>();
            //dynamic newExpando = new ExpandoObject();

            foreach (Clocked clocked in db.Clockeds)
            {
                foreach (Staff staff in db.Staffs)
                {
                    if (staff.Farm_ID == farmID && staff.Staff_ID == clocked.Staff_ID)
                    {
                        string clock = clocked.Clocked_In_Time.ToShortDateString();
                        if (clock == DateTime.Now.Date.ToShortDateString())
                        {
                            dynamic dynamicStaff = new ExpandoObject();
                            dynamicStaff.Staff_ID = staff.Staff_ID;
                            dynamicStaff.Staff_Name = staff.Staff_Name;
                            dynamicStaff.Staff_Surname = staff.Staff_Surname;
                            dynamicStaff.Staff_DoB = staff.Staff_DoB;
                            dynamicStaff.Staff_Phone_Number = staff.Staff_Phone_Number;
                            dynamicStaff.Staff_Address = staff.Staff_Address;
                            dynamicStaff.Staff_Photo = staff.Staff_Photo;
                            dynamicStaff.Is_Active = staff.Is_Active;
                            dynamicStaff.Clocked_In_Time = clocked.Clocked_In_Time;
                            dynamicStaff.Clocked_Out_Time = clocked.Clocked_Out_Time;
                            dynamicStaffMembers.Add(dynamicStaff);
                            //newExpando.staff = dynamicStaffMembers;
                        }
                    }
                }
            }


            try
            {
                return Ok(dynamicStaffMembers);
                //return Ok(myObjectList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //============================= clock-in =============================
        /*[HttpPost]
        [Route("api/Staff/ClockIn")]
        public IHttpActionResult clockIn(Clocked clockInStaff)
        {
            var today = DateTime.Now.Day;
            var month = DateTime.Now.Month;

            var isStaffInTable = from clocked in db.Clockeds
                                 where clocked.Staff_ID == clockInStaff.Staff_ID
                                 select new {
                                     Clocked_ID=clocked.Clocked_ID,
                                     Staff_ID =clocked.Staff_ID,
                                     Clocked_In_Time=clocked.Clocked_In_Time,
                                     Clocked_Out_Time=clocked.Clocked_Out_Time
                                 };

            var isClockedIn = from clocked in db.Clockeds
                              where clocked.Staff_ID == clockInStaff.Staff_ID &&
                              clocked.Clocked_In_Time.Day == today && 
                              clocked.Clocked_In_Time.Month == month 
                              select new {
                                  Clocked_ID = clocked.Clocked_ID,
                                  Staff_ID = clocked.Staff_ID,
                                  Clocked_In_Time = clocked.Clocked_In_Time,
                                  Clocked_Out_Time = clocked.Clocked_Out_Time
                              };

            dynamic expando = isClockedIn.ToList().FirstOrDefault();

            if (clockInStaff != null && expando == null)
            {
                try
                {
                    
                    db.Clockeds.Add(clockInStaff);
                    

                    db.SaveChanges(); //save changes
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.BadRequest, "POST failed"); //save failed
                }

                return Content(HttpStatusCode.OK, "rows affected"); //success msg

            }
            else
            {
                if (clockInStaff != null) {
                    expando.Clocked_In_Time = DateTime.Now;
                    db.SaveChanges();
                    return Content(HttpStatusCode.OK, "rows affected");
                }

                return Content(HttpStatusCode.BadRequest, "POST failed"); //save failed
            }
        }*/


        //============================= clock-in =============================
        [HttpPost]
        [Route("api/Staff/ClockIn")]
        public IHttpActionResult clockIn(Clocked clockInStaff)
        {
            string asString = clockInStaff.Clocked_In_Time.ToString("dd MMMM yyyy ");
            string a = clockInStaff.Staff_ID.ToString();
            var isClocked = db.Clockeds.Where(x => x.Clocked_ID == asString + a).FirstOrDefault();
            if (isClocked == null)
            {
                isClocked = new Clocked();
                clockInStaff.Clocked_ID = asString + a;
                db.Clockeds.Add(clockInStaff);
                db.SaveChanges();
                return Content(HttpStatusCode.OK, "rows affected"); //success msg
            }

            return Content(HttpStatusCode.BadRequest, "already clocked in");


        }

        //============================= clock-in =============================
        [HttpPut]
        [Route("api/Staff/ClockOut/{staffID}")]
        public IHttpActionResult clockOut(string staffID)
        {
            try
            {
                var today = DateTime.Now.Date.ToShortDateString();

                foreach (Clocked clocked in db.Clockeds)
                {
                    if (clocked.Staff_ID == staffID && clocked.Clocked_In_Time.ToShortDateString() == today && clocked.Clocked_Out_Time == null)
                    {
                        clocked.Clocked_Out_Time = DateTime.Now;


                    }
                }
                db.SaveChanges();
                return Content(HttpStatusCode.OK, "rows affected");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Bad request");
            }


            /*var isClocked = db.Clockeds.Where(x => x.Staff_ID == staffID && 
            x.Clocked_In_Time == today && x.Clocked_Out_Time==null).FirstOrDefault();

            if (isClocked != null)
            {
                //success msg
            }*/




        }


    }


}


public class CompositeObject
{
    public Staff Staff { get; set; }
    public List<Skill> Skill { get; set; }
}