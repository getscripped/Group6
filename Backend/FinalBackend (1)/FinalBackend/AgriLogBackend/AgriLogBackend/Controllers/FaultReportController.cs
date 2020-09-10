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
using System.IO;
using System.Data.SqlClient;
using System.Web.Http.Cors;
using System.Web.Hosting;
using System.Net.Http.Headers;
using System.Data;
using System.Web;

namespace AgriLogBackend.Controllers
{
    public class FaultReportController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();
        /*[HttpGet]
        [Route("api/FaultReport/getFaultReport/{farmID}")]
        public IHttpActionResult getFaultReport(int farmID)
        {
            var queryFaults = from fault in db.Fault_Log
                              join sec in db.Sections on fault.Section_ID equals sec.Section_ID
                             where sec.Farm_ID == farmID
                             select new
                             {
                                 Fault_ID = fault.Fault_ID,
                                 Fault_Description = fault.Fault_Description,
                                 Fault_Start_Date = fault.Fault_Start_Date,
                                 Fault_End_Date = fault.Fault_End_Date,
                                 FT_ID = fault.FT_ID,
                                 FT_Description = fault.Fault_Type.FT_Description,
                                 Equipment_ID = fault.Equipment_ID,
                                 Equipment_Description = fault.Equipment.Equipment_Description,
                                 Infrastructure_ID = fault.Infrastructure_ID,
                                 Infrastructure_Name = fault.Infrastructure.Infrastructure_Name,
                                 Importance_ID = fault.Importance_ID,
                                 Importance_Description = fault.Importance.Importance_Description,
                                 Status_ID = fault.Status_ID,
                                 Status_Description = fault.Status.Status_Description,
                                 Section_ID = fault.Section_ID,
                                 Section_Name = fault.Section.Section_Name,
                             };

            List<dynamic> Faults = new List<dynamic>();
            try
            {
                Faults = queryFaults.ToList<dynamic>(); // << convert to List
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
                throw;
            }

            if (Faults.Count() > 0) //<<< Check if any provider found
            {


                return Content(HttpStatusCode.OK, Faults);   // <<< return data
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "There is no faults"); //<< return if nothing found
            }
        }*/


        //Get report information
        [HttpPost]
        [Route("api/FaultReport/{farmID}")]
        public dynamic FaultReport(int farmID, [FromBody] RequestFaultModel Parameters)
        {
            //load


            db.Configuration.ProxyCreationEnabled = false;

            dynamic newExpando = new ExpandoObject();
            DateTime startDate = Convert.ToDateTime(Parameters.startDate);
            DateTime endDate = Convert.ToDateTime(Parameters.endDate);

            var queryFaults = from fault in db.Fault_Log
                              join sec in db.Sections on fault.Section_ID equals sec.Section_ID
                              where sec.Farm_ID == farmID
                              where fault.Fault_Start_Date >= startDate
                              where fault.Fault_Start_Date <= endDate
                              select new
                              {
                                  Fault_ID = fault.Fault_ID,
                                  Fault_Description = fault.Fault_Description,
                                  Fault_Start_Date = fault.Fault_Start_Date,
                                  Fault_End_Date = fault.Fault_End_Date,
                                  FT_ID = fault.FT_ID,
                                  FT_Description = fault.Fault_Type.FT_Description,
                                  Equipment_ID = fault.Equipment_ID,
                                  Equipment_Description = fault.Equipment.Equipment_Description,
                                  Infrastructure_ID = fault.Infrastructure_ID,
                                  Infrastructure_Name = fault.Infrastructure.Infrastructure_Name,
                                  Importance_ID = fault.Importance_ID,
                                  Importance_Description = fault.Importance.Importance_Description,
                                  Status_ID = fault.Status_ID,
                                  Status_Description = fault.Status.Status_Description,
                                  Section_ID = fault.Section_ID,
                                  Section_Name = fault.Section.Section_Name,
                              };

            try
            {
                dynamic faultReturn = queryFaults.ToList<dynamic>();
                newExpando.Faults = faultReturn;
                return Content(HttpStatusCode.OK, newExpando);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
                
            }


            /*if (categoryID == 1)//Beverages
            {
                products = db.Products.Include(yy => yy.Supplier).Include(zz => zz.Category).Where(aa => aa.CategoryID == categoryID).ToList();
            }*/



        }
        private dynamic getExpandoReport(List<Fault_Log> faultLog)
        {
            //Chart
            dynamic outObject = new ExpandoObject();
            var faultList = faultLog.GroupBy(aa => aa.Fault_ID);
            List<dynamic> faults = new List<dynamic>();
            /*/foreach (var group in trailList)
            {
                dynamic cO = new ExpandoObject();
                cO.Name = group.Key;
                audits.Add(cO);
            }*/
            outObject.Faults = faults;

            //ControlBreaks
            var faultBreak = faults.GroupBy(bb => bb.Fault_ID);
            List<dynamic> faultGroups = new List<dynamic>();
            //foreach (var group in supplierList)
            //{
            dynamic report = new ExpandoObject();
            //supplier.CompanyName = group.Key;
            //supplier.AveragePrice = group.Average(ga => ga.UnitPrice);
            //Math.Round(supplier.AveragePrice, 2);
            List<dynamic> fFaults = new List<dynamic>();
            //foreach (var p in group)
            //{
            dynamic fObj = new ExpandoObject();
            //trailObj.Supplier = p.Supplier.CompanyName;
            //trailObj.Category = p.Category.CategoryName;
            //trailObj.Product = p.ProductName;
            //trailObj.Price = p.UnitPrice;
            fFaults.Add(fObj);
            //}
            report.Faults = fFaults;
            faultGroups.Add(report);
            //}
            outObject.Fault = faultGroups;
            return outObject;
        }
    }
}


public class RequestFaultModel
{
    public string startDate { get; set; }
    public string endDate { get; set; }
}
