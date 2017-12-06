using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Northwind.Models;
using Northwind.Security;
using System.Web.Security;
using System.Net;
using Northwind.Mapper;

namespace Northwind.Controllers
{
    public class CustomerController : Northwind.Configuration.ControllerBase
    {
        // GET: Customer/Account
        [Authorize]
        public ActionResult Account()
        {
            if (Request.Cookies["role"].Value != "customer")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                Customer customer = db.Customers.Find(UserAccount.GetUserID());
                CustomerEdit ce = Mapper.Map<CustomerEdit>(customer);
                return View(ce);
            }


            //ViewBag.CustomerID = UserAccount.GetUserID();

        }
        // POST: Customer/Account
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Account(CustomerEdit updatedCust)
        {
            if (Request.Cookies["role"].Value != "customer")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                if (ModelState.IsValid)
                {
                    Customer customer = db.Customers.Find(UserAccount.GetUserID());
                    if (customer.CompanyName.ToLower() != updatedCust.CompanyName.ToLower())
                    {
                        // Ensure that the CompanyName is unique
                        if (db.Customers.Any(c => c.CompanyName == updatedCust.CompanyName))
                        {
                            // duplicate CompanyName
                            ModelState.AddModelError("CompanyName", "There is already a company named that");
                            return View(updatedCust);
                        }
                    }
                    Mapper.Map(updatedCust, customer);
                    db.SaveChanges();
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                //Validation Problem
                return View(updatedCust);
            }
        }
        // GET: Customer/Register
        public ActionResult Register()
        {
            return View();
        }
        // POST: Customer/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(CustomerRegister customerRegister)
        {
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                if (ModelState.IsValid)
                {
                    Customer customer = Mapper.Map<Customer>(customerRegister);
                    //Check for duplicate Registration (Name already in DB)
                    if (db.Customers.Any(c => c.CompanyName == customer.CompanyName))
                    {
                        ModelState.AddModelError("CompanyName", "There is already a company named that");
                        return View();
                    }

                    // Generate guid for this customer
                    customer.UserGuid = System.Guid.NewGuid();
                    // Generate hash (with guid + pass)
                    customer.Password = UserAccount.HashSHA1(customer.Password + customer.UserGuid);

                    //Add Customer and Save Changes
                    db.Customers.Add(customer);
                    db.SaveChanges();
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                return View();
            }
        }
        // GET: Customer/SignIn
        public ActionResult SignIn()
        {
            using (var db = new NORTHWNDEntities())
            {
                ViewBag.CustomerID = new SelectList(db.Customers.OrderBy(c => c.CompanyName),
                    "CustomerID", "CompanyName").ToList();
            }
                return View();
        }
        // POST: Customer/SignIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(CustomerSignIn customerSignIn, string ReturnUrl)
        {
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                if (ModelState.IsValid)
                {
                    // find customer by CustomerId
                    Customer customer = db.Customers.Find(customerSignIn.CustomerId);
                    // hash & salt the posted password
                    string hash = UserAccount.HashSHA1(customerSignIn.Password + customer.UserGuid);
                    // Compared posted Password to customer password
                    if (hash == customer.Password)
                    {
                        // Passwords match
                        // authenticate user (this stores the CustomerID in an encrypted cookie)
                        // normally, you would require HTTPS
                        FormsAuthentication.SetAuthCookie(customer.CustomerID.ToString(), false);

                        HttpCookie myCookie = new HttpCookie("role");
                        myCookie.Value = "customer";
                        Response.Cookies.Add(myCookie);

                        if (ReturnUrl != null)
                        {
                            return Redirect(ReturnUrl);
                        }
                        // Redirect to Home page
                        return RedirectToAction(actionName: "Index", controllerName: "Home");
                    }
                    else
                    {
                        // Passwords do not match
                        ModelState.AddModelError("Password", "Incorrect Password");
                    }
                }
                // create drop-down list box for company name
                ViewBag.CustomerID = new SelectList(db.Customers.OrderBy(c => c.CompanyName),
                "CustomerID", "CompanyName").ToList();
                return View();
                
            }
        }
    }
}