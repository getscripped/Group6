using AgriLogBackend.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AgriLogBackend.Controllers
{
    [Authorize]

    public class VehicleController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();

        //================================Get all vehicles=============================
        [HttpGet]
        [Route("api/Vehicle/{farmID}")]
        public IHttpActionResult GetVehicle(int farmID)
        {
            var query = from veh in db.Vehicles  //<<<<<<<<<< query setup
                        join sec in db.Sections on veh.Section_ID equals sec.Section_ID
                       
                        join brand in db.Vehicle_Brand on veh.Vehicle_Brand_ID equals brand.Vehicle_Brand_Id
                        join model in db.Vehicle_Model on brand.Model_ID equals model.Model_Id
                        join make in db.Vehicle_Make on brand.Make_ID equals make.Make_ID
                        join type in db.Vehicle_Type on veh.Vehicle_Type_ID equals type.Vehicle_Type_ID
                        where veh.Farm_ID == farmID
                        select new
                        {
                            Vehicle_ID = veh.Vehicle_ID,
                            Vehicle_Type_Description = type.Vehicle_Type_Description,
                            Model_Description = model.Model_Description,
                            Section_Name = sec.Section_Name,
                            Infrastructure_Name=veh.Infrastructure.Infrastructure_Name,
                            Vehicle_Mileage_Date_Of_Purchase = veh.Vehicle_Mileage_Date_Of_Purchase,
                            Vehicle_Mileage_Current = veh.Vehicle_Mileage_Current,
                            Vehicle_Service_Intervals = veh.Vehicle_Service_Intervals,
                            Vehicle_Last_Service_Mileage = veh.Vehicle_Last_Service_Mileage,
                            Vehicle_Registration_Number = veh.Vehicle_Registration_Number,
                            Vehicle_Date_Of_Purchase = veh.Vehicle_Date_Of_Purchase,
                            Is_Active = veh.Is_Active

                        };

            List<dynamic> vehicles = new List<dynamic>();
            try
            {
                vehicles = query.ToList<dynamic>();  // << excecute and handle query
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "No vehicles found"); // <<< database Error 
               
            }

            if (vehicles.Count() > 0) // <<< Check if list is empty
            {
                return Content(HttpStatusCode.OK, vehicles);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "No vehicles found");
            }


        }

        //====================================Get specific vehicle details===============
        [HttpGet]
        [Route("api/VehicleDetails/{id}")]
        public IHttpActionResult GetVehicleDetails(int id)
        {
            dynamic toReturn = new ExpandoObject();
            try
            {
                var query = from vehicles in db.Vehicles
                            join brands in db.Vehicle_Brand on vehicles.Vehicle_Brand_ID equals brands.Vehicle_Brand_Id
                            where vehicles.Vehicle_ID == id
                            select new
                            {
                                Vehicle_Type_ID = vehicles.Vehicle_Type_ID,
                                Make_ID = brands.Make_ID,
                                Model_ID = brands.Model_ID,
                                Infrastructure_ID = vehicles.Infrastructure_ID,
                                Section_ID = vehicles.Section_ID,
                                Vehicle_Year = vehicles.Vehicle_Year,
                                Vehicle_Colour = vehicles.Vehicle_Colour,
                                Vehicle_Mileage_Date_Of_Purchase = vehicles.Vehicle_Mileage_Date_Of_Purchase,
                                Vehicle_Mileage_Current = vehicles.Vehicle_Mileage_Current,
                                Vehicle_Service_Intervals = vehicles.Vehicle_Service_Intervals,
                                Vehicle_Last_Service_Mileage = vehicles.Vehicle_Last_Service_Mileage,
                                Vehicle_Registration_Number = vehicles.Vehicle_Registration_Number,
                                Vehicle_Date_Of_Purchase = vehicles.Vehicle_Date_Of_Purchase,
                                Vehicle_Purchase_Price = vehicles.Vehicle_Purchase_Price,
                                Is_Active = vehicles.Is_Active

                            };

                toReturn = query.ToList().FirstOrDefault();
            }
            catch (Exception)
            {

                return Content(HttpStatusCode.BadRequest, "Error");
            }
            if (toReturn != null)
            {
                return Content(HttpStatusCode.OK, toReturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "No vehicle found");
            }


        }

        [HttpPost]
        [Route("api/Vehicle/Add/{farmID}")]
        public IHttpActionResult AddVehicle(int farmID, Vehicle newVehicle)
        {
            if (newVehicle != null)
            {
                try
                {
                    newVehicle.Farm_ID = farmID;
                    newVehicle.Vehicle_Brand_ID = 2;
                    db.Vehicles.Add(newVehicle);
                    db.SaveChanges();

                }
                catch (Exception e)
                {

                    return Content(HttpStatusCode.BadRequest, "Error");
                }
                return Content(HttpStatusCode.OK, "1 row affected");
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Cant pass a null value");
            }

        }



        [HttpPut]
        [Route("api/Vehicle/put/{ID}")]
        public IHttpActionResult PutVehicle(int ID, Vehicle putVehicle)
        {
            if (putVehicle != null)
            {
                try
                {
                    Vehicle vehicle = db.Vehicles.Where(x => x.Vehicle_ID == ID).FirstOrDefault();

                    vehicle.Infrastructure_ID = putVehicle.Infrastructure_ID;
                    vehicle.Is_Active = putVehicle.Is_Active;
                    vehicle.Section_ID = putVehicle.Section_ID;
                    vehicle.Vehicle_Colour = putVehicle.Vehicle_Colour;
                    vehicle.Vehicle_Date_Of_Purchase = putVehicle.Vehicle_Date_Of_Purchase;
                    vehicle.Vehicle_Last_Service_Mileage = putVehicle.Vehicle_Last_Service_Mileage;
                    vehicle.Vehicle_Mileage_Current = putVehicle.Vehicle_Mileage_Current;
                    vehicle.Vehicle_Mileage_Date_Of_Purchase = putVehicle.Vehicle_Mileage_Date_Of_Purchase;
                    vehicle.Vehicle_Purchase_Price = putVehicle.Vehicle_Purchase_Price;
                    vehicle.Vehicle_Registration_Number = putVehicle.Vehicle_Registration_Number;
                    vehicle.Vehicle_Type_ID = putVehicle.Vehicle_Type_ID;

                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return Content(HttpStatusCode.BadRequest, "Db error");
                    
                }

                return Content(HttpStatusCode.OK, "1 row affected");
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Cant pass null");
            }


        }


        [HttpDelete]
        [Route("api/Vehicle/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                Vehicle vehicle = db.Vehicles.Where(x => x.Vehicle_ID == id).FirstOrDefault();
                db.Vehicles.Remove(vehicle);
                db.SaveChanges();
                return Content(HttpStatusCode.OK, "1 row affected");
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
               
            }


        }





        //====================================================================================TYPES======================================================================




        [HttpGet]
        [Route("api/VehicleType/{farmID}")]
        public IHttpActionResult GetVehicleType(int farmID)
        {
            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from types in db.Vehicle_Type where types.Farm_ID == farmID select new { Vehicle_Type_ID = types.Vehicle_Type_ID, Vehicle_Type_Description = types.Vehicle_Type_Description };
                toreturn = query.ToList<dynamic>();
            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.BadRequest, "Db error");
            }

            if (toreturn.Count() > 0)
            {
                return Content(HttpStatusCode.OK, toreturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "No types found");
            }

        }


        [HttpGet]
        [Route("api/VehicleTypeDetails/{id}")]
        public IHttpActionResult GetVehicleTypeDetails(int id)
        {
            dynamic type2 = new ExpandoObject();
            try
            {
                var type = from typ in db.Vehicle_Type where typ.Vehicle_Type_ID == id select new { Vehicle_Type_ID = typ.Vehicle_Type_ID, Vehicle_Type_Description = typ.Vehicle_Type_Description };

                type2 = type.ToList<dynamic>().FirstOrDefault();


            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            
            }
            if (type2 != null)
            {
                return Content(HttpStatusCode.OK, type2);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Notfound");
            }

        }

        [HttpPost]
        [Route("api/VehicleType/Add/{farmID}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult AddVehicleType(int farmID, HttpRequestMessage typeDesc)
        {

            if (typeDesc != null)
            {
                try
                {
                    Vehicle_Type newType = new Vehicle_Type();
                    newType.Vehicle_Type_Description = typeDesc.Content.ReadAsStringAsync().Result;
                    newType.Farm_ID = farmID;
                    db.Vehicle_Type.Add(newType);
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



        [HttpPut]
        [Route("api/VehicleType/put/{id}")]
        public IHttpActionResult PutVehicleType(int id, HttpRequestMessage typeDesc)
        {

            try
            {
                Vehicle_Type editable = db.Vehicle_Type.Where(x => x.Vehicle_Type_ID == id).FirstOrDefault(); // << find equipment types

                editable.Vehicle_Type_Description = typeDesc.Content.ReadAsStringAsync().Result; // <<< Edit data

                db.SaveChangesAsync();  // <<< save data
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Edit failed");  // <<< database save failed
                
            }

            return Content(HttpStatusCode.OK, "1 row affected"); // <<< success
        }


        [HttpGet]
        [Route("api/Vehicle/getInfras/{farmID}")]
        public IHttpActionResult getInfrastructures(int farmID)
        {
            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from infras in db.Infrastructures
                            join sections in db.Sections on infras.Section_ID equals sections.Section_ID
                            join farm in db.Farms on sections.Farm_ID equals farm.Farm_ID
                            where farm.Farm_ID == farmID
                            select new { Infrastructure_ID = infras.Infrastructure_ID, Infrastructure_Name = infras.Infrastructure_Name };
                toreturn = query.ToList<dynamic>();
            }
            catch (Exception)
            {

                return Content(HttpStatusCode.BadRequest, "Db error");
            }

            if (toreturn.Count() > 0)
            {
                return Content(HttpStatusCode.OK, toreturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "No types found");
            }


        }

        [HttpGet]
        [Route("api/Vehicle/getSections/{farmID}")]
        public IHttpActionResult getSections(int farmID)
        {

            List<dynamic> toreturn = new List<dynamic>();

            try
            {
                var query = from sections in db.Sections
                            join farm in db.Farms on sections.Farm_ID equals farm.Farm_ID
                            where farm.Farm_ID == farmID
                            select new { Section_ID = sections.Section_ID, Section_Name = sections.Section_Name };
                toreturn = query.ToList<dynamic>();
            }
            catch (Exception)
            {

                return Content(HttpStatusCode.BadRequest, "Db error");
            }

            if (toreturn.Count() > 0)
            {
                return Content(HttpStatusCode.OK, toreturn);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "No types found");
            }
        }


        [HttpGet]
        [Route("api/Vehicle/getBrands")]
        public IHttpActionResult getBrands()
        {
            List<dynamic> Makes = new List<dynamic>();
            List<dynamic> Models = new List<dynamic>();
            try
            {
                var query1 = from makes in db.Vehicle_Make select new { Make_ID = makes.Make_ID, Make_Description = makes.Make_Description };
                var query2 = from models in db.Vehicle_Model select new { Model_Id = models.Model_Id, Model_Description = models.Model_Description };

                Makes = query1.ToList<dynamic>();
                Models = query2.ToList<dynamic>();
            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.BadRequest, "Error");
            }

            if (Makes.Count() > 0 && Models.Count() > 0)
            {
                dynamic returnOBJ = new ExpandoObject();
                returnOBJ.Makes = Makes;
                returnOBJ.Models = Models;
                return Content(HttpStatusCode.OK, returnOBJ);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }
        }


        [HttpGet]
        [Route("api/Vehicle/updateKM/{Vehicle_ID}/{KM}")]
        public IHttpActionResult UpdateKM(int Vehicle_ID,string KM)
        {
            try
            {
                var vehicle = db.Vehicles.Where(x=>x.Vehicle_ID == Vehicle_ID).FirstOrDefault();
                vehicle.Vehicle_Mileage_Current = KM;

                db.SaveChanges();
                return Content(HttpStatusCode.OK, "Mileage updated");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Failed");

            }
            return Ok();
        }



    }
}
