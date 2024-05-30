using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Poe_Part_2.Data;
using Poe_Part_2.Models;
using System.Collections.Immutable;

namespace Poe_Part_2.Controllers
{
    public class ProductController : Controller
    {
        // GET: ProductController
        PoeDbContext context = new PoeDbContext();
        public ActionResult Index()
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    var products = context.Products.ToList();
                    var categoryNames = (from p in products
                                         from c in context.Categories.ToList()
                                         where c.CategoryId == p.CategoryId
                                         select c.CategoryName).ToList();
                    ViewBag.CategoryNames = categoryNames;
                    return View(products);
                }
                else
                {
                    var products = context.Products.Where(x => x.Username == loggedIn).ToList();
                    var categoryNames = (from p in products
                                         from c in context.Categories.ToList()
                                         where c.CategoryId == p.CategoryId && p.Username == loggedIn
                                         select c.CategoryName).ToList();
                    ViewBag.CategoryNames = categoryNames;
                    return View(products);
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
        }

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                var productDetails = context.Products.Find(id);
                var categoryName = context.Categories.Where(x => x.CategoryId == productDetails.CategoryId).First();
                ViewBag.CategoryName = categoryName.CategoryName;
                return View(productDetails);
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
        }

        // GET: ProductController/Create
        public ActionResult Create(Product product)
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    return RedirectToAction("Error", "Home");
                }
                else
                {
                    var categories = (from u in context.Categories
                                 select new SelectListItem()
                                 {
                                     Text = u.CategoryName,
                                     Value = u.CategoryId.ToString()
                                 }).ToList();
                    ViewBag.CategoryId = categories;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection, Product product)
        {
            product.Username = HttpContext.Session.GetString("SessionUser");
            context.Products.Add(product);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    return RedirectToAction("Error", "Home");
                }
                else
                {
                    var categories = (from u in context.Categories
                                      select new SelectListItem()
                                      {
                                          Text = u.CategoryName,
                                          Value = u.CategoryId.ToString()
                                      }).ToList();
                    ViewBag.CategoryId = categories;
                    var user = context.Products.Find(id);
                    return View(user);
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product productOld)
        {
            var productUpdated = context.Products.Find(productOld.ProductId);
            productUpdated.ProductName = productOld.ProductName;
            productUpdated.ProductionDate = productOld.ProductionDate;
            productUpdated.Info = productOld.Info;
            productUpdated.CategoryId = productOld.CategoryId;
            context.Entry(productUpdated).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    return RedirectToAction("Error", "Home");
                }
                else
                {
                    var user = context.Products.Find(id);
                    var productDetails = context.Products.Find(id);
                    var categoryName = context.Categories.Where(x => x.CategoryId == productDetails.CategoryId).First();
                    ViewBag.CategoryName = categoryName.CategoryName;
                    return View(user);
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Product product, string id)
        {
            context.Products.Remove(product);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        private string CheckUserType(string username)
        {
            var user = context.Users.Where(x => x.Username == username).First();

            if (user.AccountType == "Employee")
            {
                var users = context.Users.ToList();
                return "Employee";
            }
            else
            {
                return "Farmer";
            }
        }
        private bool CheckSignedIn(string username)
        {
            if (username == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
