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
using System.Text;
using System.IO;

namespace New_API_370.Controllers
{
    public class TestMessageController : ApiController
    {
        private AgriLogDBEntities db = new AgriLogDBEntities();



        [HttpPost]
        [Route("api/SendSMS")]
        public IHttpActionResult SendSMS([FromBody] MessageClass send)
        {
            // This URL is used for sending messages
            string myURI = "https://api.bulksms.com/v1/messages";

            // change these values to match your own account
            string myUsername = "agritech";
            string myPassword = "Farm5pac3";


            //Get number and message from json object
            string sendto = send.to;
            string sendbody = send.body;

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

            return Content(HttpStatusCode.OK, "SMS sent successfully!");

        }

    }
}

public class MessageClass
{
    public string body { get; set; }
    public string to { get; set; }
}
