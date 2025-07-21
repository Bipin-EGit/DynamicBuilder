using DynamicFormBuilderMVC.Models;
using DynamicFormBuilderMVC.Models.ViewModels;

namespace DynamicFormBuilderMVC.Services
{
    public interface IFormBuilderService
    {
        Task<DynamicForm> GetFormAsync(int id);
        Task<DynamicForm> CreateFormAsync(string title, string? description = null);
        Task<DynamicForm> UpdateFormAsync(int id, string title, string? description = null);
        Task<bool> DeleteFormAsync(int id);
        
        Task<FormComponent> AddComponentAsync(ComponentCreateViewModel model);
        Task<FormComponent> UpdateComponentAsync(ComponentUpdateViewModel model);
        Task<bool> DeleteComponentAsync(int id);
        Task<List<FormComponent>> GetFormComponentsAsync(int formId);
        
        Task<string> GenerateHtmlAsync(int formId);
        Task<List<DynamicForm>> GetAllFormsAsync();
    }
}