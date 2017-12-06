using Northwind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Northwind.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddToCart(CartDTO cartDto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            //CREATE CART
            Cart sc = new Cart();
            sc.ProductID = cartDto.ProductID;
            sc.CustomerID = cartDto.CustomerID;
            sc.Quantity = cartDto.Quantity;
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                if (db.Carts.Where(c => c.ProductID == sc.ProductID && c.CustomerID == sc.CustomerID).Any())
                {
                    //UPDATE 
                    Cart cart = db.Carts.FirstOrDefault(c => c.ProductID == sc.ProductID && c.CustomerID == sc.CustomerID);

                    cart.Quantity += sc.Quantity;
                }
                else
                {
                    db.Carts.Add(sc);
                }
                db.SaveChanges();
            }
                return Json(sc, JsonRequestBehavior.AllowGet);
        }
    }
}