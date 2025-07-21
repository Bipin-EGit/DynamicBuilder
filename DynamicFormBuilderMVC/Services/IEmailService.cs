using DynamicFormBuilderMVC.Models;
using DynamicFormBuilderMVC.Models.ViewModels;

namespace DynamicFormBuilderMVC.Services
{
    public interface IEmailService
    {
        Task<EmailSchedule> CreateScheduleAsync(EmailScheduleViewModel model);
        Task<bool> DeleteScheduleAsync(int id);
        Task<bool> ToggleScheduleAsync(int id);
        Task<List<EmailSchedule>> GetFormSchedulesAsync(int formId);
        Task SendFormEmailAsync(EmailSchedule schedule, string formHtml);
        Task<bool> TestEmailAsync(string email, string subject, string body);
    }
}