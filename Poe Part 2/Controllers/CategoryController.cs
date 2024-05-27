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
            var categories = context.Categories.ToList();
            return View(categories);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            return View();
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

        // GET: CategoryController/Edit/5
        public ActionResult Edit(int id)
        {
            var category = context.Categories.Find(id);
            return View(category);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            var categoryEdit = context.Categories.Find(category.CategoryId);
            categoryEdit.CategoryName = category.CategoryName;
            context.Entry(categoryEdit).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            Category temp = context.Categories.Find(id);
            context.Categories.Remove(temp);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
