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
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Collections;
using System.Web;


namespace New_API_370.Controllers
{
    public class MaintenanceController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();

        //====================================Get all Maintenances============================================
        [HttpGet]
        [Route("api/Maintenances/{farmID}")]
        public IHttpActionResult GetMaintenance(int farmID)
        {

            List<dynamic> dynamicMaintenance = new List<dynamic>();
            List<dynamic> dynamicMaintenanceStaff = new List<dynamic>();
            dynamic newExpando = new ExpandoObject();

            var queryMaintenance = from maintenance in db.Maintenance_Log
                                   join sec in db.Sections on maintenance.Section_ID equals sec.Section_ID
                                   where sec.Farm_ID == farmID
                                   select new
                                   {
                                       Maintenance_ID = maintenance.Maintenance_ID,
                                       Maintenance_Description = maintenance.Maintenance_Description,
                                       Maintenance_Start_Date = maintenance.Maintenance_Start_Date,
                                       Maintenance_End_Date = maintenance.Maintenance_End_Date,
                                       MT_ID = maintenance.MT_ID,
                                       MT_Description = maintenance.Maintenance_Type.MT_Description,
                                       Equipment_ID = maintenance.Equipment_ID,
                                       Equipment_Description = maintenance.Equipment.Equipment_Description,
                                       Infrastructure_ID = maintenance.Infrastructure_ID,
                                       Infrastructure_Name = maintenance.Infrastructure.Infrastructure_Name,
                                       Importance_ID = maintenance.Importance_ID,
                                       Importance_Description = maintenance.Importance.Importance_Description,
                                       Status_ID = maintenance.Status_ID,
                                       Status_Description = maintenance.Status.Status_Description,
                                       Section_ID = maintenance.Section_ID,
                                       Section_Name = maintenance.Section.Section_Name,
                                   };

            var queryMaintenancetaff = from fStaff in db.Maintenance_Staff
                                       join f in db.Maintenance_Log on fStaff.Maintenance_ID equals f.Maintenance_ID
                                       join s in db.Staffs on fStaff.Staff_ID equals s.Staff_ID
                                       select new
                                       {
                                           Maintenance_ID = fStaff.Maintenance_ID,
                                           Staff_ID = fStaff.Staff_ID,
                                           Staff_Name = fStaff.Staff.Staff_Name,
                                           Staff_Surname = fStaff.Staff.Staff_Surname,
                                           Staff_Phone_Number = fStaff.Staff.Staff_Phone_Number
                                       };



            try
            {
                dynamic maintenanceReturn = queryMaintenance.ToList<dynamic>();
                dynamic maintenancetaffReturn = queryMaintenancetaff.ToList<dynamic>();
                newExpando.Maintenance = maintenanceReturn;
                newExpando.Maintenancetaff = maintenancetaffReturn;
                return Content(HttpStatusCode.OK, newExpando);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
                
            }

            /*if (MaintenanceReturn.Count() > 0) //<<< Check if any equipment found
            {


                return Content(HttpStatusCode.OK, MaintenanceReturn);   // <<< return data
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified farm has no Maintenance!"); //<< return if nothing found
            }*/

        }

        //====================================get sections dropdown menu ============================
        [HttpGet]
        [Route("api/Maintenance/Section/{farmID}")]
        public IHttpActionResult getSection(int farmID)
        {
            List<Section> sectionList = new List<Section>();
            foreach (Section section in db.Sections)
            {
                if (section.Farm_ID == farmID && section.Is_Active == "1")
                {
                    Section sectionObject = new Section();
                    sectionObject.Section_ID = section.Section_ID;
                    sectionObject.Section_Name = section.Section_Name;
                    sectionObject.Is_Active = section.Is_Active;
                    sectionList.Add(sectionObject);
                }

            }
            try
            {
                return Ok(sectionList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //====================================get Maintenance types dropdown menu ============================
        [HttpGet]
        [Route("api/Maintenance/MaintenanceTypes")]
        public IHttpActionResult getMaintenanceTypes()
        {
            List<Maintenance_Type> typesList = new List<Maintenance_Type>();
            foreach (Maintenance_Type type in db.Maintenance_Type)
            {
                Maintenance_Type typeObject = new Maintenance_Type();
                typeObject.MT_ID = type.MT_ID;
                typeObject.MT_Description = type.MT_Description;
                typesList.Add(typeObject);

            }
            try
            {
                return Ok(typesList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //====================================get staff dropdown menu ============================
        [HttpGet]
        [Route("api/Maintenance/Staff/{farmID}")]
        public IHttpActionResult getStaff(int farmID)
        {
            List<Staff> staffList = new List<Staff>();
            foreach (Staff staff in db.Staffs)
            {
                if (staff.Farm_ID == farmID && staff.Is_Active == "true")
                {
                    Staff staffObject = new Staff();
                    staffObject.Staff_ID = staff.Staff_ID;
                    staffObject.Staff_Name = staff.Staff_Name;
                    staffObject.Staff_Surname = staff.Staff_Surname;
                    staffObject.Staff_Phone_Number = staff.Staff_Phone_Number;
                    staffObject.Is_Active = staff.Is_Active;
                    staffList.Add(staffObject);
                }

            }
            try
            {
                return Ok(staffList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //====================================get infrastructure dropdown menu ============================
        [HttpGet]
        [Route("api/Maintenance/Infrastructure/{sectionID}")]
        public IHttpActionResult getInfrastructure(int sectionID)
        {
            List<Infrastructure> infraList = new List<Infrastructure>();
            foreach (Infrastructure infrastructure in db.Infrastructures)
            {
                if (infrastructure.Section_ID == sectionID && infrastructure.Is_Active == 1)
                {
                    Infrastructure infraObject = new Infrastructure();
                    infraObject.Section_ID = infrastructure.Section_ID;
                    infraObject.Infrastructure_ID = infrastructure.Infrastructure_ID;
                    infraObject.Infrastructure_Name = infrastructure.Infrastructure_Name;
                    infraObject.Is_Active = infrastructure.Is_Active;
                    infraList.Add(infraObject);
                }

            }
            try
            {
                return Ok(infraList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //====================================get equipment of infrastruture dropdown menu ============================
        [HttpGet]
        [Route("api/Maintenance/EquipmentInfrastructure/{infraID}")]
        public IHttpActionResult getEquipmentofInfrastructure(int infraID)
        {
            List<Equipment> equipList = new List<Equipment>();
            foreach (Equipment equipment in db.Equipments)
            {
                if (equipment.Infrastructure_ID == infraID && equipment.Is_Active == "true")
                {
                    Equipment equipObject = new Equipment();
                    equipObject.Infrastructure_ID = equipment.Infrastructure_ID;
                    equipObject.Equipment_ID = equipment.Equipment_ID;
                    equipObject.Equipment_Description = equipment.Equipment_Description;
                    equipObject.Is_Active = equipment.Is_Active;
                    equipList.Add(equipObject);
                }
            }
            try
            {
                return Ok(equipList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //================================== get equipment of section dropdown menu ============================
        [HttpGet]
        [Route("api/Maintenance/EquipmentSection/{sectionID}")]
        public IHttpActionResult getEquipmentSection(int sectionID)
        {
            List<Equipment> equipList = new List<Equipment>();
            foreach (Equipment equipment in db.Equipments)
            {
                if (equipment.Section_ID == sectionID && equipment.Is_Active == "true") //Ek sal dit bysit in die groot DB sit maar vir nou in by jou
                {
                    Equipment equipObject = new Equipment();
                    equipObject.Section_ID = equipment.Section_ID;
                    equipObject.Equipment_Description = equipment.Equipment_Description;
                    equipObject.Is_Active = equipment.Is_Active;
                    equipList.Add(equipObject);
                }

            }
            try
            {
                return Ok(equipList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //================================== get importance dropdown menu ============================
        [HttpGet]
        [Route("api/Maintenance/Importance")]
        public IHttpActionResult getImportance()
        {
            List<Importance> importanceList = new List<Importance>();
            foreach (Importance importance in db.Importances)
            {
                Importance importanceObject = new Importance();
                importanceObject.Importance_ID = importance.Importance_ID;
                importanceObject.Importance_Description = importance.Importance_Description;
                importanceList.Add(importanceObject);
            }
            try
            {
                return Ok(importanceList);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //==================== get specific Maintenance details ======================
        [HttpGet]
        [Route("api/MaintenanceDetails/{id}")]
        public IHttpActionResult MaintenanceDetails(int id)
        {
            try
            {
                var queryMaintenance = from maintenance in db.Maintenance_Log
                                       where maintenance.Maintenance_ID == id

                                       select new
                                       {
                                           Maintenance_ID = maintenance.Maintenance_ID,
                                           Section_ID = maintenance.Section_ID,
                                           Section_Name = maintenance.Section.Section_Name,
                                           Infrastructure_ID = maintenance.Infrastructure_ID,
                                           Infrastructure_Name = maintenance.Infrastructure.Infrastructure_Name,
                                           Equipment_ID = maintenance.Equipment_ID,
                                           Equipment_Description = maintenance.Equipment.Equipment_Description,
                                           MT_ID = maintenance.MT_ID,
                                           MT_Description = maintenance.Maintenance_Type.MT_Description,
                                           Importance_ID = maintenance.Importance_ID,
                                           Importance_Description = maintenance.Importance.Importance_Description,
                                           Maintenance_Start_Date = maintenance.Maintenance_Start_Date,
                                           Maintenance_End_Date = maintenance.Maintenance_End_Date,
                                           Maintenance_Description = maintenance.Maintenance_Description,
                                           Status_ID = maintenance.Status_ID,
                                           Status_Description = maintenance.Status.Status_Description
                                       };

                var maintenancetaff = from fStaff in db.Maintenance_Staff
                                      where fStaff.Maintenance_ID == id
                                      select new
                                      {
                                          Maintenance_ID = fStaff.Maintenance_ID,
                                          Staff_ID = fStaff.Staff_ID
                                      };

                dynamic maintenanceReturn = queryMaintenance.ToList<dynamic>().FirstOrDefault();
                dynamic maintenancetaffReturn = maintenancetaff.ToList<dynamic>();
                dynamic myObj = new ExpandoObject();
                myObj.Maintenance = queryMaintenance;
                myObj.Maintenancetaff = maintenancetaff;
                return Content(HttpStatusCode.OK, myObj);

            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Null entry error:"); //return empty request
            }
        }


        List<dynamic> staffNumber = new List<dynamic>();

        //============================= add Maintenance =============================
        [HttpPost]
        [Route("api/Maintenance/Add")]
        public IHttpActionResult AddMaintenance([FromBody] MaintenanceStaffClass newMaintenance)
        {
            List<dynamic> staffNumbers = new List<dynamic>();
            List<dynamic> staffIDs = new List<dynamic>();

            if (newMaintenance != null)
            {
                try
                {
                    db.Maintenance_Log.Add(newMaintenance.MaintenanceLog);

                    var maintenanceType = (from fT in db.Maintenance_Type
                                           where fT.MT_ID == newMaintenance.MaintenanceLog.MT_ID
                                           select fT.MT_Description).ToList();

                    var impDescript = (from im in db.Importances
                                       where im.Importance_ID == newMaintenance.MaintenanceLog.Importance_ID
                                       select im.Importance_Description).ToList();

                    var secName = (from secN in db.Sections
                                   where secN.Section_ID == newMaintenance.MaintenanceLog.Section_ID
                                   select secN.Section_Name).ToList();

                    var infraName = (from inf in db.Infrastructures
                                     where inf.Infrastructure_ID == newMaintenance.MaintenanceLog.Infrastructure_ID
                                     select inf.Infrastructure_Name).ToList();

                    var equipDescript = (from eqD in db.Equipments
                                         where eqD.Equipment_ID == newMaintenance.MaintenanceLog.Equipment_ID
                                         select eqD.Equipment_Description).ToList();


                    string type = Convert.ToString(maintenanceType[0]);
                    //string start = Convert.ToString(newMaintenance.MaintenanceLog.Maintenance_Start_Date);
                    string start = newMaintenance.MaintenanceLog.Maintenance_Start_Date.ToString();
                    //string end = Convert.ToString(newMaintenance.MaintenanceLog.Maintenance_End_Date);
                    string end = newMaintenance.MaintenanceLog.Maintenance_End_Date.ToString();
                    string importance = Convert.ToString(impDescript[0]);
                    string section = Convert.ToString(secName[0]);

                    string detailsMessage = "NEW Maintenance Added!" + "\nDescription: " + newMaintenance.MaintenanceLog.Maintenance_Description + "\nMaintenance Type: " + type + "\nStart: " + start + "\nEnd: " + end + "\nImportance: " + importance + "\nSection: " + section + ".";

                    if (infraName.Count() != 0)
                    {
                        string infrastructure = Convert.ToString(infraName[0]);
                        detailsMessage += "\nInfrastructure: " + infrastructure + ".";
                    }
                    if (equipDescript.Count() != 0)
                    {
                        string equipment = Convert.ToString(equipDescript[0]);
                        detailsMessage += "\nEquipment: " + equipment + ".";
                    }




                    foreach (Staff staff in newMaintenance.Staff)
                    {
                        Maintenance_Staff maintenanceStaff = new Maintenance_Staff();
                        dynamic numbers = new ExpandoObject();
                        dynamic idS = new ExpandoObject();
                        maintenanceStaff.Maintenance_ID = newMaintenance.MaintenanceLog.Maintenance_ID;
                        maintenanceStaff.Staff_ID = staff.Staff_ID;
                        maintenanceStaff.Mainstaff_Id = 0;
                        numbers.Staff_Phone_Number = staff.Staff_Phone_Number;
                        staffNumbers.Add(numbers);
                        idS.Staff_ID = staff.Staff_ID;
                        staffIDs.Add(idS);
                        db.Maintenance_Staff.Add(maintenanceStaff);
                    }
                    db.SaveChanges();
                    GetNumbers(staffNumbers);


                    /*
                    foreach (var item in staffNumber)
                    {
                        SendSMS(item, detailsMessage);//(Number, Message, Staff_ID)                 
                    }

                    
                    var auditQuery = from maintenance in db.Maintenance_Log
                                     join sec in db.Sections on maintenance.Section_ID equals sec.Section_ID
                                     join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                     join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                     join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                     where maintenance.Maintenance_ID == newMaintenance.MaintenanceLog.Maintenance_ID
                                     select new
                                     {
                                         Farm_ID = sec.Farm_ID,
                                         User_ID = farmUser.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail A_Log = new Audit_Trail();
                    A_Log.Farm_ID = auditDetails.Farm_ID;
                    A_Log.User_ID = auditDetails.User_ID;
                    A_Log.Affected_ID = newMaintenance.MaintenanceLog.Maintenance_ID;
                    A_Log.Action_DateTime = DateTime.Now;
                    A_Log.User_Action = "Added new Maintenance";
                    db.Audit_Trail.Add(A_Log);
                    db.SaveChanges();

                     */



                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.BadRequest, "POST failed");

                }
                return Content(HttpStatusCode.OK, "rows affected");
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Cant pass a null value");
            }

        }

        [HttpPut]
        [Route("api/Maintenance/put/{ID}")]
        public IHttpActionResult PutMaintenance(int ID, MaintenanceStaffClass putMaintenance)
        {
            List<dynamic> staffNumbers = new List<dynamic>();
            List<dynamic> staffIDs = new List<dynamic>();
            try
            {
                foreach (Maintenance_Staff maintenanceStaff in db.Maintenance_Staff)
                {
                    if (maintenanceStaff.Maintenance_ID == putMaintenance.MaintenanceLog.Maintenance_ID)
                    {
                        db.Maintenance_Staff.Remove(maintenanceStaff);
                    }
                }

                Maintenance_Log maintenance = db.Maintenance_Log.Where(x => x.Maintenance_ID == ID).FirstOrDefault();

                maintenance.Infrastructure_ID = putMaintenance.MaintenanceLog.Infrastructure_ID;
                maintenance.Equipment_ID = putMaintenance.MaintenanceLog.Equipment_ID;
                maintenance.Section_ID = putMaintenance.MaintenanceLog.Section_ID;
                maintenance.Maintenance_Description = putMaintenance.MaintenanceLog.Maintenance_Description;
                maintenance.Maintenance_Start_Date = putMaintenance.MaintenanceLog.Maintenance_Start_Date;
                maintenance.Maintenance_End_Date = putMaintenance.MaintenanceLog.Maintenance_End_Date;
                maintenance.Importance_ID = putMaintenance.MaintenanceLog.Importance_ID;
                maintenance.Status_ID = putMaintenance.MaintenanceLog.Status_ID;
                maintenance.MT_ID = putMaintenance.MaintenanceLog.MT_ID;

                var maintenanceType = (from fT in db.Maintenance_Type
                                       where fT.MT_ID == putMaintenance.MaintenanceLog.MT_ID
                                       select fT.MT_Description).ToList();

                var impDescript = (from im in db.Importances
                                   where im.Importance_ID == putMaintenance.MaintenanceLog.Importance_ID
                                   select im.Importance_Description).ToList();

                var secName = (from secN in db.Sections
                               where secN.Section_ID == putMaintenance.MaintenanceLog.Section_ID
                               select secN.Section_Name).ToList();

                var infraName = (from inf in db.Infrastructures
                                 where inf.Infrastructure_ID == putMaintenance.MaintenanceLog.Infrastructure_ID
                                 select inf.Infrastructure_Name).ToList();

                var equipDescript = (from eqD in db.Equipments
                                     where eqD.Equipment_ID == putMaintenance.MaintenanceLog.Equipment_ID
                                     select eqD.Equipment_Description).ToList();

                string type = Convert.ToString(maintenanceType[0]);
                //string start = Convert.ToString(newmaintenance.MaintenanceLog.Maintenance_Start_Date);
                string start = putMaintenance.MaintenanceLog.Maintenance_Start_Date.ToString();
                //string end = Convert.ToString(newMaintenance.MaintenanceLog.Maintenance_End_Date);
                string end = putMaintenance.MaintenanceLog.Maintenance_End_Date.ToString();
                string importance = Convert.ToString(impDescript[0]);
                string section = Convert.ToString(secName[0]);

                string detailsMessage = "Maintenance UPDATED!" + "\nDescription: " + putMaintenance.MaintenanceLog.Maintenance_Description + "\nMaintenance Type: " + type + "\nStart: " + start + "\nEnd: " + end + "\nImportance: " + importance + "\nSection: " + section + ".";

                if (infraName.Count() != 0)
                {
                    string infrastructure = Convert.ToString(infraName[0]);
                    detailsMessage += "\nInfrastructure: " + infrastructure + ".";
                }
                if (equipDescript.Count() != 0)
                {
                    string equipment = Convert.ToString(equipDescript[0]);
                    detailsMessage += "\nEquipment: " + equipment + ".";
                }


                foreach (Staff staff in putMaintenance.Staff)
                {
                    Maintenance_Staff putMaintenanceStaff = new Maintenance_Staff();
                    dynamic numbers = new ExpandoObject();
                    dynamic idS = new ExpandoObject();
                    putMaintenanceStaff.Maintenance_ID = putMaintenance.MaintenanceLog.Maintenance_ID;
                    putMaintenanceStaff.Staff_ID = staff.Staff_ID;
                    putMaintenanceStaff.Mainstaff_Id = 0;
                    numbers.Staff_Phone_Number = staff.Staff_Phone_Number;
                    staffNumbers.Add(numbers);
                    idS.Staff_ID = staff.Staff_ID;
                    staffIDs.Add(idS);
                    db.Maintenance_Staff.Add(putMaintenanceStaff);
                }

                db.SaveChanges();

                GetNumbers(staffNumbers);

                foreach (var item in staffNumber)
                {
                    foreach (var id in staffIDs)
                    {
                        SendSMS(item, detailsMessage);//(Number, Message, Staff_ID)
                    }
                }

                var auditQuery = from maintenancePut in db.Maintenance_Log
                                 join sec in db.Sections on maintenancePut.Section_ID equals sec.Section_ID
                                 join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                 join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                 join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                 where maintenance.Section_ID == sec.Section_ID
                                 select new
                                 {
                                     Farm_ID = farms.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = maintenance.Maintenance_ID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Updated Maintenance";
                db.Audit_Trail.Add(A_Log);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Db error");
               
            }

            return Content(HttpStatusCode.OK, "1 row affected");
        }


        [HttpDelete]
        [Route("api/Maintenance/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            List<dynamic> staffNumbers = new List<dynamic>();
            List<dynamic> staffIDs = new List<dynamic>();
            try
            {
                Maintenance_Log maintenance = db.Maintenance_Log.Where(x => x.Maintenance_ID == id).FirstOrDefault();

                if (maintenance.Status_ID == 1)
                {
                    foreach (Maintenance_Staff maintenanceStaff in db.Maintenance_Staff)
                    {
                        if (maintenanceStaff.Maintenance_ID == id)
                        {

                            dynamic numbers = new ExpandoObject();
                            dynamic idS = new ExpandoObject();
                            numbers.Staff_Phone_Number = maintenanceStaff.Staff.Staff_Phone_Number;
                            staffNumbers.Add(numbers);
                            idS.Staff_ID = maintenanceStaff.Staff_ID;
                            staffIDs.Add(idS);
                            db.Maintenance_Staff.Remove(maintenanceStaff);
                        }
                    }

                    var maintenanceType = (from fT in db.Maintenance_Type
                                           where fT.MT_ID == maintenance.MT_ID
                                           select fT.MT_Description).ToList();

                    string type = Convert.ToString(maintenanceType[0]);
                    //string start = Convert.ToString(newMaintenance.MaintenanceLog.Maintenance_Start_Date);
                    string start = maintenance.Maintenance_Start_Date.ToString();
                    //string end = Convert.ToString(newMaintenance.MaintenanceLog.Maintenance_End_Date);
                    string end = maintenance.Maintenance_End_Date.ToString();

                    string detailsMessage = "Maintenance DELETED!" + "\nDescription: " + maintenance.Maintenance_Description + "\nMaintenance Type: " + type + "\nStart: " + start + "\nEnd: " + end + ".";

                    db.Maintenance_Log.Remove(maintenance);
                    db.SaveChanges();

                    GetNumbers(staffNumbers);

                    foreach (var item in staffNumber)
                    {
                        SendSMS(item, detailsMessage);//(Number, Message, Staff_ID)
                    }



                    var auditQuery = from fDelete in db.Sections
                                     join sec in db.Sections on fDelete.Section_ID equals sec.Section_ID
                                     join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                     join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                     join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                     where maintenance.Section_ID == sec.Section_ID
                                     select new
                                     {
                                         Farm_ID = farms.Farm_ID,
                                         User_ID = farmUser.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail A_Log = new Audit_Trail();
                    A_Log.Farm_ID = auditDetails.Farm_ID;
                    A_Log.User_ID = auditDetails.User_ID;
                    A_Log.Affected_ID = maintenance.Maintenance_ID;
                    A_Log.Action_DateTime = DateTime.Now;
                    A_Log.User_Action = "Delete";
                    db.Audit_Trail.Add(A_Log);
                    db.SaveChanges();

                    return Content(HttpStatusCode.OK, "rows affected");
                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "Maintenance complete");
                }
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
              
            }


        }





        public dynamic GetNumbers(List<dynamic> numbers)
        {
            try
            {

                foreach (var staff in numbers)
                {
                    staff.Staff_Phone_Number = "+27" + staff.Staff_Phone_Number.Substring(staff.Staff_Phone_Number.Length - 9);
                    staffNumber.Add(staff.Staff_Phone_Number);

                }

                return Content(HttpStatusCode.OK, staffNumber);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, e);  //<<< Database error

            }


        }


        public dynamic SendSMS(string to, string body)
        {
            // This URL is used for sending messages
            string myURI = "https://api.bulksms.com/v1/messages";

            // change these values to match your own account
            string myUsername = "agritech";
            string myPassword = "Farm5pac3";


            //Get number and message from json object
            string sendto = to;
            string sendbody = body;

            // the details of the message we want to send
            string myData = "{to: \"" + sendto + "\", body:\"" + sendbody + "\"}"; //"{to: \"+27785320438\", body:\"Hello Nantes\"}" 
            //=======Gebruik \n in jou JSON body: om te line break==========

            // build the request based on the supplied settings
            var request = WebRequest.Create(myURI);

            // supply the credentials
            request.Credentials = new NetworkCredential(myUsername, myPassword);
            request.PreAuthenticate = true;
            // we want to use HTTP POST
            request.Method = "POST";
            // for this API, the type must always be JSON
            request.ContentType = "application/json";

            // Here we use Unicode encoding, but ASCIIEncoding would also work
            var encoding = new UnicodeEncoding();
            var encodedData = encoding.GetBytes(myData);

            // Write the data to the request stream
            var stream = request.GetRequestStream();
            stream.Write(encodedData, 0, encodedData.Length);
            stream.Close();

            // try ... catch to handle errors nicely
            try
            {
                // make the call to the API
                var response = request.GetResponse();

                // read the response and print it to the console
                var reader = new StreamReader(response.GetResponseStream());
                Console.WriteLine(reader.ReadToEnd());
            }
            catch (WebException ex)
            {
                // show the general message
                Console.WriteLine("An error occurred:" + ex.Message);

                // print the detail that comes with the error
                var reader = new StreamReader(ex.Response.GetResponseStream());
                Console.WriteLine("Error details:" + reader.ReadToEnd());

                return Content(HttpStatusCode.BadRequest, "Message Failed");
            }


            var staffIDQ = (from sID in db.Staffs
                            where sID.Staff_Phone_Number == to
                            select sID.Staff_ID).ToList();
            int staffID = Convert.ToInt32(staffIDQ[0]);

            try
            {

                Staff_Notification_Log staffLog = new Staff_Notification_Log();
                staffLog.Staff_ID = staffID.ToString();
                staffLog.SNL_Message = body;
                staffLog.Date_Time_Sent = DateTime.Now;

                db.Staff_Notification_Log.Add(staffLog);
                db.SaveChanges();

                return Content(HttpStatusCode.OK, "Log updated successfully!");

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Log update Failed");
            }

            return Content(HttpStatusCode.OK, "SMS sent successfully!");

        }

        public class MaintenanceStaffClass
        {
            public Maintenance_Log MaintenanceLog { get; set; }
            public List<Staff> Staff { get; set; }
        }
        public class StaffNumber
        {
            public int staffID { get; set; }
            public string number { get; set; }
        }

    }
}

