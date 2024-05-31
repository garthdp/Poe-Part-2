using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Poe_Part_2.Data;
using Poe_Part_2.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Poe_Part_2.Controllers
{
    public class UserController : Controller
    {
        // provides access to database
        public PoeDbContext context = new PoeDbContext();

        // shows list of users 
        public ActionResult Index()
        {
            // If statement which checks if user is signed in, redirects unauthorised users
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                // checks user type and only allows employees access, redirects unauthorised users
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
        // shows list of selected users products
        public ActionResult UserProducts(string id, DateOnly searchDate1, DateOnly searchDate2, int searchCategories)
        {
            // If statement which checks if user is signed in, redirects unauthorised users
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                // checks user type and only allows employees access, redirects unauthorised users
                if (userType == "Employee")
                {
                    // fills categories selectbox
                    var categories = (from u in context.Categories
                                      select new SelectListItem()
                                      {
                                          Text = u.CategoryName,
                                          Value = u.CategoryId.ToString()
                                      }).ToList();
                    categories.Insert(0, new SelectListItem() { Text = "----Select----", Value = "" });
                    ViewBag.CategoryId = categories;

                    ViewBag.User = id;

                    //if statements which filter products for user
                    //checks if user has changed anything and if they havent shows all products
                    if ((searchDate1 == DateOnly.Parse("0001/01/01") || searchDate2 == DateOnly.Parse("0001/01/01")) && searchCategories == 0)
                    {
                        var products = context.Products.Where(x => x.Username == id).ToList();
                        var categoryNames = (from p in products
                                             from c in context.Categories.ToList()
                                             where c.CategoryId == p.CategoryId
                                             select c.CategoryName).ToList();
                        ViewBag.CategoryNames = categoryNames;
                        return View(products);
                    }
                    // filters by date
                    else if (searchDate1 <= searchDate2 && searchCategories == 0)
                    {
                        var products = context.Products.ToList();
                        var filterProducts = (from p in products
                                              where p.ProductionDate >= searchDate1 && p.ProductionDate <= searchDate2
                                              && p.Username == id
                                              select p);
                        var categoryNames = (from p in filterProducts
                                             from c in context.Categories.ToList()
                                             where c.CategoryId == p.CategoryId
                                             select c.CategoryName).ToList();
                        ViewBag.CategoryNames = categoryNames;
                        return View(filterProducts);
                    }
                    // filters by category
                    else if ((searchDate1 == DateOnly.Parse("0001/01/01") || searchDate2 == DateOnly.Parse("0001/01/01")) && searchCategories != 0)
                    {
                        var products = context.Products.ToList();
                        var filterProducts = (from p in products
                                              where p.CategoryId == searchCategories
                                              && p.Username == id
                                              select p);
                        var categoryNames = (from p in filterProducts
                                             from c in context.Categories.ToList()
                                             where c.CategoryId == searchCategories
                                             select c.CategoryName).ToList();
                        ViewBag.CategoryNames = categoryNames;
                        return View(filterProducts);
                    }
                    // filters by date and category
                    else if (searchDate1 <= searchDate2 && searchCategories != 0)
                    {
                        var products = context.Products.ToList();
                        var filterProducts = (from p in products
                                              where p.CategoryId == searchCategories
                                              && p.ProductionDate >= searchDate1 && p.ProductionDate <= searchDate2
                                              && p.Username == id
                                              select p);
                        var categoryNames = (from p in filterProducts
                                             from c in context.Categories.ToList()
                                             where c.CategoryId == searchCategories
                                             select c.CategoryName).ToList();
                        ViewBag.CategoryNames = categoryNames;
                        return View(filterProducts);
                    }
                    // shows all products
                    else
                    {
                        var products = context.Products.Where(x => x.Username == id).ToList();
                        var categoryNames = (from p in products
                                             from c in context.Categories.ToList()
                                             where c.CategoryId == p.CategoryId
                                             select c.CategoryName).ToList();
                        ViewBag.CategoryNames = categoryNames;
                        return View(products);
                    }
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

        //shows user details
        public ActionResult Details(string id)
        {
            //checks if user is logged in and if user is employee
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
        // shows employee the users product details
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

        // shows users their profile
        public ActionResult Profile()
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                var user = context.Users.Find(loggedIn);
                ViewBag.User = user.Username;
                return View(user);
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
        }

        // Creates user account
        public ActionResult Create()
        {
            // only allows employees to create accounts
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

        // saves account to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            EncryptionClass encryption = new EncryptionClass();
            //checks if username is take
            var check = context.Users.Where(x => x.Username == user.Username).FirstOrDefault();
            if(check == null)
            {
                // checks email and phonenumber format
                if (IsEmail(user.Email) && IsPhonenumber(user.PhoneNumber))
                {
                    user.Password = encryption.HashPassword(user.Password);
                    context.Users.Add(user);
                    context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("Create", "User");
                }
            }
            else
            {
                return RedirectToAction("Create", "User");
            }
        }

        // Edits user details
        public ActionResult Edit(string id)
        {
            // only employees can make changes
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    ViewBag.User = id;

                    // fills selectlist with account types
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

        //saves changes to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (IsEmail(user.Email) && IsPhonenumber(user.PhoneNumber))
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
            else
            {
                return RedirectToAction("Edit", "User");
            }
        }
        //signs user out
        public ActionResult SignOut()
        {
            HttpContext.Session.SetString("SessionUser", "");
            return RedirectToAction("SignIn", "Home");
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
        // checks email format
        private static bool IsEmail(string email)
        {
            bool isEmail = true;
            try
            {
                var checkedEmail = new MailAddress(email);
            }
            catch
            {
                isEmail = false;
            }
            return isEmail;
        }
        // checks phonenumber format
        private static bool IsPhonenumber(string phonenumber)
        {
            bool isPhonenumber = true;
            if (phonenumber.Length == 10)
            {
                for (int i = 0; i < phonenumber.Length; i++)
                {
                    if ((i == 0 && phonenumber[i] != '0') || !Char.IsDigit(phonenumber[i]))
                    {
                        isPhonenumber = false;
                        break;
                    }
                    else
                    {
                        isPhonenumber = true;
                    }
                }
                return isPhonenumber;
            }
            else
            {
                isPhonenumber = false;
                return isPhonenumber;
            }
        }
    }
}
