using PayEmFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayEmFinal.Controllers
{
    public class LoginController : Controller
    {
        dbPayEmEntities context = new dbPayEmEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Customer customer)
        {
            try
            {
                var user = context.Customers.Single(FetchUserDetails => FetchUserDetails.Email == customer.Email && FetchUserDetails.Password == customer.Password);
                if (user != null)
                {
                    if (user.UserType == "user")
                    {
                        Session["PhoneNumber"] = user.PhoneNumber;
                        TempData["PhoneNumber"] = user.PhoneNumber.ToString();
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Invalid Username or Password.";
            }
            return View();
        }
    }
}