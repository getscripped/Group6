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
    public class SectionReportController : ApiController
    {
        //====================================Get all report data============================================
        private AgriLogDBEntities db = new AgriLogDBEntities();
        [HttpPost]
        [Route("api/SectionReport/{farmID}")]
        public dynamic SectionReport(int farmID, [FromBody] RequestSectionModel Parameters)
        {
            //load


            db.Configuration.ProxyCreationEnabled = false;

            dynamic newExpando = new ExpandoObject();
            int selectSection = Parameters.section;

            var querySection = from sect in db.Sections
                               join secType in db.Section_Type on sect.Section_Type_ID equals secType.Section_Type_ID
                               where sect.Farm_ID == farmID
                               select new
                               {
                                   Section_ID = sect.Section_ID,
                                   Section_Name = sect.Section_Name,
                                   Section_Size = sect.Section_Size,
                                   Section_Location = sect.Section_Location,
                                   Section_Type_ID = sect.Section_Type_ID,
                                   Section_Type_Descritpion = sect.Section_Type.Section_Type_Description,

                               };
            var queryEquipment = from eq in db.Equipments
                                 join eqType in db.Equipment_Type on eq.Equipment_Type_ID equals eqType.Equipment_Type_ID
                                 join eqSec in db.Sections on eq.Section_ID equals eqSec.Section_ID
                                 where eq.Section_ID == selectSection
                                 select new
                                 {
                                     Equipment_ID = eq.Equipment_ID,
                                     Equipment_Description = eq.Equipment_Description,
                                     Equipment_Condition = eq.Equipment_Condition,
                                     Equipment_Type_ID = eq.Equipment_Type_ID,
                                     Equipment_Type_Description = eq.Equipment_Type.Equipment_Type_Description,
                                     Section_ID = eq.Section_ID,
                                     Section_Name = eq.Section.Section_Name,
                                 };
            var queryInfrastructure = from infra in db.Infrastructures
                                      join infraType in db.Infrastructure_Type on infra.Infrastructure_Type_ID equals infraType.Infrastructure_Type_ID
                                      join infraSec in db.Sections on infra.Section_ID equals infraSec.Section_ID
                                      where infra.Section_ID == selectSection
                                      select new
                                      {
                                          Infrastructure_ID = infra.Infrastructure_ID,
                                          Infrastructure_Description = infra.Infrastructure_Name,
                                          Infrastructure_Size = infra.Infrastructure_Size,
                                          Infrastructure_Type_ID = infra.Infrastructure_Type_ID,
                                          Infrastructure_Type_Description = infra.Infrastructure_Type.Infrastructure_Type_Description,
                                          Section_ID = infra.Section_ID,
                                          Section_Name = infra.Section.Section_Name,
                                      };
            var queryTask = from task in db.Tasks
                            join taskType in db.Task_Type on task.Task_Type_ID equals taskType.Task_Type_ID
                            join taskSec in db.Sections on task.Section_ID equals taskSec.Section_ID
                            where task.Section_ID == selectSection
                            select new
                            {
                                Task_ID = task.Equipment_ID,
                                Task_Description = task.Task_Description,
                                Task_Condition = task.Task_Duration,
                                Task_Type_ID = task.Task_Type_ID,
                                Task_Type_Description = task.Task_Type.Task_Type_Description,
                                Section_ID = task.Section_ID,
                                Section_Name = task.Section.Section_Name,
                            };
            var queryFaults = from fault in db.Fault_Log
                              join fType in db.Fault_Type on fault.FT_ID equals fType.FT_ID
                              join fSec in db.Sections on fault.Section_ID equals fSec.Section_ID
                              where fault.Section_ID == selectSection
                              select new
                              {
                                  Fault_ID = fault.Equipment_ID,
                                  Fault_Description = fault.Fault_Description,
                                  Status_ID = fault.Status_ID,
                                  Status_Description = fault.Status.Status_Description,
                                  FT_ID = fault.FT_ID,
                                  FT_Description = fault.Fault_Type.FT_Description,
                                  Section_ID = fault.Section_ID,
                                  Section_Name = fault.Section.Section_Name,
                              };
            var queryMaintenance = from maintenance in db.Maintenance_Log
                                   join mType in db.Maintenance_Type on maintenance.MT_ID equals mType.MT_ID
                                   join mSec in db.Sections on maintenance.Section_ID equals mSec.Section_ID
                                   where maintenance.Section_ID == selectSection
                                   select new
                                   {
                                       Maintenance_ID = maintenance.Maintenance_ID,
                                       Maintenance_Description = maintenance.Maintenance_Description,
                                       Status_ID = maintenance.Status_ID,
                                       Status_Description = maintenance.Status.Status_Description,
                                       MT_ID = maintenance.MT_ID,
                                       MT_Description = maintenance.Maintenance_Type.MT_Description,
                                       Section_ID = maintenance.Section_ID,
                                       Section_Name = maintenance.Section.Section_Name,
                                   };
            var queryVehicles = from veh in db.Vehicles
                                join vehType in db.Vehicle_Type on veh.Vehicle_Type_ID equals vehType.Vehicle_Type_ID
                                join vehSec in db.Sections on veh.Section_ID equals vehSec.Section_ID
                                where veh.Section_ID == selectSection
                                select new
                                {
                                    Vehicle_ID = veh.Vehicle_ID,
                                    Vehicle_Type_ID = veh.Vehicle_Type_ID,
                                    Vehicle_Type_Description = veh.Vehicle_Type.Vehicle_Type_Description,
                                    Vehicle_Registration_Number = veh.Vehicle_Registration_Number,
                                    Vehicle_Mileage_Current = veh.Vehicle_Mileage_Current,
                                    Vehicle_Purchase_Price = veh.Vehicle_Purchase_Price,
                                    Section_ID = veh.Section_ID,
                                    Section_Name = veh.Section.Section_Name,
                                };
            try
            {
                dynamic sectionReturn = querySection.ToList<dynamic>();
                dynamic taskReturn = queryTask.ToList<dynamic>();
                dynamic faultReturn = queryFaults.ToList<dynamic>();
                dynamic maintenanceReturn = queryMaintenance.ToList<dynamic>();
                dynamic vehicleReturn = queryVehicles.ToList<dynamic>();
                dynamic infrastructureReturn = queryInfrastructure.ToList<dynamic>();
                dynamic equipmentReturn = queryEquipment.ToList<dynamic>();
                newExpando.Section = sectionReturn;
                newExpando.Task = taskReturn;
                newExpando.Fault = faultReturn;
                newExpando.Maintenance = maintenanceReturn;
                newExpando.Vehicle = vehicleReturn;
                newExpando.Infrastructure = infrastructureReturn;
                newExpando.Equipment = equipmentReturn;
                return Content(HttpStatusCode.OK, newExpando);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
               
            }
        }

    }
}
public class RequestSectionModel
{
    public int section { get; set; }
}