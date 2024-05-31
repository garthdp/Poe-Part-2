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
        public PoeDbContext context = new PoeDbContext();
        //shows categories
        public ActionResult Index()
        {
            // only allows employees to see categories
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

        //creates categories
        public ActionResult Create()
        {
            // only allows employees to make categories
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

        // saves category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            context.Categories.Add(category);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // checks if user name is taken
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
        //checks if user is signed in
        private bool CheckSignedIn(string username)
        {
            if (username == null || username == "")
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
