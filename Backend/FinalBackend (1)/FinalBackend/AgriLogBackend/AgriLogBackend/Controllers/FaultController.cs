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
    public class FaultController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();

        //====================================Get all Faults============================================
        [HttpGet]
        [Route("api/Faults/{farmID}")]
        public IHttpActionResult GetFaults(int farmID)
        {

            List<dynamic> dynamicFaults = new List<dynamic>();
            List<dynamic> dynamicFaultStaff = new List<dynamic>();
            dynamic newExpando = new ExpandoObject();

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

            var queryFaultStaff = from fStaff in db.Fault_Staff
                                  join f in db.Fault_Log on fStaff.Fault_ID equals f.Fault_ID
                                  join s in db.Staffs on fStaff.Staff_ID equals s.Staff_ID
                                  select new
                                  {
                                      Fault_ID = fStaff.Fault_ID,
                                      Staff_ID = fStaff.Staff_ID,
                                      Staff_Name = fStaff.Staff.Staff_Name,
                                      Staff_Surname = fStaff.Staff.Staff_Surname,
                                      Staff_Phone_Number = fStaff.Staff.Staff_Phone_Number
                                  };



            try
            {
                dynamic faultsReturn = queryFaults.ToList<dynamic>();
                dynamic faultStaffReturn = queryFaultStaff.ToList<dynamic>();
                newExpando.Faults = faultsReturn;
                newExpando.FaultStaff = faultStaffReturn;
                return Content(HttpStatusCode.OK, newExpando);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");  //<<< Database error
               
            }

            /*if (faultsReturn.Count() > 0) //<<< Check if any equipment found
            {


                return Content(HttpStatusCode.OK, faultsReturn);   // <<< return data
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The specified farm has no Faults!"); //<< return if nothing found
            }*/

        }

        //====================================get sections dropdown menu ============================
        [HttpGet]
        [Route("api/Fault/Section/{farmID}")]
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

        //====================================get fault types dropdown menu ============================
        [HttpGet]
        [Route("api/Fault/FaultTypes")]
        public IHttpActionResult getFaultTypes()
        {
            List<Fault_Type> typesList = new List<Fault_Type>();
            foreach (Fault_Type type in db.Fault_Type)
            {
                Fault_Type typeObject = new Fault_Type();
                typeObject.FT_ID = type.FT_ID;
                typeObject.FT_Description = type.FT_Description;
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
        [Route("api/Fault/Staff/{farmID}")]
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
        [Route("api/Fault/Infrastructure/{sectionID}")]
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
        [Route("api/Fault/EquipmentInfrastructure/{infraID}")]
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
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }

        }

        //================================== get equipment of section dropdown menu ============================
        [HttpGet]
        [Route("api/Fault/EquipmentSection/{sectionID}")]
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
        [Route("api/Fault/Importance")]
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

        //==================== get specific fault details ======================
        [HttpGet]
        [Route("api/FaultDetails/{id}")]
        public IHttpActionResult FaultDetails(int id)
        {
            try
            {
                var queryFault = from fault in db.Fault_Log
                                 where fault.Fault_ID == id

                                 select new
                                 {
                                     Fault_ID = fault.Fault_ID,
                                     Section_ID = fault.Section_ID,
                                     Section_Name = fault.Section.Section_Name,
                                     Infrastructure_ID = fault.Infrastructure_ID,
                                     Infrastructure_Name = fault.Infrastructure.Infrastructure_Name,
                                     Equipment_ID = fault.Equipment_ID,
                                     Equipment_Description = fault.Equipment.Equipment_Description,
                                     FT_ID = fault.FT_ID,
                                     FT_Description = fault.Fault_Type.FT_Description,
                                     Importance_ID = fault.Importance_ID,
                                     Importance_Description = fault.Importance.Importance_Description,
                                     Fault_Start_Date = fault.Fault_Start_Date,
                                     Fault_End_Date = fault.Fault_End_Date,
                                     Fault_Description = fault.Fault_Description,
                                     Status_ID = fault.Status_ID,
                                     Status_Description = fault.Status.Status_Description
                                 };

                var faultStaff = from fStaff in db.Fault_Staff
                                 where fStaff.Fault_ID == id
                                 select new
                                 {
                                     Fault_ID = fStaff.Fault_ID,
                                     Staff_ID = fStaff.Staff_ID
                                 };

                dynamic faultReturn = queryFault.ToList<dynamic>().FirstOrDefault();
                dynamic faultStaffReturn = faultStaff.ToList<dynamic>();
                dynamic myObj = new ExpandoObject();
                myObj.Fault = queryFault;
                myObj.FaultStaff = faultStaff;
                return Content(HttpStatusCode.OK, myObj);

            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Null entry error:"); //return empty request
            }
        }


        List<dynamic> staffNumber = new List<dynamic>();

        //============================= add fAULT =============================
        [HttpPost]
        [Route("api/Fault/Add")]
        public IHttpActionResult AddFault([FromBody] FaultStaffClass newFault)
        {
            List<dynamic> staffNumbers = new List<dynamic>();
            List<dynamic> staffIDs = new List<dynamic>();

            if (newFault != null)
            {
                try
                {
                    db.Fault_Log.Add(newFault.FaultLog);

                    var faultType = (from fT in db.Fault_Type
                                     where fT.FT_ID == newFault.FaultLog.FT_ID
                                     select fT.FT_Description).ToList();

                    var impDescript = (from im in db.Importances
                                       where im.Importance_ID == newFault.FaultLog.Importance_ID
                                       select im.Importance_Description).ToList();

                    var secName = (from secN in db.Sections
                                   where secN.Section_ID == newFault.FaultLog.Section_ID
                                   select secN.Section_Name).ToList();

                    var infraName = (from inf in db.Infrastructures
                                     where inf.Infrastructure_ID == newFault.FaultLog.Infrastructure_ID
                                     select inf.Infrastructure_Name).ToList();

                    var equipDescript = (from eqD in db.Equipments
                                         where eqD.Equipment_ID == newFault.FaultLog.Equipment_ID
                                         select eqD.Equipment_Description).ToList();


                    string type = Convert.ToString(faultType[0]);
                    //string start = Convert.ToString(newFault.FaultLog.Fault_Start_Date);
                    string start = newFault.FaultLog.Fault_Start_Date.ToString();
                    //string end = Convert.ToString(newFault.FaultLog.Fault_End_Date);
                    string end = newFault.FaultLog.Fault_End_Date.ToString();
                    string importance = Convert.ToString(impDescript[0]);
                    string section = Convert.ToString(secName[0]);

                    string detailsMessage = "NEW Fault Added!" + "\nDescription: " + newFault.FaultLog.Fault_Description + "\nFault Type: " + type + "\nStart: " + start + "\nEnd: " + end + "\nImportance: " + importance + "\nSection: " + section + ".";

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




                    foreach (Staff staff in newFault.Staff)
                    {
                        Fault_Staff faultStaff = new Fault_Staff();
                        dynamic numbers = new ExpandoObject();
                        dynamic idS = new ExpandoObject();
                        faultStaff.Fault_ID = newFault.FaultLog.Fault_ID;
                        faultStaff.Staff_ID = staff.Staff_ID;
                        faultStaff.FaultStaff_Id = 0;
                        numbers.Staff_Phone_Number = staff.Staff_Phone_Number;
                        staffNumbers.Add(numbers);
                        idS.Staff_ID = staff.Staff_ID;
                        staffIDs.Add(idS);
                        db.Fault_Staff.Add(faultStaff);
                    }
                    db.SaveChanges();
                    GetNumbers(staffNumbers);


                    /*
                    foreach (var item in staffNumber)
                    {
                        SendSMS(item, detailsMessage);//(Number, Message, Staff_ID)                 
                    }

                    var auditQuery = from fault in db.Fault_Log
                                     join sec in db.Sections on fault.Section_ID equals sec.Section_ID
                                     join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                     join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                     join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                     where fault.Fault_ID == newFault.FaultLog.Fault_ID
                                     select new
                                     {
                                         Farm_ID = sec.Farm_ID,
                                         User_ID = farmUser.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail A_Log = new Audit_Trail();
                    A_Log.Farm_ID = auditDetails.Farm_ID;
                    A_Log.User_ID = auditDetails.User_ID;
                    A_Log.Affected_ID = newFault.FaultLog.Fault_ID;
                    A_Log.Action_DateTime = DateTime.Now;
                    A_Log.User_Action = "Added new Fault";
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
        [Route("api/Fault/put/{ID}")]
        public IHttpActionResult PutFault(int ID, FaultStaffClass putFault)
        {
            List<dynamic> staffNumbers = new List<dynamic>();
            List<dynamic> staffIDs = new List<dynamic>();
            try
            {
                foreach (Fault_Staff faultStaff in db.Fault_Staff)
                {
                    if (faultStaff.Fault_ID == putFault.FaultLog.Fault_ID)
                    {
                        db.Fault_Staff.Remove(faultStaff);
                    }
                }

                Fault_Log fault = db.Fault_Log.Where(x => x.Fault_ID == ID).FirstOrDefault();

                fault.Infrastructure_ID = putFault.FaultLog.Infrastructure_ID;
                fault.Equipment_ID = putFault.FaultLog.Equipment_ID;
                fault.Section_ID = putFault.FaultLog.Section_ID;
                fault.Fault_Description = putFault.FaultLog.Fault_Description;
                fault.Fault_Start_Date = putFault.FaultLog.Fault_Start_Date;
                fault.Fault_End_Date = putFault.FaultLog.Fault_End_Date;
                fault.Importance_ID = putFault.FaultLog.Importance_ID;
                fault.Status_ID = putFault.FaultLog.Status_ID;
                fault.FT_ID = putFault.FaultLog.FT_ID;

                var faultType = (from fT in db.Fault_Type
                                 where fT.FT_ID == putFault.FaultLog.FT_ID
                                 select fT.FT_Description).ToList();

                var impDescript = (from im in db.Importances
                                   where im.Importance_ID == putFault.FaultLog.Importance_ID
                                   select im.Importance_Description).ToList();

                var secName = (from secN in db.Sections
                               where secN.Section_ID == putFault.FaultLog.Section_ID
                               select secN.Section_Name).ToList();

                var infraName = (from inf in db.Infrastructures
                                 where inf.Infrastructure_ID == putFault.FaultLog.Infrastructure_ID
                                 select inf.Infrastructure_Name).ToList();

                var equipDescript = (from eqD in db.Equipments
                                     where eqD.Equipment_ID == putFault.FaultLog.Equipment_ID
                                     select eqD.Equipment_Description).ToList();

                string type = Convert.ToString(faultType[0]);
                //string start = Convert.ToString(newFault.FaultLog.Fault_Start_Date);
                string start = putFault.FaultLog.Fault_Start_Date.ToString();
                //string end = Convert.ToString(newFault.FaultLog.Fault_End_Date);
                string end = putFault.FaultLog.Fault_End_Date.ToString();
                string importance = Convert.ToString(impDescript[0]);
                string section = Convert.ToString(secName[0]);

                string detailsMessage = "Fault UPDATED!" + "\nDescription: " + putFault.FaultLog.Fault_Description + "\nFault Type: " + type + "\nStart: " + start + "\nEnd: " + end + "\nImportance: " + importance + "\nSection: " + section + ".";

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


                foreach (Staff staff in putFault.Staff)
                {
                    Fault_Staff putFaultStaff = new Fault_Staff();
                    dynamic numbers = new ExpandoObject();
                    dynamic idS = new ExpandoObject();
                    putFaultStaff.Fault_ID = putFault.FaultLog.Fault_ID;
                    putFaultStaff.Staff_ID = staff.Staff_ID;
                    putFaultStaff.FaultStaff_Id = 0;
                    numbers.Staff_Phone_Number = staff.Staff_Phone_Number;
                    staffNumbers.Add(numbers);
                    idS.Staff_ID = staff.Staff_ID;
                    staffIDs.Add(idS);
                    db.Fault_Staff.Add(putFaultStaff);
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

                var auditQuery = from faultPut in db.Fault_Log
                                 join sec in db.Sections on faultPut.Section_ID equals sec.Section_ID
                                 join farms in db.Farms on sec.Farm_ID equals farms.Farm_ID
                                 join userPos in db.Farm_User_User_Position on farms.Farm_ID equals userPos.Farm_ID
                                 join farmUser in db.Farm_User on userPos.Farm_User_ID equals farmUser.Farm_User_ID

                                 where fault.Section_ID == sec.Section_ID
                                 select new
                                 {
                                     Farm_ID = farms.Farm_ID,
                                     User_ID = farmUser.User_ID
                                 };
                var auditDetails = auditQuery.ToList().FirstOrDefault();
                Audit_Trail A_Log = new Audit_Trail();
                A_Log.Farm_ID = auditDetails.Farm_ID;
                A_Log.User_ID = auditDetails.User_ID;
                A_Log.Affected_ID = fault.Fault_ID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Updated Fault";
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
        [Route("api/Fault/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            List<dynamic> staffNumbers = new List<dynamic>();
            List<dynamic> staffIDs = new List<dynamic>();
            try
            {
                Fault_Log fault = db.Fault_Log.Where(x => x.Fault_ID == id).FirstOrDefault();

                if (fault.Status_ID == 1)
                {
                    foreach (Fault_Staff faultStaff in db.Fault_Staff)
                    {
                        if (faultStaff.Fault_ID == id)
                        {

                            dynamic numbers = new ExpandoObject();
                            dynamic idS = new ExpandoObject();
                            numbers.Staff_Phone_Number = faultStaff.Staff.Staff_Phone_Number;
                            staffNumbers.Add(numbers);
                            idS.Staff_ID = faultStaff.Staff_ID;
                            staffIDs.Add(idS);
                            db.Fault_Staff.Remove(faultStaff);
                        }
                    }

                    var faultType = (from fT in db.Fault_Type
                                     where fT.FT_ID == fault.FT_ID
                                     select fT.FT_Description).ToList();

                    string type = Convert.ToString(faultType[0]);
                    //string start = Convert.ToString(newFault.FaultLog.Fault_Start_Date);
                    string start = fault.Fault_Start_Date.ToString();
                    //string end = Convert.ToString(newFault.FaultLog.Fault_End_Date);
                    string end = fault.Fault_End_Date.ToString();

                    string detailsMessage = "Fault DELETED!" + "\nDescription: " + fault.Fault_Description + "\nFault Type: " + type + "\nStart: " + start + "\nEnd: " + end + ".";

                    db.Fault_Log.Remove(fault);
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

                                     where fault.Section_ID == sec.Section_ID
                                     select new
                                     {
                                         Farm_ID = farms.Farm_ID,
                                         User_ID = farmUser.User_ID
                                     };
                    var auditDetails = auditQuery.ToList().FirstOrDefault();
                    Audit_Trail A_Log = new Audit_Trail();
                    A_Log.Farm_ID = auditDetails.Farm_ID;
                    A_Log.User_ID = auditDetails.User_ID;
                    A_Log.Affected_ID = fault.Fault_ID;
                    A_Log.Action_DateTime = DateTime.Now;
                    A_Log.User_Action = "Delete";
                    db.Audit_Trail.Add(A_Log);
                    db.SaveChanges();

                    return Content(HttpStatusCode.OK, "rows affected");
                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "Fault complete");
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



    }
}

public class FaultStaffClass
{
    public Fault_Log FaultLog { get; set; }
    public List<Staff> Staff { get; set; }
}
public class StaffNumber
{
    public int staffID { get; set; }
    public string number { get; set; }
}