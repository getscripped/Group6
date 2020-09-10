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
    public class SRProviderController : ApiController
    {

        private AgriLogDBEntities db = new AgriLogDBEntities();

        //====================================Get all SRPRoviders============================================
        [HttpGet]
        [Route("api/SRProvider/getAllProviders/{farmID}")]
        public IHttpActionResult GetProviders(int farmID)
        {
            var queryProviders = from srP in db.Vehicle_Service_Provider
                                 where srP.Farm_ID == farmID
                                 select new
                                 {
                                     Provider_ID = srP.Provider_ID,
                                     Provider_Name = srP.Provider_Name,
                                     Provider_Contact_Number = srP.Provider_Contact_Number,
                                     Provider_Address = srP.Provider_Address,
                                     Provider_Email = srP.Provider_Email,
                                     Is_Active = srP.Is_Active,
                                     Farm_ID = srP.Farm_ID
                                 };

            List<dynamic> provider = new List<dynamic>();
            try
            {
                provider = queryProviders.ToList<dynamic>(); // << convert to List
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
              
            }

            if (provider.Count() > 0) //<<< Check if any provider found
            {


                return Content(HttpStatusCode.OK, provider);   // <<< return data
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "There is no Service or Repair Providers!"); //<< return if nothing found
            }
        }

        //====================================Get specific SRPRovider details===============================
        [HttpGet]
        [Route("api/SRProviderDetails/{id}")]
        public IHttpActionResult GetSRProviderDetails(int id)
        {
            //Vehicle_Service_Provider returnOBJ = new Vehicle_Service_Provider();

            try
            {
                var queryProvider = from prov in db.Vehicle_Service_Provider
                                    where prov.Provider_ID == id
                                    select new
                                    {
                                        Provider_ID = prov.Provider_ID,
                                        Provider_Name = prov.Provider_Name,
                                        Provider_Contact_Number = prov.Provider_Contact_Number,
                                        Provider_Address = prov.Provider_Address,
                                        Provider_Email = prov.Provider_Email,
                                        Is_Active = prov.Is_Active,
                                        Farm_ID = prov.Farm_ID
                                    };
                dynamic toReturn = queryProvider.ToList<dynamic>().FirstOrDefault();
                return Content(HttpStatusCode.OK, toReturn);// << find Provider
            }
            catch (Exception)
            {

                return Content(HttpStatusCode.BadRequest, "Null entry error:"); // Return empty request error
            }
        }

        //========================================Add new SRProvider========================================
        [HttpPost]
        [Route("api/SRProvider/Add")]
        public IHttpActionResult AddSRProvider(Vehicle_Service_Provider newSRProvider)
        {
            if (newSRProvider != null)
            {
                try
                {
                    db.Vehicle_Service_Provider.Add(newSRProvider);   // <<< try to add new provider
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


        //==========================================Edit SRProvider========================================
        [HttpPut]
        [Route("api/SRProvider/put/{id}")]
        public IHttpActionResult PutSRProvider(int id, Vehicle_Service_Provider putSRProvider)
        {
            try
            {
                Vehicle_Service_Provider toPut = db.Vehicle_Service_Provider.Where(x => x.Provider_ID == id).FirstOrDefault();

                toPut.Provider_Name = putSRProvider.Provider_Name; //<< re assign all values
                toPut.Provider_Contact_Number = putSRProvider.Provider_Contact_Number;
                toPut.Provider_Address = putSRProvider.Provider_Address;
                toPut.Provider_Email = putSRProvider.Provider_Email;
                toPut.Is_Active = putSRProvider.Is_Active;
                toPut.Farm_ID = putSRProvider.Farm_ID;

                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Edit failed");  // <<< Database save failed
                
            }

            return Content(HttpStatusCode.OK, "1 row affected");  // <<< success
        }


        //=========================================Delete SRProvider=======================================
        [HttpPut]
        [Route("api/SRProvider/Delete/{id}")]
        public IHttpActionResult Delete(int id, Vehicle_Service_Provider putSRProvider)
        {
            try
            {
                Vehicle_Service_Provider provider = db.Vehicle_Service_Provider.Where(x => x.Provider_ID == id).FirstOrDefault(); // << find provider

                provider.Provider_Name = putSRProvider.Provider_Name; //<< re assign all values
                provider.Provider_Contact_Number = putSRProvider.Provider_Contact_Number;
                provider.Provider_Address = putSRProvider.Provider_Address;
                provider.Provider_Email = putSRProvider.Provider_Email;
                provider.Is_Active = putSRProvider.Is_Active;
                provider.Farm_ID = putSRProvider.Farm_ID;// Deactivated from frontend

                //db.Vehicle_Service_Provider.Remove(provider);
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
