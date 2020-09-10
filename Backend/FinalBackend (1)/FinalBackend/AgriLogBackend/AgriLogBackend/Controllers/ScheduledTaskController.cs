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
using System.IO;
using System.Configuration;

using System.Web.UI.WebControls;
using System.Text;

namespace test_db.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ScheduledTaskController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();


        //====================================Get all equipment============================================
        [HttpGet]
        [Route("api/ScheduledTask/{farmID}")]
        public IHttpActionResult getScheduledTasks(int farmID)
        {
            var queryScheduledTask = from sch in db.Task_Task_Schedule
                                     join staff in db.Staffs on sch.Staff_ID equals   //<<<< query with joins to return no ID
                                     staff.Staff_ID
                                     join task in db.Tasks on sch.Task_ID
                                     equals task.Task_ID
                                     join day in db.Day_Of_Week on sch.Day_ID
                                     equals day.Day_ID
                                     join stat in db.Status on sch.Status_ID
                                      equals stat.Status_ID
                                     where sch.Task.Section.Farm_ID == farmID
                                     select new
                                     {
                                         Schedule_ID = sch.Schedule_ID,
                                         Staff_Name = staff.Staff_Name,
                                         Task_Description = task.Task_Description,
                                         Day_Description = day.Day_Description,
                                         Schedule_End_Date = sch.Schedule_End_Date,
                                         Schedule_Start_Date = sch.Schedule_Start_Date,
                                         Status_Description = stat.Status_Description
                                     };

            List<dynamic> scheduledTask = new List<dynamic>();
            try
            {
                scheduledTask = queryScheduledTask.ToList<dynamic>(); // << convert to List
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
            }

            if (scheduledTask.Count() > 0) //<<< Check if any equipment found
            {


                return Content(HttpStatusCode.OK, scheduledTask);   // <<< return data
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified farm, section, or infrastructure has no tasks scheduled yet!"); //<< return if nothing found
            }
        }

        //====================================Get specific equipment details===============================
        [HttpGet]
        [Route("api/ScheduledTaskDetails/{id}")]
        public IHttpActionResult getScheduledTaskDetails(int id)
        {
            dynamic returnOBJ = new ExpandoObject();
            //Task_Task_Schedule returnOBJ = new Task_Task_Schedule();

            try
            {
                var queryScheduledTask = from sch in db.Task_Task_Schedule
                                         where sch.Schedule_ID == id
                                         select new
                                         {
                                             Schedule_ID = sch.Schedule_ID,
                                             Staff_ID = sch.Staff_ID,
                                             Task_ID = sch.Task_ID,
                                             Day_ID = sch.Day_ID,
                                             Schedule_End_Date = sch.Schedule_End_Date,
                                             Schedule_Start_Date = sch.Schedule_Start_Date,
                                             Status_ID = sch.Status_ID
                                         };
                returnOBJ = queryScheduledTask.ToList().FirstOrDefault();
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
                return Content(HttpStatusCode.BadRequest, "No scheduled task found with the specified ID");  // <<< Nothing found error
            }
        }


        //========================================Add new Equipment========================================
        [HttpPost]
        [Route("api/ScheduledTask/Add")]
        public IHttpActionResult AddScheduledTask(Task_Task_Schedule newTask_Task_Schedule)
        {
            if (newTask_Task_Schedule != null)
            {
                try
                {
                    db.Task_Task_Schedule.Add(newTask_Task_Schedule);   // <<< try to add new equipment
                    db.SaveChanges();   // <<< Save new changes

                    var auditQuery = from tsk in db.Tasks
                                     join sec in db.Sections on tsk.Section_ID equals sec.Section_ID
                                     join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                     join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                     join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                     where newTask_Task_Schedule.Task.Section_ID == sec.Section_ID
                                     select new
                                     {
                                         Farm_ID = farms.Farm_ID,
                                         User_ID = farmUser.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail A_Log = new Audit_Trail();
                    A_Log.Farm_ID = auditDetails.Farm_ID;
                    A_Log.User_ID = auditDetails.User_ID;
                    A_Log.Affected_ID = newTask_Task_Schedule.Schedule_ID;
                    A_Log.Action_DateTime = DateTime.Now;
                    A_Log.User_Action = "Scheduled task deleted";
                    db.Audit_Trail.Add(A_Log);
                    db.SaveChanges();
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
        [Route("api/ScheduledTask/put/{id}")]
        public IHttpActionResult PutScheduledTask(int id, Task_Task_Schedule putTask_Task_Schedule)
        {
            try
            {
                Task_Task_Schedule toPut = db.Task_Task_Schedule.Where(x => x.Schedule_ID == id).FirstOrDefault();

                toPut.Staff_ID = putTask_Task_Schedule.Staff_ID; //<< re assign all values
                toPut.Task_ID = putTask_Task_Schedule.Task_ID;
                toPut.Day_ID = putTask_Task_Schedule.Day_ID;
                toPut.Schedule_End_Date = putTask_Task_Schedule.Schedule_End_Date;
                toPut.Schedule_Start_Date = putTask_Task_Schedule.Schedule_Start_Date;
                toPut.Status_ID = putTask_Task_Schedule.Status_ID;

                db.SaveChanges();

                var auditQuery = from tsk in db.Tasks
                                 join sec in db.Sections on tsk.Section_ID equals sec.Section_ID
                                 join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                 join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                 join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                 where putTask_Task_Schedule.Task.Section_ID == sec.Section_ID
                                 select new
                                 {
                                     Farm_ID = farms.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = putTask_Task_Schedule.Schedule_ID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Scheduled task details updated";
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
        [Route("api/ScheduledTask/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var scheduledTask = db.Task_Task_Schedule.Where(x => x.Schedule_ID == id).FirstOrDefault(); // << find equipment

                db.Task_Task_Schedule.Remove(scheduledTask);
                db.SaveChangesAsync();

                var auditQuery = from tsk in db.Tasks
                                 join sec in db.Sections on tsk.Section_ID equals sec.Section_ID
                                 join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                 join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                 join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                 where scheduledTask.Task.Section_ID == sec.Section_ID
                                 select new
                                 {
                                     Farm_ID = farms.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = scheduledTask.Schedule_ID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Scheduled task deleted";
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
        [Route("api/ScheduledTask/getStf/{taskID}/{farmID}")]
        public IHttpActionResult getStaff(int taskID, int farmID)
        {
            List<dynamic> toreturn = new List<dynamic>();
            List<dynamic> skills = new List<dynamic>();
            List<dynamic> staff = new List<dynamic>();
            List<string> staffID = new List<string>();
            List<dynamic> checkArray = new List<dynamic>();

            try
            {
                var query = from tsk in db.Tasks
                            join tskskl in db.Task_Skill on tsk.Task_ID equals tskskl.Task_ID
                            join skl in db.Skills on tskskl.Skill_ID equals skl.Skill_ID
                            where tsk.Task_ID == taskID
                            select new { Skill_ID = skl.Skill_ID };
                skills = query.ToList<dynamic>();

                var staffQuery = from stf in db.Staffs
                                 join stfskl in db.Staff_Skill on stf.Staff_ID equals stfskl.Staff_ID
                                 join skl in db.Skills on stfskl.Skill_ID equals skl.Skill_ID

                                 where stf.Farm_ID == farmID
                                 select new { Staff_ID = stf.Staff_ID, Skill_ID = skl.Skill_ID, Staff_Name = stf.Staff_Name };
                staff = staffQuery.ToList<dynamic>();

                for (int i = 0; i < staff.Count(); i++)
                {
                    if (staffID.Contains(staff[i].Staff_ID))
                    {
                        continue;
                    }
                    else
                    {
                        staffID.Add(staff[i].Staff_ID);
                    }
                }

                if (staffID.Count() > 0)
                {
                    foreach (var tempstaff in staffID)
                    {
                        dynamic compositeStaff = new ExpandoObject();
                        List<int> skillsforstaff = new List<int>();
                        compositeStaff.Staff_ID = tempstaff;
                        compositeStaff.Staff_Name = staff.Where(x => x.Staff_ID == tempstaff).FirstOrDefault().Staff_Name;

                        foreach (var skillArray in staff)
                        {
                            if (skillArray.Staff_ID == compositeStaff.Staff_ID)
                            {
                                skillsforstaff.Add(skillArray.Skill_ID);
                            }
                        }

                        List<int> newSkills = new List<int>();
                        foreach (var temp in skills)
                        {
                            newSkills.Add(temp.Skill_ID);
                        }



                        if (!newSkills.Except(skillsforstaff).Any())
                        {

                            checkArray.Add(compositeStaff);
                        }



                    }


                    return Content(HttpStatusCode.OK, checkArray);
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "no staff on system");
                }



            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.BadRequest, "Db error");
            }




        }

        [HttpGet]
        [Route("api/ScheduledTask/getTsk/{farmID}")]
        public IHttpActionResult getTask(int farmID)
        {
            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from tasks in db.Tasks
                                //join tskskl in db.Task_Skill on tasks.Task_ID equals tskskl.Task_ID
                            join sections in db.Sections on tasks.Section_ID equals sections.Section_ID
                            join farm in db.Farms on sections.Farm_ID equals farm.Farm_ID
                            where farm.Farm_ID == farmID
                            select new { Task_ID = tasks.Task_ID, Task_Description = tasks.Task_Description };
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
                return Content(HttpStatusCode.BadRequest, "No tasks found");
            }


        }

        [HttpGet]
        [Route("api/ScheduledTask/getSkls/{taskID}")]
        public IHttpActionResult getSkills(int taskID)
        {
            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from tasks in db.Tasks
                            join tskskl in db.Task_Skill on tasks.Task_ID equals tskskl.Task_ID
                            join skl in db.Skills on tskskl.Skill_ID equals skl.Skill_ID
                            where tasks.Task_ID == taskID
                            select new { Skill_ID = tskskl.Skill_ID, Skill_Description = skl.Skill_Description };
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
                return Content(HttpStatusCode.BadRequest, "No tasks found");
            }


        }

        [HttpGet]
        [Route("api/ScheduledTask/getDay/{farmID}")]
        public IHttpActionResult getDay(int farmID)
        {
            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from days in db.Day_Of_Week
                            select new { Day_ID = days.Day_ID, Day_Description = days.Day_Description };  // No references to other tables, need farm_ID
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
                return Content(HttpStatusCode.BadRequest, "No days found");
            }


        }

        [HttpGet]
        [Route("api/ScheduledTask/getStat/{farmID}")]
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


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Scheduled_TaskExists(int id)
        {
            return db.Task_Task_Schedule.Count(e => e.Schedule_ID == id) > 0;
        }

        [HttpPost]
        [Route("api/SendSMS/{farmID}")]
        public IHttpActionResult SendSMS(int farmID)
        {
            var weekDay = DateTime.Today.DayOfWeek.ToString();
            List<dynamic> toreturn = new List<dynamic>();

            var query = from schedules in db.Task_Task_Schedule
                        join stf in db.Staffs on schedules.Staff_ID equals stf.Staff_ID
                        join tsk in db.Tasks on schedules.Task_ID equals tsk.Task_ID
                        where schedules.Schedule_Start_Date <= DateTime.Today.Date && schedules.Schedule_End_Date >= DateTime.Today.Date && schedules.Day_Of_Week.Day_Description == weekDay && schedules.Task.Section.Farm_ID == farmID
                        select new { Schedule_ID = schedules.Schedule_ID, Task_Description = tsk.Task_Description, Staff_Phone_Number = stf.Staff_Phone_Number, Staff_Name = stf.Staff_Name };
            toreturn = query.ToList<dynamic>();

            foreach (var sms in toreturn)
            {
                // This URL is used for sending messages
                string myURI = "https://api.bulksms.com/v1/messages";

                // change these values to match your own account
                string myUsername = "agritech";
                string myPassword = "Farm5pac3";

                var number = sms.Staff_Phone_Number;
                number = number.Substring(1, 9);
                number = "+27" + number;
                //Get number and message from json object
                string sendto = number;
                string sendbody = sms.Task_Description;

                // the details of the message we want to send
                string myData = "{to: \"" + sendto + "\", body:\"" + sendbody + "\"}"; //"{to: \"+27785320438\", body:\"Hello Nantes\"}" 
                                                                                       //=======Gebruik \n in jou JSON body: om te line break==========

                // build the request based on the supplied settings
                var request = WebRequest.Create(myURI);

                // supply the credentials
                request.Credentials = new NetworkCredential(myUsername, myPassword);
                request.PreAuthenticate = true;
                // we want to use HTTP POST
                request.Method = "POST";
                // for this API, the type must always be JSON
                request.ContentType = "application/json";

                // Here we use Unicode encoding, but ASCIIEncoding would also work
                var encoding = new UnicodeEncoding();
                var encodedData = encoding.GetBytes(myData);

                // Write the data to the request stream
                var stream = request.GetRequestStream();
                stream.Write(encodedData, 0, encodedData.Length);
                stream.Close();

                // try ... catch to handle errors nicely
                try
                {
                    // make the call to the API
                    var response = request.GetResponse();

                    // read the response and print it to the console
                    var reader = new StreamReader(response.GetResponseStream());
                    Console.WriteLine(reader.ReadToEnd());
                }
                catch (WebException ex)
                {
                    // show the general message
                    Console.WriteLine("An error occurred:" + ex.Message);

                    // print the detail that comes with the error
                    var reader = new StreamReader(ex.Response.GetResponseStream());
                    Console.WriteLine("Error details:" + reader.ReadToEnd());

                    return Content(HttpStatusCode.BadRequest, "Message Failed");
                }
            }
            return Content(HttpStatusCode.OK, "SMS sent successfully!");

        }

    }
}