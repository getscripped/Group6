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

namespace AgriLogBackend.Controllers
{
    public class AccessController : ApiController
    {
        // GET: Access
        private string _key = "GuessThePasswordToThisSiteAndGetACookieFromMeOrIGetACookieFromYou";


        private AgriLogDBEntities db = new AgriLogDBEntities();


        [HttpPost]
        [Route("api/User/Register")]
        public IHttpActionResult Register(UserRegister user)
        {
            var finduser = db.Users.Where(x => x.User_Email == user.Email).FirstOrDefault();
            if (finduser == null)
            {
                user.Password = encrypt(user.Password);
                User addUser = new User();
                addUser.User_Email = user.Email;
                addUser.User_Password = user.Password;
                addUser.Is_Active = "1";


                Farm_User faddUser = new Farm_User();
                faddUser.Farm_User_ID = user.IDNum;
                faddUser.Farm_User_Name = user.Name;
                faddUser.Farm_User_Surname = user.Surname;
                faddUser.Farm_User_DOB = user.DOB;
                faddUser.Farm_User_Phone_Number = user.Phone;
                faddUser.Farm_User_Address = user.Adress;
                faddUser.Farm_User_Image = user.Image;

                db.Users.Add(addUser);
                db.SaveChanges();

                var id = db.Users.Where(x => x.User_Email == user.Email).FirstOrDefault().User_ID;

                faddUser.User_ID = id;

                db.Farm_User.Add(faddUser);
                db.SaveChanges();




                return Ok("1 row affected");
            }
            else
            {
                return Unauthorized();
            }



        }



        [HttpPost]
        [Route("api/UserExists")]
        public IHttpActionResult UserExists(HttpRequestMessage ID)
        {

            var someText = ID.Content.ReadAsStringAsync().Result;

            var ID1 = db.Farm_User.Where(x => x.Farm_User_ID == someText).FirstOrDefault();
            var ID2 = db.Users.Where(x => x.User_Email == someText).FirstOrDefault();

            if (ID1 != null || ID2 != null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok();
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("api/User/InsuranceRegister")]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult InsuranceRegister(InsuranceRegister IPUser)
        {
            var finduser = db.Users.Where(x => x.User_Email == IPUser.User_Email).FirstOrDefault();
            if (finduser == null)
            {
                IPUser.User_Password = encrypt(IPUser.User_Password);
                User addUser = new User();
                addUser.User_Email = IPUser.User_Email;
                addUser.User_Password = IPUser.User_Password;
                addUser.Is_Active = "1";

                Insurance_Provider faddIP = new Insurance_Provider();
                faddIP.IP_Name = IPUser.IP_Name;
                faddIP.IP_Phone_Number = IPUser.IP_Phone_Number;
                faddIP.IP_Reg_Number = IPUser.IP_Reg_Number;
                faddIP.IP_VAT_Number = IPUser.IP_VAT_Number;

                db.Users.Add(addUser);
                db.SaveChanges();

                var id = db.Users.Where(x => x.User_Email == IPUser.User_Email).FirstOrDefault().User_ID;

                faddIP.User_ID = id;

                db.Insurance_Provider.Add(faddIP);
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
                A_Log.Affected_ID = IPUser.IP_ID;
                A_Log.Action_DateTime = DateTime.Now;
                A_Log.User_Action = "Add new Insurance Provider";
                db.Audit_Trail.Add(A_Log);
                //db.SaveChanges();

                return Ok("1 row affected");
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "lol nope");
            }
        }


        [HttpPost]
        [Route("api/InsuranceUserExists")]
        public IHttpActionResult InsuranceUserExists(HttpRequestMessage ID)
        {

            var someText = ID.Content.ReadAsStringAsync().Result;

            var ID1 = db.Insurance_Provider.Where(x => x.IP_ID.ToString() == someText).FirstOrDefault();
            var ID2 = db.Users.Where(x => x.User_Email == someText).FirstOrDefault();

            if (ID1 != null || ID2 != null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok();
            }
        }



        [HttpPost]
        [Route("api/User/Login")]
        public IHttpActionResult Login(User user)
        {
            var finduser = db.Users.Where(x => x.User_Email == user.User_Email).FirstOrDefault();

            var encPass = encrypt(user.User_Password);
            if (finduser != null && finduser.User_Password == encPass)
            {

                var claims = new[]
                {
                   new Claim(ClaimTypes.Name,finduser.User_ID.ToString())



                };
                var keytoReturn = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

                var Credentials = new SigningCredentials(keytoReturn, SecurityAlgorithms.HmacSha512Signature);
                var descriptorToken = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = Credentials
                };
                var Handler = new JwtSecurityTokenHandler();

                var userToken = Handler.CreateToken(descriptorToken);
                return Ok
                (new
                {
                    Token = Handler.WriteToken(userToken)
                }
                );

            }

            return Unauthorized();

        }


        [HttpGet]
        [Route("api/User/GetFarms(id)")]
        public IHttpActionResult GetFarms(string id)
        {
            var farmsQuery = from userPos in db.Farm_User_User_Position
                             join positions in db.User_Position on userPos.User_Type_ID equals positions.User_Type_ID
                             join farm in db.Farms on userPos.Farm_ID equals farm.Farm_ID
                             where userPos.Farm_User_ID == id
                             select new { UserID = userPos.Farm_User_ID, UserType = positions.User_Type_Description, FarmID = userPos.Farm_ID, FarmName = farm.Farm_Name };

            List<dynamic> results = farmsQuery.ToList<dynamic>();


            if (results.Count == 0)
            {
                return Ok("No Farms Found");
            }
            else
            {
                return Ok(results);
            }


        }


        [HttpGet]
        [Route("api/addForeman/{User_ID}/{Farm_ID}")]
        public IHttpActionResult addToFarm(int User_ID, int Farm_ID)
        {
            var check = from farm in db.Farms
                        join user_Pos in db.Farm_User_User_Position on farm.Farm_ID equals user_Pos.Farm_ID
                        join users in db.Farm_User on user_Pos.Farm_User_ID equals users.Farm_User_ID
                        join mainuser in db.Users on users.User_ID equals mainuser.User_ID
                        where mainuser.User_ID == User_ID
                        select new
                        {
                            User_ID = mainuser.User_ID,
                            Farm_User_ID = users.Farm_User_ID,
                            Farm_ID = farm.Farm_ID

                        };



            try
            {
                dynamic record = check.ToList().FirstOrDefault();
                if (record == null)
                {
                    var query = from users in db.Users
                                join farmUsers in db.Farm_User on users.User_ID equals farmUsers.User_ID
                                where users.User_ID == User_ID
                                select new
                                {
                                    Farm_User_ID = farmUsers.Farm_User_ID
                                };

                    Farm_User_User_Position toAdd = new Farm_User_User_Position();
                    toAdd.Farm_User_ID = query.ToList().FirstOrDefault().Farm_User_ID;
                    toAdd.Farm_ID = Farm_ID;
                    toAdd.User_Type_ID = 2;

                    db.Farm_User_User_Position.Add(toAdd);
                    db.SaveChanges();
                    return Content(HttpStatusCode.OK, "User succesfully added");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "Farm already has a Foreman, please notify the user adding you to the farm");
                }
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
    
            }


        }

        [HttpGet]
        [Route("api/access/checkfarms/{UserID}")]
        public IHttpActionResult getFarms(int UserID)
        {
            var query = from user in db.Farm_User_User_Position
                        join farms in db.Farms on user.Farm_ID equals farms.Farm_ID
                        join relation in db.Farm_User on user.Farm_User_ID equals relation.Farm_User_ID
                        join users in db.Users on relation.User_ID equals users.User_ID
                        where users.User_ID == UserID
                        select new
                        {
                            Farm_ID = farms.Farm_ID,
                            Farm_Name = farms.Farm_Name
                        };
            try
            {
                var returnObj = query.ToList();

                return Content(HttpStatusCode.OK, returnObj);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "an error occured");

            }


        }


        [HttpGet]
        [Route("api/Acess/HasFarm/{User_ID}")]
        public IHttpActionResult hasFarm(int User_ID)
        {
            var query = from users in db.Users
                        join farmUser in db.Farm_User on users.User_ID equals farmUser.User_ID
                        join userPos in db.Farm_User_User_Position on farmUser.Farm_User_ID equals userPos.Farm_User_ID
                        where users.User_ID == User_ID
                        select new
                        {
                            Farm_ID = userPos.Farm_ID
                        };

            try
            {
                var farms = query.ToList();
                if (farms.Count > 0)
                {
                    return Content(HttpStatusCode.OK, "farms found");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "No farm found");
                }
            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.BadRequest, "an error occured");
            }

        }

        [HttpGet]
        [Route("api/Acess/HasSection/{Farm_ID}")]
        public IHttpActionResult hasSection(int Farm_ID)
        {
            var query = from users in db.Users
                        join farmUser in db.Farm_User on users.User_ID equals farmUser.User_ID
                        join userPos in db.Farm_User_User_Position on farmUser.Farm_User_ID equals userPos.Farm_User_ID
                        join farms in db.Farms on userPos.Farm_ID equals farms.Farm_ID
                        join sections in db.Sections on farms.Farm_ID equals sections.Farm_ID
                        where farms.Farm_ID == Farm_ID
                        select new
                        {
                            Section_ID = sections.Section_ID
                        };

            try
            {
                var sections = query.ToList();
                if (sections.Count > 0)
                {
                    return Content(HttpStatusCode.OK, "farms found");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "No farm found");
                }
            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.BadRequest, "an error occured");
            }

        }

        [HttpGet]
        [Route("api/Acess/HasInfra/{Farm_ID}")]
        public IHttpActionResult hasInfra(int Farm_ID)
        {
            var query = from users in db.Users
                        join farmUser in db.Farm_User on users.User_ID equals farmUser.User_ID
                        join userPos in db.Farm_User_User_Position on farmUser.Farm_User_ID equals userPos.Farm_User_ID
                        join farms in db.Farms on userPos.Farm_ID equals farms.Farm_ID
                        join sections in db.Sections on farms.Farm_ID equals sections.Farm_ID
                        join infras in db.Infrastructures on sections.Section_ID equals infras.Section_ID
                        where farms.Farm_ID == Farm_ID
                        select new
                        {
                            Infrastructure_ID = infras.Infrastructure_ID
                        };

            try
            {
                var infrastructures = query.ToList();
                if (infrastructures.Count > 0)
                {
                    return Content(HttpStatusCode.OK, "farms found");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "No farm found");
                }
            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.BadRequest, "an error occured");
            }

        }

        private string encrypt(string Pass)
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