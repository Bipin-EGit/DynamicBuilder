using DynamicFormBuilderMVC.Models;
using DynamicFormBuilderMVC.Models.ViewModels;
using DynamicFormBuilderMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicFormBuilderMVC.Controllers
{
    public class FormBuilderController : Controller
    {
        private readonly IFormBuilderService _formBuilderService;
        private readonly IEmailService _emailService;

        public FormBuilderController(IFormBuilderService formBuilderService, IEmailService emailService)
        {
            _formBuilderService = formBuilderService;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index(int id = 1)
        {
            var form = await _formBuilderService.GetFormAsync(id);
            if (form.Id == 0)
            {
                form = await _formBuilderService.CreateFormAsync("New Form", "Created from Form Builder");
            }

            var viewModel = new FormBuilderViewModel
            {
                Form = form,
                Components = form.Components.ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var form = await _formBuilderService.CreateFormAsync("New Form", "Created from Form Builder");
            return RedirectToAction("Index", new { id = form.Id });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateForm(int id, string title, string? description)
        {
            await _formBuilderService.UpdateFormAsync(id, title, description);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddComponent([FromBody] ComponentCreateViewModel model)
        {
            try
            {
                var component = await _formBuilderService.AddComponentAsync(model);
                return Json(new { success = true, component });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateComponent([FromBody] ComponentUpdateViewModel model)
        {
            try
            {
                var component = await _formBuilderService.UpdateComponentAsync(model);
                return Json(new { success = true, component });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComponent(int id)
        {
            try
            {
                var success = await _formBuilderService.DeleteComponentAsync(id);
                return Json(new { success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Preview(int id)
        {
            var form = await _formBuilderService.GetFormAsync(id);
            var generatedHtml = await _formBuilderService.GenerateHtmlAsync(id);
            var emailSchedules = await _emailService.GetFormSchedulesAsync(id);

            var viewModel = new FormPreviewViewModel
            {
                Form = form,
                GeneratedHtml = generatedHtml,
                EmailSchedules = emailSchedules
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateHtml(int id)
        {
            var html = await _formBuilderService.GenerateHtmlAsync(id);
            return Json(new { success = true, html });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadHtml(int id)
        {
            var form = await _formBuilderService.GetFormAsync(id);
            var html = await _formBuilderService.GenerateHtmlAsync(id);
            
            var fileName = $"{form.Title.Replace(" ", "_").ToLower()}.html";
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(html);
            
            return File(fileBytes, "text/html", fileName);
        }

        // Email Scheduling Actions
        public async Task<IActionResult> EmailScheduler(int id)
        {
            var form = await _formBuilderService.GetFormAsync(id);
            var schedules = await _emailService.GetFormSchedulesAsync(id);
            
            ViewBag.Form = form;
            ViewBag.Schedules = schedules;
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmailSchedule([FromBody] EmailScheduleViewModel model)
        {
            try
            {
                var schedule = await _emailService.CreateScheduleAsync(model);
                return Json(new { success = true, schedule });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmailSchedule(int id)
        {
            try
            {
                var success = await _emailService.DeleteScheduleAsync(id);
                return Json(new { success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleEmailSchedule(int id)
        {
            try
            {
                var success = await _emailService.ToggleScheduleAsync(id);
                return Json(new { success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TestEmail([FromBody] dynamic data)
        {
            try
            {
                string email = data.email;
                string subject = data.subject ?? "Test Email";
                string body = data.body ?? "<h1>Test Email</h1><p>This is a test email from Dynamic Form Builder.</p>";
                
                var success = await _emailService.TestEmailAsync(email, subject, body);
                return Json(new { success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFormComponents(int formId)
        {
            var components = await _formBuilderService.GetFormComponentsAsync(formId);
            return Json(components);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmailSchedules(int formId)
        {
            var schedules = await _emailService.GetFormSchedulesAsync(formId);
            return Json(schedules);
        }
    }
}