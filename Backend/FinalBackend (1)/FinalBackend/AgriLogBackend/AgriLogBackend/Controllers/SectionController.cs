using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AgriLogBackend.Models;

namespace AgriLogBackend.Controllers
{
    public class SectionController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();


        //====================================Get all Sections============================================
        [HttpGet]
        [Route("api/Section/{farmID}")]
        public IHttpActionResult GetSections(int farmID)
        {
            var querySections = from sec in db.Sections
                                join types in db.Section_Type on sec.Section_Type_ID
                                equals types.Section_Type_ID
                                where sec.Farm_ID == farmID
                                
                                select new
                                {
                                    Section_ID = sec.Section_ID,
                                    Section_Type_Description = types.Section_Type_Description,
                                    Section_Name = sec.Section_Name,
                                    Section_Size = sec.Section_Size,
                                    Section_Location = sec.Section_Location,
                                    Is_Active = sec.Is_Active
                                };

            List<dynamic> sections = new List<dynamic>();
            try
            {
                sections = querySections.ToList<dynamic>(); // << convert to List
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
               
            }

            if (sections.Count() > 0) //<<< Check if any sections found
            {


                return Content(HttpStatusCode.OK, sections);   // <<< return data
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified farm has no sections!"); //<< return if nothing found
            }
        }

        //====================================Get specific equipment details===============================
        [HttpGet]
        [Route("api/SectionDetails/{id}")]
        public IHttpActionResult GetSectionDetails(int id)
        {
            //Section returnOBJ = new Section();

            try
            {
                var querySections = from sec in db.Sections
                                    join types in db.Section_Type on sec.Section_Type_ID
                                    equals types.Section_Type_ID
                                    where sec.Section_ID == id
                                    select new
                                    {
                                        Section_ID = sec.Section_ID,
                                        Section_Type_ID=sec.Section_Type_ID,
                                        Section_Type_Description = types.Section_Type_Description,
                                        Section_Name = sec.Section_Name,
                                        Section_Size = sec.Section_Size,
                                        Section_Location = sec.Section_Location,
                                        Is_Active = sec.Is_Active
                                    };
                dynamic toReturn = querySections.ToList<dynamic>().FirstOrDefault();
                return Content(HttpStatusCode.OK, toReturn);
            }
            catch (Exception)
            {

                return Content(HttpStatusCode.BadRequest, "Null entry error:"); // Return empty request error
            }


        }


        //========================================Add new Equipment========================================
        [HttpPost]
        [Route("api/Section/Add")]
        public IHttpActionResult AddSection(Section newSection)
        {
            if (newSection != null)
            {
                try
                {
                    db.Sections.Add(newSection);   // <<< try to add new Section
                    db.SaveChanges();   // <<< Save new changes
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


        //==========================================Edit Section========================================
        [HttpPut]
        [Route("api/Section/put/{id}")]
        public IHttpActionResult PutSection(int id, Section putSection)
        {
            try
            {
                Section toPut = db.Sections.Where(x => x.Section_ID == id).FirstOrDefault();

                toPut.Section_Type_ID = putSection.Section_Type_ID; //<< re assign all values
                toPut.Farm_ID = putSection.Farm_ID;
                toPut.Section_Name = putSection.Section_Name;
                toPut.Section_Size = putSection.Section_Size;
                toPut.Section_Location = putSection.Section_Location;
                toPut.Is_Active = putSection.Is_Active;

                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Edit failed");  // <<< Database save failed
                
            }

            return Content(HttpStatusCode.OK, "1 row affected");  // <<< success
        }

        //=========================================Delete Section=======================================
        [HttpPut]
        [Route("api/Section/Delete/{id}")]
        public IHttpActionResult Delete(int id, Section putSection)
        {
            try
            {
                Section section = db.Sections.Where(x => x.Section_ID == id).FirstOrDefault(); // << find equipment

                section.Section_Type_ID = putSection.Section_Type_ID; //<< re assign all values
                section.Farm_ID = putSection.Farm_ID;
                section.Section_Name = putSection.Section_Name;
                section.Section_Size = putSection.Section_Size;
                section.Section_Location = putSection.Section_Location;
                section.Is_Active = putSection.Is_Active;//Is_Active sent from frontend to deactivate

                //db.Sections.Remove(section);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed"); // <<< database delete failed
             
            }
            return Content(HttpStatusCode.OK, "1 row affected"); // <<< success
        }

    }
}
