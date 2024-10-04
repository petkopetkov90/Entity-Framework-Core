using BlogApp.Models;
using BlogApp.Services.Models;
using BlogApp.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlogApp.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService service;

        public ArticleController(IArticleService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var models = await service.GetAllAsync();

            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ArticleInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await service.AddAsync(model);
            }
            catch (UnauthorizedAccessException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var model = await service.FindByIdAsync(id);
                return View(model);
            }
            catch (KeyNotFoundException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            try
            {
                var model = await service.FindByIdAsync(id);
                return View(model);
            }
            catch (Exception e) when (e is KeyNotFoundException || e is UnauthorizedAccessException)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(ArticleViewModel model, int id)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await service.UpdateAsync(model, id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e) when (e is KeyNotFoundException || e is UnauthorizedAccessException)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await service.DeleteAsync(id);
            }
            catch (Exception e) when (e is KeyNotFoundException || e is UnauthorizedAccessException)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
