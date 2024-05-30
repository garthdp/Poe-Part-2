using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poe_Part_2.Data;
using Poe_Part_2.Models;
using System.Data;
using System.Configuration;

namespace Poe_Part_2.Controllers
{
    public class CategoryController : Controller
    {
        // GET: CategoryController
        public PoeDbContext context = new PoeDbContext();
        public ActionResult Index()
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    var categories = context.Categories.ToList();
                    return View(categories);
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            context.Categories.Add(category);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
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
