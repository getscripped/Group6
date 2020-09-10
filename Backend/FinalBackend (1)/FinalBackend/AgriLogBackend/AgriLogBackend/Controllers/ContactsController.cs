using AgriLogBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;

namespace AgriLogBackend.Controllers
{
    public class ContactsController : ApiController
    {
        /*
        private AgriLogDBEntities db = new AgriLogDBEntities();
        [HttpPost]
        [Route("api/Contacts/Foreman/{FarmID}")]
        public IHttpActionResult postForeman(int FarmID, HttpRequestMessage email)
        {
            var emails = email.Content.ReadAsStringAsync().Result;
            var check = db.Users.Where(e => e.User_Email == emails).FirstOrDefault();
            var UserID = 0;
            if (check == null)
            {
                return Unauthorized();
            }
            else
            {
                UserID = check.User_ID;
            }

            var link = "http://35.178.156.37/api/addForeman/" + UserID.ToString() + "/" + FarmID.ToString();

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");


                mail.From = new MailAddress("agrilognotifications@gmail.com");
                mail.To.Add(emails);
                mail.Subject = "You have been added to a farm!";
                mail.Body = FarmID.ToString() + " Has added you as a foreman to their farm! Please click on the   following link to add your profile to the farm! " + link;
                ;


                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("agrilognotifications@gmail.com", "AgriLog321");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
                return Content(HttpStatusCode.OK, "Email sent");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Message failed");
            }


        }
        */
    }
}
