﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Poe_Part_2.Data;
using Poe_Part_2.Models;

namespace Poe_Part_2.Controllers
{
    public class UserController : Controller
    {
        // GET: UserController
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


        // GET: UserController/Details/5
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

        public ActionResult Profile()
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
                    var user = context.Users.Find(loggedIn);
                    return View(user);
                }
                else
                {
                    var user = context.Users.Find(loggedIn);
                    return View(user);
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
        }

        // GET: UserController/Create
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

        // POST: UserController/Create
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

        // GET: UserController/Edit/5
        public ActionResult Edit(string id)
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
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

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            var userEdit = context.Users.Find(user.Username);
            userEdit.Username = user.Username;
            userEdit.PhoneNumber = user.PhoneNumber;
            userEdit.Surname = user.Surname;
            userEdit.Name = user.Name;
            userEdit.Email = user.Email;
            userEdit.AccountType = user.AccountType;
            context.Entry(userEdit).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(string id)
        {
            string loggedIn = HttpContext.Session.GetString("SessionUser");
            if (CheckSignedIn(loggedIn))
            {
                string userType = CheckUserType(loggedIn);
                if (userType == "Employee")
                {
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
        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(User user, string id)
        {
            context.Users.Remove(user);
            context.SaveChanges();
            return RedirectToAction("Index");
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
