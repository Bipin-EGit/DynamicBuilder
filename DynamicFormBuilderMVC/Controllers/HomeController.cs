using DynamicFormBuilderMVC.Models;
using DynamicFormBuilderMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DynamicFormBuilderMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFormBuilderService _formBuilderService;

        public HomeController(IFormBuilderService formBuilderService)
        {
            _formBuilderService = formBuilderService;
        }

        public async Task<IActionResult> Index()
        {
            var forms = await _formBuilderService.GetAllFormsAsync();
            return View(forms);
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
    }
}