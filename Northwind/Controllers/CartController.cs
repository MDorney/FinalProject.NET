using Northwind.Models;
using Northwind.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        [Authorize]
        public ActionResult ConfirmCart(FormCollection Form)
        {
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                Customer cust = db.Customers.Find(UserAccount.GetUserID());
                List<Cart> carts = db.Carts.Where(c => c.CustomerID == cust.CustomerID).ToList<Cart>();
                Order o = new Order();
                o.CustomerID = cust.CustomerID;
                o.OrderDate = DateTime.Now;
                o.ShipName = cust.CompanyName;
                o.ShipAddress = cust.Address;
                o.ShipCity = cust.City;
                o.ShipRegion = cust.Region;
                o.ShipPostalCode = cust.PostalCode;
                o.ShipCountry = cust.Country;
                Order newOrder = db.Orders.Add(o);
                db.SaveChanges();
                foreach (Cart c in carts)
                {
                    Order_Detail lineItem = new Order_Detail();
                    lineItem.OrderID = newOrder.OrderID;
                    lineItem.ProductID = (int)c.ProductID;
                    lineItem.UnitPrice = (decimal)db.Products.Find(c.ProductID).UnitPrice;
                    lineItem.Quantity = (short)c.Quantity;
                    string discount = Form["discount_" + c.ProductID];
                    if (string.IsNullOrEmpty(discount))
                    {
                        lineItem.Discount = 0.0M;
                    } else
                    {
                        Discount toTest = db.Discounts.FirstOrDefault(d => d.Code.Equals(discount));
                        if (toTest.ProductID == lineItem.ProductID && toTest.StartTime < DateTime.Now && toTest.EndTime > DateTime.Now)
                        {
                            lineItem.Discount = (decimal)toTest.DiscountPercent;
                        }
                    }
                    RemoveFromCartAfterOrder(c.CartID);
                }
                db.SaveChanges();
            }
            return View("Ordered");
        }
        [HttpPost]
        public void RemoveFromCart(int id)
        {
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                Cart c = db.Carts.Find(id);
                db.Carts.Remove(c);
            }
        }
        private void RemoveFromCartAfterOrder(int id)
        {
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                Cart c = db.Carts.Find(id);
                db.Carts.Remove(c);
                db.SaveChanges();
            }
        }
        [Authorize]
        [HttpGet]
        public JsonResult CartRefresh()
        {
            int custID = UserAccount.GetUserID();
            
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                //Customer cust = db.Customers.Find(UserAccount.GetUserID());
                var carts = (from d in db.Carts.Where(c => c.CustomerID == custID)
                                   select new
                                   {
                                       d.ProductID,
                                       d.Quantity
                                   }).ToList();
                return Json(carts, JsonRequestBehavior.AllowGet);
            }
            
        }
        public ActionResult Cart()
        {
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                Customer customer = db.Customers.Find(UserAccount.GetUserID());

                List<Cart> carts = db.Carts.Where(c => c.CustomerID == customer.CustomerID).ToList();

                List<Product> products = new List<Product>();

                foreach (Cart c in carts)
                {
                    Product p = db.Products.Find(c.ProductID);
                    products.Add(p);
                }

                ViewBag.Products = products;

                return View("Cart", carts);
            }
        }
        [HttpPost]
        public void UpdateQty(int? id, int? qty)
        {
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                if (id >= 0 && qty >= 1)
                {
                    Cart c = db.Carts.Find(id);
                    c.Quantity = qty;
                    db.SaveChanges();
                }

            }
        }

    }
}