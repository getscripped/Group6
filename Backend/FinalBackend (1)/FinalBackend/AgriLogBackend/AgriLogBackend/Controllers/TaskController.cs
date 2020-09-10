using System;
using System.Collections.Generic;
using AgriLogBackend.Models;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using Microsoft.Ajax.Utilities;
using System.Net;
using System.Dynamic;

namespace CelineAgriLog.Controllers
{
    public class TaskController : ApiController
    {
        AgriLogDBEntities db = new AgriLogDBEntities();
        //=========================== get Tasks =================================
        [HttpGet]
        [Route("api/Task/{farmID}")]
        public IHttpActionResult getTask(int farmID)
        {

            List<dynamic> dynamicTasks = new List<dynamic>();
            List<dynamic> dynamicTaskSkills = new List<dynamic>();
            dynamic newExpando = new ExpandoObject();

            var taskFarms = from task in db.Tasks
                            join section in db.Sections on task.Section_ID equals section.Section_ID
                            where section.Farm_ID == farmID

                            select new
                            {
                                Task_ID = task.Task_ID,
                                Section_ID = task.Section_ID,
                                Section_Name = task.Section.Section_Name,
                                Infrastructure_ID = task.Infrastructure_ID,
                                Infrastructure_Name = task.Infrastructure.Infrastructure_Name,
                                Equipment_ID = task.Equipment_ID,
                                Equipment_Description = task.Equipment.Equipment_Description,
                                Task_Type_ID = task.Task_Type_ID,
                                Task_Type_Description = task.Task_Type.Task_Type_Description,
                                Importance_ID = task.Importance_ID,
                                Importance_Description = task.Importance.Importance_Description,
                                Task_Duration = task.Task_Duration,
                                Task_Description = task.Task_Description
                            };
            var taskSkills = from task in db.Tasks
                             join section in db.Sections
                             on task.Section_ID equals section.Section_ID
                             join taskSkill in db.Task_Skill
                             on task.Task_ID equals taskSkill.Task_ID
                             where section.Farm_ID == farmID
                             select new
                             {
                                 Task_ID = task.Task_ID,
                                 Skill_ID = taskSkill.Skill_ID
                             };


            try
            {
                dynamic tasksReturn = taskFarms.ToList<dynamic>();
                dynamic taskSkillsReturn = taskSkills.ToList<dynamic>();
                newExpando.Tasks = tasksReturn;
                newExpando.TaskSkills = taskSkillsReturn;
                return Content(HttpStatusCode.OK, newExpando);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //==================== get specific task details ======================
        [HttpGet]
        [Route("api/TaskDetails/{id}")]
        public IHttpActionResult TaskDetails(int id)
        {
            try
            {
                var tasks = from task in db.Tasks
                            where task.Task_ID == id

                            select new
                            {
                                Task_ID = task.Task_ID,
                                Section_ID = task.Section_ID,
                                Section_Name = task.Section.Section_Name,
                                Infrastructure_ID = task.Infrastructure_ID,
                                Infrastructure_Name = task.Infrastructure.Infrastructure_Name,
                                Equipment_ID = task.Equipment_ID,
                                Equipment_Description = task.Equipment.Equipment_Description,
                                Task_Type_ID = task.Task_Type_ID,
                                Task_Type_Description = task.Task_Type.Task_Type_Description,
                                Importance_ID = task.Importance_ID,
                                Importance_Description = task.Importance.Importance_Description,
                                Task_Duration = task.Task_Duration,
                                Task_Description = task.Task_Description
                            };

                                 var taskSkills = from taskSkill in db.Task_Skill
                                 where taskSkill.Task_ID == id
                                 select new
                                 {
                                     Task_ID = taskSkill.Task_ID,
                                     Skill_ID = taskSkill.Skill_ID,
                                     Skill_Description = taskSkill.Skill.Skill_Description
                                 };
                var skills = from taskSkill in db.Task_Skill
                             join
                             skill in db.Skills on taskSkill.Skill_ID equals skill.Skill_ID
                             where taskSkill.Task_ID == id
                             select new
                             {
                                 Skill_ID = skill.Skill_ID,
                                 Skill_Description = skill.Skill_Description
                             };

                dynamic taskReturn = tasks.ToList<dynamic>().FirstOrDefault();
                dynamic taskSkillReturn = taskSkills.ToList<dynamic>();
                dynamic myObj = new ExpandoObject();
                myObj.Tasks = tasks;
                myObj.TaskSkills = taskSkills;
                myObj.Skills = skills;
                return Content(HttpStatusCode.OK, myObj);

            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Null entry error:"); //return empty request
            }
        }


        //============================= add task =============================
        [HttpPost]
        [Route("api/Task/AddTask")]
        public IHttpActionResult AddTask([FromBody] TaskSkillClass taskSkill)
        {

            if (taskSkill != null)
            {
                try
                {
                    db.Tasks.Add(taskSkill.Task); //add task
                    if (taskSkill.Skill != null)
                    {
                        foreach (Skill skill in taskSkill.Skill)
                        {
                            Task_Skill newTaskSkill = new Task_Skill();
                            newTaskSkill.Task_ID = taskSkill.Task.Task_ID;
                            newTaskSkill.Skill_ID = skill.Skill_ID;
                            newTaskSkill.TS_ID = 0;
                            db.Task_Skill.Add(newTaskSkill); //add task skill
                        }
                    }

                    db.SaveChanges(); //save changes

                    var auditQuery = from task in db.Tasks
                                     join section in db.Sections on task.Section_ID equals section.Section_ID
                                     join farm in db.Farms on section.Farm_ID equals farm.Farm_ID
                                     join farmUserPosition in db.Farm_User_User_Position on farm.Farm_ID equals farmUserPosition.Farm_ID
                                     join farmUser in db.Farm_User on farmUserPosition.Farm_User_ID equals farmUser.Farm_User_ID
                                     where task.Task_ID == taskSkill.Task.Task_ID
                                     select new
                                     {
                                         Farm_ID = section.Farm_ID,
                                         User_ID = farmUser.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail auditLog = new Audit_Trail();
                    auditLog.Farm_ID = auditDetails.Farm_ID;
                    auditLog.User_ID = auditDetails.User_ID;
                    auditLog.Affected_ID = taskSkill.Task.Task_ID;
                    auditLog.Action_DateTime = DateTime.Now;
                    auditLog.User_Action = "Added a new task";
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

        //===================== update task; ====================================
        [HttpPut]
        [Route("api/Task/UpdateTask/{id}")]
        public IHttpActionResult UpdateTask(int id, TaskSkillClass updateTask)
        {

            try
            {

                foreach (Task_Skill taskSkill in db.Task_Skill)
                {
                    if (taskSkill.Task_ID == updateTask.Task.Task_ID)
                    {
                        db.Task_Skill.Remove(taskSkill);

                    }
                }
                Task temp = db.Tasks.Where(x => x.Task_ID == id).FirstOrDefault(); //find task
                temp.Task_ID = updateTask.Task.Task_ID;
                temp.Section_ID = updateTask.Task.Section_ID;
                temp.Infrastructure_ID = updateTask.Task.Infrastructure_ID;
                temp.Equipment_ID = updateTask.Task.Equipment_ID;
                temp.Task_Type_ID = updateTask.Task.Task_Type_ID;
                temp.Task_Description = updateTask.Task.Task_Description;
                temp.Task_Duration = updateTask.Task.Task_Duration;

                if (updateTask.Skill != null)
                {
                    foreach (Skill skill in updateTask.Skill)
                    {
                        Task_Skill newTaskSkill = new Task_Skill();
                        newTaskSkill.Task_ID = updateTask.Task.Task_ID;
                        newTaskSkill.Skill_ID = skill.Skill_ID;
                        newTaskSkill.TS_ID = 0;
                        db.Task_Skill.Add(newTaskSkill);
                    }
                }



                db.SaveChanges();

                var auditQuery = from task in db.Tasks
                                 join section in db.Sections on task.Section_ID equals section.Section_ID
                                 join farm in db.Farms on section.Farm_ID equals farm.Farm_ID
                                 join farmUserPosition in db.Farm_User_User_Position on farm.Farm_ID equals farmUserPosition.Farm_ID
                                 join farmUser in db.Farm_User on farmUserPosition.Farm_User_ID equals farmUser.Farm_User_ID
                                 where task.Task_ID == updateTask.Task.Task_ID
                                 select new
                                 {
                                     Farm_ID = farm.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail auditLog = new Audit_Trail();
                auditLog.Farm_ID = auditDetails.Farm_ID;
                auditLog.User_ID = auditDetails.User_ID;
                auditLog.Affected_ID = updateTask.Task.Task_ID;
                auditLog.Action_DateTime = DateTime.Now;
                auditLog.User_Action = "Updated a task";
                db.Audit_Trail.Add(auditLog);
                db.SaveChanges();

            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Update failed"); //update failed
               
            }
            return Content(HttpStatusCode.OK, "1 row affected"); //success

        }


        //========================= remove staff -- update isActive status =================
        [HttpDelete]
        [Route("api/Task/DeleteTask/{id}")]
        public IHttpActionResult DeleteTask(int id)
        {
            try
            {
                foreach (Task_Skill taskSkill in db.Task_Skill)
                {
                    if (taskSkill.Task_ID == id)
                    {
                        db.Task_Skill.Remove(taskSkill);
                    }
                }
                Task task = db.Tasks.Where(x => x.Task_ID == id).FirstOrDefault(); //find staff
                db.Tasks.Remove(task);
                db.SaveChanges();

                var auditQuery = from tasks in db.Tasks
                                 join section in db.Sections on tasks.Section_ID equals section.Section_ID
                                 join farm in db.Farms on section.Farm_ID equals farm.Farm_ID
                                 join farmUserPosition in db.Farm_User_User_Position on farm.Farm_ID equals farmUserPosition.Farm_ID
                                 join farmUser in db.Farm_User on farmUserPosition.Farm_User_ID equals farmUser.Farm_User_ID
                                 where task.Task_ID == id
                                 select new
                                 {
                                     Farm_ID = farm.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail auditLog = new Audit_Trail();
                auditLog.Farm_ID = auditDetails.Farm_ID;
                auditLog.User_ID = auditDetails.User_ID;
                auditLog.Affected_ID = id;
                auditLog.Action_DateTime = DateTime.Now;
                auditLog.User_Action = "Deleted a task";
                db.Audit_Trail.Add(auditLog);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed");
            }
            return Content(HttpStatusCode.OK, "rows affected"); //success

        }


        //====================================get sections dropdown menu ============================
        [HttpGet]
        [Route("api/Task/Section/{farmID}")]
        public IHttpActionResult getSection(int farmID)
        {
            List<Section> sectionList = new List<Section>();
            foreach (Section section in db.Sections)
            {
                if (section.Farm_ID == farmID && section.Is_Active == "1")
                {
                    Section sectionObject = new Section();
                    sectionObject.Section_ID = section.Section_ID;
                    sectionObject.Section_Name = section.Section_Name;
                    sectionObject.Is_Active = section.Is_Active;
                    sectionList.Add(sectionObject);
                }

            }
            try
            {
                return Ok(sectionList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //====================================get infrastructure dropdown menu ============================
        [HttpGet]
        [Route("api/Task/Infrastructure/{sectionID}")]
        public IHttpActionResult getInfrastructure(int sectionID)
        {
            List<Infrastructure> infraList = new List<Infrastructure>();
            foreach (Infrastructure infrastructure in db.Infrastructures)
            {
                if (infrastructure.Section_ID == sectionID && infrastructure.Is_Active == 1)
                {
                    Infrastructure infraObject = new Infrastructure();
                    infraObject.Section_ID = infrastructure.Section_ID;
                    infraObject.Infrastructure_ID = infrastructure.Infrastructure_ID;
                    infraObject.Infrastructure_Name = infrastructure.Infrastructure_Name;
                    infraObject.Is_Active = infrastructure.Is_Active;
                    infraList.Add(infraObject);
                }

            }
            try
            {
                return Ok(infraList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }


        //====================================get equipment of infrastruture dropdown menu ============================
        [HttpGet]
        [Route("api/Task/EquipmentInfrastructure/{infraID}")]
        public IHttpActionResult getEquipmentofInfrastructure(int infraID)
        {
            List<Equipment> equipList = new List<Equipment>();
            foreach (Equipment equipment in db.Equipments)
            {
                if (equipment.Infrastructure_ID == infraID && equipment.Is_Active == "true")
                {
                    Equipment equipObject = new Equipment();
                    equipObject.Infrastructure_ID = equipment.Infrastructure_ID;
                    equipObject.Equipment_ID = equipment.Equipment_ID;
                    equipObject.Equipment_Description = equipment.Equipment_Description;
                    equipObject.Is_Active = equipment.Is_Active;
                    equipList.Add(equipObject);
                }
            }
            try
            {
                return Ok(equipList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }


        //================================== get equipment of section dropdown menu ============================
        [HttpGet]
        [Route("api/Task/EquipmentSection/{sectionID}")]
        public IHttpActionResult getEquipmentSection(int sectionID)
        {
            List<Equipment> equipList = new List<Equipment>();
            foreach (Equipment equipment in db.Equipments)
            {
                if (equipment.Section_ID == sectionID && equipment.Is_Active == "true" && equipment.Infrastructure_ID == null) //Ek sal dit bysit in die groot DB sit maar vir nou in by jou
                {
                    Equipment equipObject = new Equipment();
                    equipObject.Section_ID = equipment.Section_ID;
                    equipObject.Equipment_ID = equipment.Equipment_ID;
                    equipObject.Equipment_Description = equipment.Equipment_Description;
                    equipObject.Is_Active = equipment.Is_Active;
                    equipList.Add(equipObject);
                }

            }
            try
            {
                return Ok(equipList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }


        //================================== get importance dropdown menu ============================
        [HttpGet]
        [Route("api/Task/Importance")]
        public IHttpActionResult getImportance()
        {
            List<Importance> importanceList = new List<Importance>();
            foreach (Importance importance in db.Importances)
            {
                Importance importanceObject = new Importance();
                importanceObject.Importance_ID = importance.Importance_ID;
                importanceObject.Importance_Description = importance.Importance_Description;
                importanceList.Add(importanceObject);
            }
            try
            {
                return Ok(importanceList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        [HttpGet]
        [Route("api/Task/TaskType")]
        public IHttpActionResult getTaskType()
        {
            List<Task_Type> typeList = new List<Task_Type>();
            foreach (Task_Type taskType in db.Task_Type)
            {
                Task_Type typeObject = new Task_Type();
                typeObject.Task_Type_ID = taskType.Task_Type_ID;
                typeObject.Task_Type_Description = taskType.Task_Type_Description;
                typeList.Add(typeObject);
            }
            try
            {
                return Ok(typeList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }
    }
}

public class TaskSkillClass
{
    public Task Task { get; set; }
    public List<Skill> Skill { get; set; }
}

