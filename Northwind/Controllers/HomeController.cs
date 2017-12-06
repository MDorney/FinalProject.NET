using Northwind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Northwind.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new NORTHWNDEntities())
            {
                DateTime now = DateTime.Now;
                return View(db.Discounts.Where(q => q.StartTime <= now && q.EndTime > now).OrderByDescending(q => q.DiscountPercent).ToList().Take(3));
            }
                
        }
        // GET: Home/SignOut
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction(actionName: "SignedOut", controllerName: "Home");
        }
        // GET: Home/SignedOut
        public ActionResult SignedOut()
        {
            return View();
        }
    }
}