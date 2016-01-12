using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prairie_path.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View(new ContactInfo());
        }
        [HttpPost]
        public ActionResult Contact(ContactInfo contact)
        {

            var fullName = contact.FullName;
            var contactEmail = contact.ContactEmail;
            var contactMessage = contact.ContactMessage;
            SendEmail(fullName, contactEmail, contactMessage);
            return RedirectToAction("Index");
        }

        public ActionResult ContactSubmited()
        {

            var result = new FilePathResult("~/Views/Home/Index.html", "text/html");
            return result;

        }


        public void SendEmail(string fullName, string contactEmail, string contactMessage)
        {
            string mailgunKey = System.Configuration.ConfigurationManager.AppSettings["MAILGUN_API_KEY"];
            if (String.IsNullOrEmpty(mailgunKey))
            {
                throw new Exception("Mailgun api key missing");
            }
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               mailgunKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 System.Configuration.ConfigurationManager.AppSettings["MAILGUN_DOMAIN"],
                                 ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Tosca Ragnini - inquiry <mailgun@mailgun.org>");
            request.AddParameter("to", "tosca.ragnini@gmail.com");
            request.AddParameter("subject", "Prairie Path - Contact Request");
            request.AddParameter("text", "FullName : " + fullName);
            request.AddParameter("text", "ContactEmail : " + contactEmail);
            request.AddParameter("text", "ContactMessage : " + contactMessage);
            request.Method = Method.POST;
            var result = client.Execute(request);
            return;
        }


    }


    public class ContactInfo
    {
        public string FullName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactMessage { get; set; }

    }

}

