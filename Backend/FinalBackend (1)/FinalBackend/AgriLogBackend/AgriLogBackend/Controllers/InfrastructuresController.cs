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


namespace AgriLogBackend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class InfrastructuresController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();


        //====================================Get all infuipment============================================
        [HttpGet]
        [Route("api/Infrastructures/{farmID}")]
        public IHttpActionResult getInfrastructure(int farmID)
        {
            var queryInfrastructure = from inf in db.Infrastructures
                                      join sec in db.Sections on inf.Section_ID equals   //<<<< query with joins to return no ID
sec.Section_ID
                                      join types in db.Infrastructure_Type on inf.Infrastructure_Type_ID
       equals types.Infrastructure_Type_ID
                                      where inf.Section.Farm_ID == farmID
                                      select new
                                      {
                                          Infrastructure_ID = inf.Infrastructure_ID,
                                          Infrastructure_Type_Description = types.Infrastructure_Type_Description,
                                          Section_Name = sec.Section_Name,
                                          Infrastructure_Name = inf.Infrastructure_Name,
                                          Infrastructure_Size = inf.Infrastructure_Size,
                                          Infrastructure_Location = inf.Infrastructure_Location,
                                          Is_Active = inf.Is_Active
                                      };

            List<dynamic> infrastructure = new List<dynamic>();
            try
            {
                infrastructure = queryInfrastructure.ToList<dynamic>(); // << convert to List
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
  
            }

            if (infrastructure.Count() > 0) //<<< Check if any equipment found
            {


                return Content(HttpStatusCode.OK, infrastructure);   // <<< return data
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified farm has no infrastructure!"); //<< return if nothing found
            }
        }

        //====================================Get specific equipment details===============================
        [HttpGet]
        [Route("api/InfrastructureDetails/{id}")]
        public IHttpActionResult getInfrastructureDetails(int id)
        {
            dynamic returnOBJ = new ExpandoObject();
            //Task_Task_Schedule returnOBJ = new Task_Task_Schedule();

            try
            {
                var queryInfrastructure = from inf in db.Infrastructures
                                          where inf.Infrastructure_ID == id
                                          select new
                                          {
                                              Infrastructure_ID = inf.Infrastructure_ID,
                                              Infrastructure_Type_ID = inf.Infrastructure_Type_ID,
                                              Section_ID = inf.Section_ID,
                                              Infrastructure_Name = inf.Infrastructure_Name,
                                              Infrastructure_Size = inf.Infrastructure_Size,
                                              Infrastructure_Location = inf.Infrastructure_Location,
                                              Is_Active = inf.Is_Active
                                          };
                returnOBJ = queryInfrastructure.ToList().FirstOrDefault();
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
                return Content(HttpStatusCode.BadRequest, "No Infrastructure found with the specified ID");  // <<< Nothing found error
            }
        }


        //========================================Add new Equipment========================================
        [HttpPost]
        [Route("api/Infrastructures/Add")]
        public IHttpActionResult AddInfrastructure(Infrastructure newInfrastructure)
        {
            if (newInfrastructure != null)
            {
                try
                {
                    db.Infrastructures.Add(newInfrastructure);   // <<< try to add new equipment
                    db.SaveChanges();   // <<< Save new changes

                    var auditQuery = from sec in db.Sections
                                     join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                     join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                     join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                     where newInfrastructure.Section_ID == sec.Section_ID
                                     select new
                                     {
                                         Farm_ID = farms.Farm_ID,
                                         User_ID = farmUser.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail A_Log = new Audit_Trail();
                    A_Log.Farm_ID = auditDetails.Farm_ID;
                    A_Log.User_ID = auditDetails.User_ID;
                    A_Log.Affected_ID = newInfrastructure.Infrastructure_ID;
                    A_Log.Action_DateTime = DateTime.Now;
                    A_Log.User_Action = "Added a new infrastructure";
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
        [Route("api/Infrastructures/put/{id}")]
        public IHttpActionResult PutInfrastructure(int id, Infrastructure putInfrastructure)
        {
            try
            {
                Infrastructure toPut = db.Infrastructures.Where(x => x.Infrastructure_ID == id).FirstOrDefault();

                toPut.Infrastructure_Type_ID = putInfrastructure.Infrastructure_Type_ID; //<< re assign all values
                //toPut.Section.Farm_ID = putInfrastructure.Section.Farm_ID;
                toPut.Section_ID = putInfrastructure.Section_ID;
                toPut.Is_Active = putInfrastructure.Is_Active;
                toPut.Infrastructure_Size = putInfrastructure.Infrastructure_Size;
                toPut.Infrastructure_Name = putInfrastructure.Infrastructure_Name;
                toPut.Infrastructure_Location = putInfrastructure.Infrastructure_Location;

                db.SaveChanges();

                var auditQuery = from sec in db.Sections
                                 join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                 join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                 join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                 where putInfrastructure.Section_ID == sec.Section_ID
                                 select new
                                 {
                                     Farm_ID = farms.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = putInfrastructure.Infrastructure_ID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Infrastructure updated";
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
        [Route("api/Infrastructures/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var infrastructure = db.Infrastructures.Where(x => x.Infrastructure_ID == id).FirstOrDefault(); // << find equipment

                db.Infrastructures.Remove(infrastructure);
                db.SaveChangesAsync();

                var auditQuery = from sec in db.Sections
                                 join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                 join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                 join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                 where infrastructure.Section_ID == sec.Section_ID
                                 select new
                                 {
                                     Farm_ID = farms.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = infrastructure.Infrastructure_ID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Infrastructure Deleted";
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
        [Route("api/InfrastructureTypes")]
        public IHttpActionResult GetInfrastructureType()
        {

            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from type in db.Infrastructure_Type
                            select new { Infrastructure_Type_ID = type.Infrastructure_Type_ID, Infrastructure_Type_Description = type.Infrastructure_Type_Description };
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
                return Content(HttpStatusCode.BadRequest, "No sections found");
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

        private bool InfrastrutureExists(int id)
        {
            return db.Infrastructures.Count(e => e.Infrastructure_ID == id) > 0;
        }

        [HttpGet]
        [Route("api/Infrastructures/getSec/{farmID}")]
        public IHttpActionResult getSections(int farmID)
        {
            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from sec in db.Sections
                            join farm in db.Farms on sec.Farm_ID equals farm.Farm_ID
                            where farm.Farm_ID == farmID
                            select new { Section_ID = sec.Section_ID, Section_Name = sec.Section_Name };
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
                return Content(HttpStatusCode.BadRequest, "No sections found");
            }


        }
    }
}
