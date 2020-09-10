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
    public class MaintenanceReportController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();
        /*[HttpGet]
        [Route("api/MaintenanceReport/getMaintenanceReport/{farmID}")]
        public IHttpActionResult getMaintenanceReport(int farmID)
        {
            var queryMaintenances = from Maintenance in db.Maintenance_Log
                              join sec in db.Sections on Maintenance.Section_ID equals sec.Section_ID
                             where sec.Farm_ID == farmID
                             select new
                             {
                                 Maintenance_ID = Maintenance.Maintenance_ID,
                                 Maintenance_Description = Maintenance.Maintenance_Description,
                                 Maintenance_Start_Date = Maintenance.Maintenance_Start_Date,
                                 Maintenance_End_Date = Maintenance.Maintenance_End_Date,
                                 FT_ID = Maintenance.FT_ID,
                                 FT_Description = Maintenance.Maintenance_Type.FT_Description,
                                 Equipment_ID = Maintenance.Equipment_ID,
                                 Equipment_Description = Maintenance.Equipment.Equipment_Description,
                                 Infrastructure_ID = Maintenance.Infrastructure_ID,
                                 Infrastructure_Name = Maintenance.Infrastructure.Infrastructure_Name,
                                 Importance_ID = Maintenance.Importance_ID,
                                 Importance_Description = Maintenance.Importance.Importance_Description,
                                 Status_ID = Maintenance.Status_ID,
                                 Status_Description = Maintenance.Status.Status_Description,
                                 Section_ID = Maintenance.Section_ID,
                                 Section_Name = Maintenance.Section.Section_Name,
                             };

            List<dynamic> Maintenances = new List<dynamic>();
            try
            {
                Maintenances = queryMaintenances.ToList<dynamic>(); // << convert to List
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
                throw;
            }

            if (Maintenances.Count() > 0) //<<< Check if any provider found
            {


                return Content(HttpStatusCode.OK, Maintenances);   // <<< return data
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "There is no Maintenances"); //<< return if nothing found
            }
        }*/


        //Get report information
        [HttpPost]
        [Route("api/MaintenanceReport/{farmID}")]
        public dynamic MaintenanceReport(int farmID, [FromBody] RequestMaintenanceModel Parameters)
        {
            //load


            db.Configuration.ProxyCreationEnabled = false;

            dynamic newExpando = new ExpandoObject();
            DateTime startDate = Convert.ToDateTime(Parameters.startDate);
            DateTime endDate = Convert.ToDateTime(Parameters.endDate);

            var queryMaintenances = from Maintenance in db.Maintenance_Log
                                    join sec in db.Sections on Maintenance.Section_ID equals sec.Section_ID
                                    where sec.Farm_ID == farmID
                                    where Maintenance.Maintenance_Start_Date >= startDate
                                    where Maintenance.Maintenance_Start_Date <= endDate
                                    select new
                                    {
                                        Maintenance_ID = Maintenance.Maintenance_ID,
                                        Maintenance_Description = Maintenance.Maintenance_Description,
                                        Maintenance_Start_Date = Maintenance.Maintenance_Start_Date,
                                        Maintenance_End_Date = Maintenance.Maintenance_End_Date,
                                        FT_ID = Maintenance.MT_ID,
                                        FT_Description = Maintenance.Maintenance_Type.MT_Description,
                                        Equipment_ID = Maintenance.Equipment_ID,
                                        Equipment_Description = Maintenance.Equipment.Equipment_Description,
                                        Infrastructure_ID = Maintenance.Infrastructure_ID,
                                        Infrastructure_Name = Maintenance.Infrastructure.Infrastructure_Name,
                                        Importance_ID = Maintenance.Importance_ID,
                                        Importance_Description = Maintenance.Importance.Importance_Description,
                                        Status_ID = Maintenance.Status_ID,
                                        Status_Description = Maintenance.Status.Status_Description,
                                        Section_ID = Maintenance.Section_ID,
                                        Section_Name = Maintenance.Section.Section_Name,
                                    };

            try
            {
                dynamic MaintenanceReturn = queryMaintenances.ToList<dynamic>();
                newExpando.Maintenances = MaintenanceReturn;
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
        private dynamic getExpandoReport(List<Maintenance_Log> MaintenanceLog)
        {
            //Chart
            dynamic outObject = new ExpandoObject();
            var MaintenanceList = MaintenanceLog.GroupBy(aa => aa.Maintenance_ID);
            List<dynamic> Maintenances = new List<dynamic>();
            /*/foreach (var group in trailList)
            {
                dynamic cO = new ExpandoObject();
                cO.Name = group.Key;
                audits.Add(cO);
            }*/
            outObject.Maintenances = Maintenances;

            //ControlBreaks
            var MaintenanceBreak = Maintenances.GroupBy(bb => bb.Maintenance_ID);
            List<dynamic> MaintenanceGroups = new List<dynamic>();
            //foreach (var group in supplierList)
            //{
            dynamic report = new ExpandoObject();
            //supplier.CompanyName = group.Key;
            //supplier.AveragePrice = group.Average(ga => ga.UnitPrice);
            //Math.Round(supplier.AveragePrice, 2);
            List<dynamic> fMaintenances = new List<dynamic>();
            //foreach (var p in group)
            //{
            dynamic fObj = new ExpandoObject();
            //trailObj.Supplier = p.Supplier.CompanyName;
            //trailObj.Category = p.Category.CategoryName;
            //trailObj.Product = p.ProductName;
            //trailObj.Price = p.UnitPrice;
            fMaintenances.Add(fObj);
            //}
            report.Maintenances = fMaintenances;
            MaintenanceGroups.Add(report);
            //}
            outObject.Maintenance = MaintenanceGroups;
            return outObject;
        }
    }
}


public class RequestMaintenanceModel
{
    public string startDate { get; set; }
    public string endDate { get; set; }
}
