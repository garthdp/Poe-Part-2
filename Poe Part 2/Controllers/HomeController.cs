using Microsoft.AspNetCore.Mvc;
using Poe_Part_2.Data;
using Poe_Part_2.Models;
using System.Diagnostics;

namespace Poe_Part_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public PoeDbContext context = new PoeDbContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        // Shows homescreen
        public IActionResult Index()
        {
            // only allows signed in users to see homescreen
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                ViewBag.User = loggedIn;
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
        }
        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(User user)
        {
            // calls encryption class to check is password matchs on in database
            EncryptionClass encryption = new EncryptionClass();
            var check = context.Users.Where(x => x.Username == user.Username).FirstOrDefault();
            //if found it checks if password matchs the one in the database
            if (check != null)
            {
                // compares passwords to see if user entered correct password
                bool isValid = encryption.VerifyPassword(user.Password, check.Password);
                // if password is correct it saves the users usersname to the session
                // the session makes it so that where ever he navigates to it will only bring up his information
                if (isValid)
                {
                    // saves username to session key
                    HttpContext.Session.SetString("SessionUser", user.Username);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
