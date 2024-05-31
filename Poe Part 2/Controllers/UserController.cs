using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Poe_Part_2.Data;
using Poe_Part_2.Models;

namespace Poe_Part_2.Controllers
{
    public class UserController : Controller
    {
        public PoeDbContext context = new PoeDbContext();
        public ActionResult Index()
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    var user = context.Users.ToList();
                    return View(user);
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

        public ActionResult UserProducts(string id)
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    ViewBag.User = context.Users.Find(id).Username;
                    var products = context.Products.Where(x => x.Username == id).ToList();
                    var categoryNames = (from p in products
                                         from c in context.Categories.ToList()
                                         where c.CategoryId == p.CategoryId && p.Username == id
                                         select c.CategoryName).ToList();
                    ViewBag.CategoryNames = categoryNames;
                    return View(products);
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

        public ActionResult Details(string id)
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    var userDetails = context.Users.Find(id);
                    return View(userDetails);
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
        public ActionResult UserProductDetails(int id)
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                var productDetails = context.Products.Find(id);
                ViewBag.User = productDetails.Username;
                var categoryName = context.Categories.Where(x => x.CategoryId == productDetails.CategoryId).First();
                ViewBag.CategoryName = categoryName.CategoryName;
                return View(productDetails);
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
        }

        public ActionResult Profile()
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    var user = context.Users.Find(loggedIn);
                    ViewBag.User = user.Username;
                    return View(user);
                }
                else
                {
                    var user = context.Users.Find(loggedIn);
                    ViewBag.User = user.Username;
                    return View(user);
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
        }

        public ActionResult Create()
        {

            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    List<SelectListItem> accountTypes = new List<SelectListItem>();
                    SelectListItem accountType1 = new SelectListItem();
                    accountType1.Text = "Employee";
                    accountType1.Value = "Employee";
                    accountTypes.Add(accountType1);
                    SelectListItem accountType2 = new SelectListItem();
                    accountType2.Text = "Farmer";
                    accountType2.Value = "Farmer";
                    accountTypes.Add(accountType2);
                    ViewBag.AccountTypes = accountTypes;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            EncryptionClass encryption = new EncryptionClass();
            var check = context.Users.Where(x => x.Username == user.Username).FirstOrDefault();
            if(check == null)
            {
                user.Password = encryption.HashPassword(user.Password);
                context.Users.Add(user);
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }
        }

        public ActionResult Edit(string id)
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    List<SelectListItem> accountTypes = new List<SelectListItem>();
                    SelectListItem accountType1 = new SelectListItem();
                    accountType1.Text = "Employee";
                    accountType1.Value = "Employee";
                    accountTypes.Add(accountType1);
                    SelectListItem accountType2 = new SelectListItem();
                    accountType2.Text = "Farmer";
                    accountType2.Value = "Farmer";
                    accountTypes.Add(accountType2);
                    ViewBag.AccountTypes = accountTypes;
                    var user = context.Users.Find(id);
                    return View(user);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            var userEdit = context.Users.Find(user.Username);
            userEdit.PhoneNumber = user.PhoneNumber;
            userEdit.Surname = user.Surname;
            userEdit.Name = user.Name;
            userEdit.Email = user.Email;
            userEdit.AccountType = user.AccountType;
            context.Entry(userEdit).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public ActionResult SignOut()
        {
            HttpContext.Session.SetString("SessionUser", "");
            return RedirectToAction("SignIn", "Home");
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
