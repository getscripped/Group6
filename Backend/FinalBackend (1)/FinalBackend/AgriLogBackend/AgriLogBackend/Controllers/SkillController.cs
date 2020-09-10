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
    public class SkillController : ApiController
    {
        AgriLogDBEntities db = new AgriLogDBEntities();

        //=========================== get skills =================================
        [HttpGet]
        [Route("api/Skill/{farmID}")]
        public IHttpActionResult getSkills(int farmID)
        {
            List<dynamic> dynamicSkills = new List<dynamic>();
            foreach (Skill skill in db.Skills)
            {
                if (skill.Farm_ID == farmID)
                {
                    dynamic dynamicSkill = new ExpandoObject();
                    dynamicSkill.Skill_ID = skill.Skill_ID;
                    dynamicSkill.Skill_Description = skill.Skill_Description;
                    dynamicSkills.Add(dynamicSkill);
                }

            }
            try
            {
                return Ok(dynamicSkills);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //==================== get specific skill details ======================
        [HttpGet]
        [Route("api/SkillDetails/{id}")]
        public IHttpActionResult SkillDetails(int id)
        {
            //List<dynamic> dynamicSkills = new List<dynamic>();


            try
            {

                var Skill = from skill in db.Skills
                            where skill.Skill_ID == id
                            select new
                            {
                                Skill_ID = skill.Skill_ID,
                                Skill_Description = skill.Skill_Description
                            };

                dynamic toReturn = Skill.ToList<dynamic>().FirstOrDefault();
                return Content(HttpStatusCode.OK, toReturn);

            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Null entry error:"); //return empty request
            }
        }


        //============================= add skill =============================
        [HttpPost]
        [Route("api/Skill/AddSkill")]
        public IHttpActionResult AddSkill(Skill newSkill)
        {

            if (newSkill != null)
            {
                try
                {
                    db.Skills.Add(newSkill); //add skill
                    db.SaveChanges(); //save changes

                    var auditQuery = from skill in db.Skills
                                     join farm in db.Farms on skill.Farm_ID equals farm.Farm_ID
                                     join farmUserPosition in db.Farm_User_User_Position on farm.Farm_ID equals farmUserPosition.Farm_ID
                                     join farmUser in db.Farm_User on farmUserPosition.Farm_User_ID equals farmUser.Farm_User_ID
                                     where skill.Skill_ID == newSkill.Skill_ID
                                     select new
                                     {
                                         Farm_ID = skill.Farm_ID,
                                         User_ID = farmUser.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail auditLog = new Audit_Trail();
                    auditLog.Farm_ID = auditDetails.Farm_ID;
                    auditLog.User_ID = auditDetails.User_ID;
                    auditLog.Affected_ID = newSkill.Skill_ID;
                    auditLog.Action_DateTime = DateTime.Now;
                    auditLog.User_Action = "Added a new skill";
                    db.Audit_Trail.Add(auditLog);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.BadRequest, "POST failed"); //save failed
                }

                return Content(HttpStatusCode.OK, "1 row affected"); //success msg

            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Post item cannot be empty");
            }
        }

        //===================== update skill ====================================
        [HttpPut]
        [Route("api/Skill/UpdateSkill/{id}")]
        public IHttpActionResult UpdateSkill(int id, [FromBody] Skill updateSkill)
        {

            try
            {
                Skill temp = db.Skills.Where(x => x.Skill_ID == id).FirstOrDefault(); //find skill
                temp.Skill_Description = updateSkill.Skill_Description;
                db.SaveChanges();

                var auditQuery = from skill in db.Skills
                                 join farm in db.Farms on skill.Farm_ID equals farm.Farm_ID
                                 join farmUserPosition in db.Farm_User_User_Position on farm.Farm_ID equals farmUserPosition.Farm_ID
                                 join farmUser in db.Farm_User on farmUserPosition.Farm_User_ID equals farmUser.Farm_User_ID
                                 where skill.Skill_ID == updateSkill.Skill_ID
                                 select new
                                 {
                                     Farm_ID = skill.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail auditLog = new Audit_Trail();
                auditLog.Farm_ID = auditDetails.Farm_ID;
                auditLog.User_ID = auditDetails.User_ID;
                auditLog.Affected_ID = updateSkill.Skill_ID;
                auditLog.Action_DateTime = DateTime.Now;
                auditLog.User_Action = "Updated a skill";
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


        //========================= delete skill =================
        [HttpDelete]
        [Route("api/Skill/DeleteSkill/{id}")]
        public IHttpActionResult DeleteSkill(int id)
        {

            Skill skill = db.Skills.Where(x => x.Skill_ID == id).FirstOrDefault(); //find skill
            try
            {

                db.Skills.Remove(skill); //remove
                db.SaveChanges();

                var auditQuery = from skills in db.Skills
                                 join farm in db.Farms on skills.Farm_ID equals farm.Farm_ID
                                 join farmUserPosition in db.Farm_User_User_Position on farm.Farm_ID equals farmUserPosition.Farm_ID
                                 join farmUser in db.Farm_User on farmUserPosition.Farm_User_ID equals farmUser.Farm_User_ID
                                 where skill.Skill_ID == id
                                 select new
                                 {
                                     Farm_ID = skill.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail auditLog = new Audit_Trail();
                auditLog.Farm_ID = auditDetails.Farm_ID;
                auditLog.User_ID = auditDetails.User_ID;
                auditLog.Affected_ID = id;
                auditLog.Action_DateTime = DateTime.Now;
                auditLog.User_Action = "Deleted a skill";
                db.Audit_Trail.Add(auditLog);
                db.SaveChanges();
            }

            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed");

            }
            return Content(HttpStatusCode.OK, "1 row affected"); //success

        }

        [HttpPost]
        [Route("api/Skill/SkillExists/{id}")]
        public IHttpActionResult SkillExists(int id, HttpRequestMessage skill)
        {
            var someText = skill.Content.ReadAsStringAsync().Result;
            var ID1 = db.Skills.Where(x => x.Farm_ID == id && x.Skill_Description == someText).FirstOrDefault();
            if (ID1 != null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok();
            }
        }

    }

}
