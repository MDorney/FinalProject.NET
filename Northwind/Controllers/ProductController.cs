using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Northwind.Models;

namespace Northwind.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product/Category
        public ActionResult Category()
        {
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                return View(db.Categories.OrderBy(c=>c.CategoryID).ToList());
            }
        }
        // GET: Product/Discount
        public ActionResult Discounts()
        {
            using (var db = new NORTHWNDEntities())
            {
                return View(db.Discounts.OrderByDescending(q => q.DiscountPercent).ToList());
            }
        }
        
        //GET: Products in One Category
        public ActionResult ProductsInCategory(int? id)
        {
            ViewBag.id = id;
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                return View("Product", db.Products.Where(q => q.CategoryID == id).OrderBy(q=>q.ProductName).ToList());
            }
        }
        //GET: Product Search   
        public ActionResult Search()
        {
            return View();
        }
        //Post: Search Results
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchResults(FormCollection Form)
        {
            string SearchString = Form["SearchString"];
            ViewBag.Filter = "Product";
            ViewBag.SearchString = SearchString;
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                return View("Product", db.Products.Where(p => p.ProductName.Contains(SearchString) && p.Discontinued == false).OrderBy(p => p.ProductName).ToList());
            }
        }
        public JsonResult FilterProducts(int? id, string SearchString, decimal? PriceFilter)
        {
            
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                if (PriceFilter == null)
                {
                    Response.StatusCode = 400;
                    return Json(new { }, JsonRequestBehavior.AllowGet);
                }
                var Products = db.Products.Where(p => p.Discontinued == false).ToList();
                if (id != null)
                {
                    Products = Products.Where(p => p.CategoryID == id).ToList();
                }
                if (!string.IsNullOrEmpty(SearchString))
                {
                    Products = Products.Where(p => p.ProductName.Contains(SearchString)).ToList();
                }
                var ProductDTOs = (from p in Products.Where(p => p.UnitPrice >= PriceFilter)
                                   orderby p.ProductName
                                   select new
                                   {
                                       p.ProductID,
                                       p.ProductName,
                                       p.QuantityPerUnit,
                                       p.UnitPrice,
                                       p.UnitsInStock
                                   }).ToList();
                return Json(ProductDTOs, JsonRequestBehavior.AllowGet);
            }
        }
    }
}