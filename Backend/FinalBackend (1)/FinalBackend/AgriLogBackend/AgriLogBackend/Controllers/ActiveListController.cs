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
    public class ActiveListController : ApiController
    {


        private AgriLogDBEntities db = new AgriLogDBEntities();

        [HttpGet]
        [Route("api/ActiveListFM/{farmID}")]
        public IHttpActionResult GetActiveFaults(int farmID)
        {
            dynamic newExpando = new ExpandoObject();

            var queryFaults = from f in db.Fault_Log
                              join fType in db.Fault_Type on f.FT_ID equals fType.FT_ID
                              join fSec in db.Sections on f.Section_ID equals fSec.Section_ID
                              join fImp in db.Importances on f.Importance_ID equals fImp.Importance_ID
                              join fStat in db.Status on f.Status_ID equals fStat.Status_ID
                              where fSec.Farm_ID == farmID
                              where fStat.Status_ID != 1 //1 = Completed
                              select new
                              {
                                  Fault_ID = f.Fault_ID,
                                  Fault_Description = f.Fault_Description,
                                  Fault_Start_DateTime = f.Fault_Start_Date,
                                  Fault_End_Date = f.Fault_End_Date,
                                  FT_ID = f.FT_ID,
                                  FT_Description = f.Fault_Type.FT_Description,
                                  Importance_ID = f.Importance_ID,
                                  Importance_Description = f.Importance.Importance_Description,
                                  Status_ID = f.Status_ID,
                                  Status_Description = f.Status.Status_Description,
                                  Equipment_ID = f.Equipment_ID,
                                  Equipment_Description = f.Equipment.Equipment_Description,
                                  Infrastructure_ID = f.Infrastructure_ID,
                                  Infrastructure_Name = f.Infrastructure.Infrastructure_Name,
                                  Section_ID = f.Section_ID,
                                  Section_Name = f.Section.Section_Name
                              };
            var queryMaintenance = from m in db.Maintenance_Log
                                   join mType in db.Maintenance_Type on m.MT_ID equals mType.MT_ID
                                   join mSec in db.Sections on m.Section_ID equals mSec.Section_ID
                                   join mImp in db.Importances on m.Importance_ID equals mImp.Importance_ID
                                   join mStat in db.Status on m.Status_ID equals mStat.Status_ID
                                   where mSec.Farm_ID == farmID
                                   where mStat.Status_ID != 1 //1 = Completed
                                   select new
                                   {
                                       Maintenance_ID = m.Maintenance_ID,
                                       Maintenance_Description = m.Maintenance_Description,
                                       Maintenance_Start_DateTime = m.Maintenance_Start_Date,
                                       Maintenance_End_Date = m.Maintenance_End_Date,
                                       MT_ID = m.MT_ID,
                                       MT_Description = m.Maintenance_Type.MT_Description,
                                       Importance_ID = m.Importance_ID,
                                       Importance_Description = m.Importance.Importance_Description,
                                       Status_ID = m.Status_ID,
                                       Status_Description = m.Status.Status_Description,
                                       Equipment_ID = m.Equipment_ID,
                                       Equipment_Description = m.Equipment.Equipment_Description,
                                       Infrastructure_ID = m.Infrastructure_ID,
                                       Infrastructure_Name = m.Infrastructure.Infrastructure_Name,
                                       Section_ID = m.Section_ID,
                                       Section_Name = m.Section.Section_Name
                                   };

            try
            {
                dynamic faultsReturn = queryFaults.ToList<dynamic>();
                dynamic maintenanceReturn = queryMaintenance.ToList<dynamic>();

                newExpando.Faults = faultsReturn;
                newExpando.Maintenance = maintenanceReturn;

                return Content(HttpStatusCode.OK, newExpando);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error

            }

        }


    }
}
