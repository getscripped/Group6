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
    public class SectionTypeController : ApiController
    {

        private AgriLogDBEntities db = new AgriLogDBEntities();

        //=======================================Types==================================================

        //============================Get all Section types===========================================
        [HttpGet]
        [Route("api/SectionType/{farmID}")]
        public IHttpActionResult GetSectionTypes(int farmID)
        {
            var querySectionTypes = from secType in db.Section_Type
                                    where secType.Farm_ID == farmID
                                    select new
                                    {
                                        Section_Type_ID = secType.Section_Type_ID,
                                        Section_Type_Description = secType.Section_Type_Description
                                    };

            List<dynamic> sectionTypes = new List<dynamic>();
            try
            {
                sectionTypes = querySectionTypes.ToList<dynamic>(); // << convert to List
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
                
            }

            if (sectionTypes.Count() > 0) //<<< Check if any sections found
            {


                return Content(HttpStatusCode.OK, sectionTypes);   // <<< return data
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified farm has no sections!"); //<< return if nothing found
            }
        }

        //================================Get specific Section type===================================
        [HttpGet]
        [Route("api/SectionTypeDetails/{id}")]
        public IHttpActionResult GetSectionTypeDetails(int id)
        {

            //Section_Type returnOBJ = new Section_Type();

            try
            {
                var querySectionTypes = from secType in db.Section_Type
                                        where secType.Section_Type_ID == id
                                        select new
                                        {
                                            Section_Type_ID = secType.Section_Type_ID,
                                            Section_Type_Description = secType.Section_Type_Description,
                                            Farm_ID = secType.Farm_ID
                                        };// << find Section types
                dynamic toReturn = querySectionTypes.ToList<dynamic>().FirstOrDefault();
                return Content(HttpStatusCode.OK, toReturn);
            }
            catch (Exception)
            {

                return Content(HttpStatusCode.BadRequest, "Null entry error:");
            }


        }


        //=====================================Add new Section type==================================
        [HttpPost]
        [Route("api/SectionType/Add")]
        public IHttpActionResult AddSectionType(Section_Type newSectionType)
        {

            if (newSectionType != null)
            {
                try
                {
                    db.Section_Type.Add(newSectionType);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    
                   return Content(HttpStatusCode.BadRequest, "POST failed");  // <<< database save failed
                }

                return Content(HttpStatusCode.OK, "1 row affected"); // <<< success
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Post item cannot be empty"); // <<< empty object request error
            }
        }


        //=====================================Edit Section Type=========================================
        [HttpPut]
        [Route("api/SectionType/put/{id}")]
        public IHttpActionResult PutSectionType(int id, Section_Type putSectionType)
        {

            try
            {
                Section_Type toPut = db.Section_Type.Where(x => x.Section_Type_ID == id).FirstOrDefault();

                toPut.Section_Type_Description = putSectionType.Section_Type_Description; // <<< Edit data
                toPut.Farm_ID = putSectionType.Farm_ID;

                db.SaveChanges();  // <<< save data
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Edit failed");  // <<< database save failed
               
            }

            return Content(HttpStatusCode.OK, "1 row affected"); // <<< success
        }


        //=====================================Delete Section Type=========================================
        [HttpPut]
        [Route("api/SectionType/Delete/{id}")]
        public IHttpActionResult Delete(int id, Section_Type putSectionType)
        {
            try
            {
                Section_Type sectionType = db.Section_Type.Where(x => x.Section_Type_ID == id).FirstOrDefault(); // << find section Type

                sectionType.Section_Type_Description = putSectionType.Section_Type_Description; // <<< Edit data
                sectionType.Farm_ID = putSectionType.Farm_ID;

                //db.Section_Type.Remove(sectionType);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Delete failed"); // <<< database delete failed
                
            }
            return Content(HttpStatusCode.OK, "1 row affected"); // <<< success
        }

    }
}
