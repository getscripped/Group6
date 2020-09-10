using AgriLogBackend.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace AgriLogBackend.Controllers
{

    public class Insurance_ProviderController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();

        //=====================================getinsuranceproviders=======================================================

        [Route("api/InsuranceProviders/{UserID}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        public IHttpActionResult getInsuranceProviders(int UserID)
        {
            var query = from ip in db.Insurance_Provider
                        join us in db.Users on ip.User_ID equals us.User_ID
                        select new
                        {
                            IP_ID = ip.IP_ID,
                            User_ID = us.User_ID,
                            IP_Name = ip.IP_Name,
                            IP_Phone_Number = ip.IP_Phone_Number,
                            IP_VAT_Number = ip.IP_VAT_Number,
                            IP_Reg_Number = ip.IP_Reg_Number,
                            User_Email = us.User_Email,
                            User_Password = us.User_Password
                        };

            List<dynamic> insurance_providers = new List<dynamic>();
            try
            {
                insurance_providers = query.ToList<dynamic>();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }
            if (insurance_providers.Count > 0)
            {
                dynamic toReturn = new ExpandoObject();
                toReturn.IPList = insurance_providers;

                return Content(HttpStatusCode.OK, toReturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified insurance provider doesnt exist");
            }
        }

        //===========================================getspecificInsuranceproviders======================================================

        // GET: api/Insurance_Provider/5
        [ResponseType(typeof(Insurance_Provider))]
        [Route("api/InsuranceProviderDetails/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult getInsuranceProviderDetails(int id)
        {
            try
            {
                var query = from ipUser in db.Insurance_Provider
                            join user in db.Users on ipUser.User_ID equals user.User_ID
                            where ipUser.User_ID == id
                            select new
                            {
                                IP_ID = ipUser.IP_ID,
                                IP_Name = ipUser.IP_Name,
                                IP_VAT_Number = ipUser.IP_VAT_Number,
                                IP_Reg_Number = ipUser.IP_Reg_Number,
                                IP_Phone_Number = ipUser.IP_Phone_Number,
                                User_Email = ipUser.User.User_Email,
                                User_Password = ipUser.User.User_Password
                            };

                dynamic toReturn = query.ToList<dynamic>().FirstOrDefault();
                return Content(HttpStatusCode.OK, toReturn);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Null entry error: ");
            }
            /*if ( != null)
            {
                return Content(HttpStatusCode.OK, toReturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "No Insurance Provider was found with the specified ID");
            }*/
        }

        //====================================================editInsuranceProvider=====================================================

        // PUT: api/Insurance_Provider/5
        [ResponseType(typeof(void))]
        [Route("api/InsuranceProviders/put/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult putInsuranceProvider(int id, Insurance_Provider putInsuranceProvider)
        {
            try
            {
                Insurance_Provider IP = db.Insurance_Provider.Where(ip => ip.User_ID == id).FirstOrDefault();
                IP.IP_ID = putInsuranceProvider.IP_ID;
                IP.IP_Name = putInsuranceProvider.IP_Name;
                IP.IP_Phone_Number = putInsuranceProvider.IP_Phone_Number;
                IP.IP_Reg_Number = putInsuranceProvider.IP_Reg_Number;
                IP.IP_VAT_Number = putInsuranceProvider.IP_VAT_Number;
                //IP.User_ID = putInsuranceProvider.User_ID;
                db.SaveChanges();

                var auditQuery = from ip in db.Insurance_Provider
                                 join us in db.Users on ip.User_ID equals us.User_ID
                                 select new
                                 {
                                     IP_ID = ip.IP_ID,
                                     User_ID = us.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.IP_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = id;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Updated Insurance Provider details";
                db.Audit_Trail.Add(A_Log);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Failed to update");
            }
            return Content(HttpStatusCode.OK, "1 Row affected");
        }




        //========================================================addInsuranceProvider=============================================



        //==================================================deleteInsuranceproviders===================================================

        // DELETE: api/Insurance_Provider/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [ResponseType(typeof(Insurance_Provider))]
        [Route("api/InsuranceProviders/delete/{id}")]
        public IHttpActionResult deleteInsuranceProvider(int id)
        {
            try
            {
                foreach (Farm farm in db.Farms)
                {
                    if (farm.IP_ID == id)
                    {
                        Farm fipdelete = db.Farms.Where(x => x.IP_ID == id).FirstOrDefault();
                        db.Farms.Remove(fipdelete);
                    }
                }
                Insurance_Provider ipdelete = db.Insurance_Provider.Where(f => f.IP_ID == id).FirstOrDefault();
                db.Insurance_Provider.Remove(ipdelete);
                db.SaveChanges();

                var auditQuery = from ip in db.Insurance_Provider
                                 join us in db.Users on ip.User_ID equals us.User_ID
                                 select new
                                 {
                                     IP_ID = ip.IP_ID,
                                     User_ID = us.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.IP_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = id;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Deleted Insurance Provider";
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

        private bool Insurance_ProviderExists(int id)
        {
            return db.Insurance_Provider.Count(e => e.IP_ID == id) > 0;
        }

        private string Encrypt(string Pass)
        {
            var toEncrypt = Encoding.UTF8.GetBytes(Pass);
            using (SHA512 shaM = new SHA512Managed())
            {
                var hash = shaM.ComputeHash(toEncrypt);
                var hashedpass = new System.Text.StringBuilder(128);
                foreach (var b in hash)
                {
                    hashedpass.Append(b.ToString("X2"));
                }
                return hashedpass.ToString();
            }

        }
    }
}



