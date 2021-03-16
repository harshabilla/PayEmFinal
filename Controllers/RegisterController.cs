using PayEmFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayEmFinal.Controllers
{
    public class RegisterController : Controller
    {
        dbPayEmEntities context = new dbPayEmEntities();
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult DoesPhoneNumberExists(Customer customer)
        {
            return Json(!context.Customers.Any(user => user.PhoneNumber == customer.PhoneNumber), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CustomerRegistration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CustomerRegistration(Customer customer)
        {
            if (ModelState.IsValid)
            {
                TempData["CustomerPhoneNumber"] = customer.PhoneNumber;
                context.Customers.Add(customer);
                customer.UserType = "user";
                context.SaveChanges();
                GenerateUPI();
                ViewBag.Message = "Account created successfully.";
            }
            return View();
        }

        public void GenerateUPI()
        {
            string UPIid;
            string PhoneNumberOfNewUser = (string)TempData.Peek("CustomerPhoneNumber");
            UPIid = PhoneNumberOfNewUser + "@payem";
            //var UpdatingUPI = context.UPIs.Single(FetchDetails => FetchDetails.PhoneNumber == PhoneNumberOfNewUser);
            context.UPIs.Add( new UPI (UPIid, " ", PhoneNumberOfNewUser));
            context.SaveChanges();
        }
    }
}