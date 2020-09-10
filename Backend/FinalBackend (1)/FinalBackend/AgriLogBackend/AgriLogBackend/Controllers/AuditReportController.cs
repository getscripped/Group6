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
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuditReportController : ApiController
    {
        public class ReadController : ApiController
        {
            private AgriLogDBEntities db = new AgriLogDBEntities();
            [HttpGet]
            [Route("api/AuditReport/getAuditTrail/{farmID}")]
            public IHttpActionResult getAuditReport(int farmID)
            {
                var queryTrail = from audit in db.Audit_Trail
                                 where audit.Farm_ID == farmID
                                 select new
                                 {
                                     Audit_Log_ID = audit.Audit_Log_ID,
                                     User_ID = audit.User_ID,
                                     Farm_ID = audit.Farm_ID,
                                     User_Action = audit.User_Action,
                                     Action_DateTime = audit.Action_DateTime,
                                     Affected_ID = audit.Affected_ID,
                                 };

                List<dynamic> Trail = new List<dynamic>();
                try
                {
                    Trail = queryTrail.ToList<dynamic>(); // << convert to List
                }
                catch (Exception)
                {
                    return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error

                }

                if (Trail.Count() > 0) //<<< Check if any provider found
                {


                    return Content(HttpStatusCode.OK, Trail);   // <<< return data
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "There is no Service or Repair Providers!"); //<< return if nothing found
                }
            }


            //Get report information
            [HttpPost]
            [Route("api/AuditReport/{farmID}")]
            public dynamic AuditReport(int farmID, [FromBody] RequestModel Parameters)
            {
                //load


                db.Configuration.ProxyCreationEnabled = false;

                dynamic newExpando = new ExpandoObject();
                DateTime startDate = Convert.ToDateTime(Parameters.startDate);
                DateTime endDate = Convert.ToDateTime(Parameters.endDate);

                var auditTrail = from audit in db.Audit_Trail
                                 join user in db.Users on audit.User_ID equals user.User_ID
                                 where audit.Farm_ID == farmID
                                 where audit.Action_DateTime >= startDate
                                 where audit.Action_DateTime <= endDate
                                 select new
                                 {
                                     Audit_Log_ID = audit.Audit_Log_ID,
                                     User_ID = audit.User_ID,
                                     Farm_ID = audit.Farm_ID,
                                     User_Action = audit.User_Action,
                                     Action_DateTime = audit.Action_DateTime,
                                     Affected_ID = audit.Affected_ID,
                                 };

                try
                {
                    dynamic auditReturn = auditTrail.ToList<dynamic>();
                    newExpando.Log = auditReturn;
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
            private dynamic getExpandoReport(List<Audit_Trail> auditTrail)
            {
                //Chart
                dynamic outObject = new ExpandoObject();
                var trailList = auditTrail.GroupBy(aa => aa.Audit_Log_ID);
                List<dynamic> audits = new List<dynamic>();
                /*/foreach (var group in trailList)
                {
                    dynamic cO = new ExpandoObject();
                    cO.Name = group.Key;
                    audits.Add(cO);
                }*/
                outObject.Orders = audits;

                //ControlBreaks
                var auditList = auditTrail.GroupBy(bb => bb.Audit_Log_ID);
                List<dynamic> auditGroups = new List<dynamic>();
                //foreach (var group in supplierList)
                //{
                dynamic report = new ExpandoObject();
                //supplier.CompanyName = group.Key;
                //supplier.AveragePrice = group.Average(ga => ga.UnitPrice);
                //Math.Round(supplier.AveragePrice, 2);
                List<dynamic> fAudits = new List<dynamic>();
                //foreach (var p in group)
                //{
                dynamic trailObj = new ExpandoObject();
                //trailObj.Supplier = p.Supplier.CompanyName;
                //trailObj.Category = p.Category.CategoryName;
                //trailObj.Product = p.ProductName;
                //trailObj.Price = p.UnitPrice;
                fAudits.Add(trailObj);
                //}
                report.AuditTrail = fAudits;
                auditGroups.Add(report);
                //}
                outObject.Trail = auditGroups;
                return outObject;
            }
        }
    }

}
public class RequestModel
{
    public string startDate { get; set; }
    public string endDate { get; set; }
}
