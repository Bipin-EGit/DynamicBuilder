using DynamicFormBuilderMVC.Data;
using DynamicFormBuilderMVC.Models;
using DynamicFormBuilderMVC.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace DynamicFormBuilderMVC.Services
{
    public class FormBuilderService : IFormBuilderService
    {
        private readonly ApplicationDbContext _context;

        public FormBuilderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DynamicForm> GetFormAsync(int id)
        {
            return await _context.Forms
                .Include(f => f.Components.OrderBy(c => c.Order))
                .Include(f => f.EmailSchedules)
                .FirstOrDefaultAsync(f => f.Id == id) ?? new DynamicForm();
        }

        public async Task<DynamicForm> CreateFormAsync(string title, string? description = null)
        {
            var form = new DynamicForm
            {
                Title = title,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Forms.Add(form);
            await _context.SaveChangesAsync();
            return form;
        }

        public async Task<DynamicForm> UpdateFormAsync(int id, string title, string? description = null)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form != null)
            {
                form.Title = title;
                form.Description = description;
                form.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return form ?? new DynamicForm();
        }

        public async Task<bool> DeleteFormAsync(int id)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form != null)
            {
                _context.Forms.Remove(form);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<FormComponent> AddComponentAsync(ComponentCreateViewModel model)
        {
            var component = new FormComponent
            {
                ComponentId = Guid.NewGuid().ToString(),
                Type = model.Type,
                Label = model.Label,
                Properties = model.Properties,
                PositionX = model.PositionX,
                PositionY = model.PositionY,
                FormId = model.FormId,
                Order = await GetNextOrderAsync(model.FormId),
                CreatedAt = DateTime.UtcNow
            };

            _context.FormComponents.Add(component);
            await _context.SaveChangesAsync();
            return component;
        }

        public async Task<FormComponent> UpdateComponentAsync(ComponentUpdateViewModel model)
        {
            var component = await _context.FormComponents.FindAsync(model.Id);
            if (component != null)
            {
                component.Label = model.Label;
                component.Properties = model.Properties;
                component.PositionX = model.PositionX;
                component.PositionY = model.PositionY;
                await _context.SaveChangesAsync();
            }
            return component ?? new FormComponent();
        }

        public async Task<bool> DeleteComponentAsync(int id)
        {
            var component = await _context.FormComponents.FindAsync(id);
            if (component != null)
            {
                _context.FormComponents.Remove(component);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<FormComponent>> GetFormComponentsAsync(int formId)
        {
            return await _context.FormComponents
                .Where(c => c.FormId == formId)
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        public async Task<List<DynamicForm>> GetAllFormsAsync()
        {
            return await _context.Forms
                .Include(f => f.Components)
                .OrderByDescending(f => f.UpdatedAt)
                .ToListAsync();
        }

        public async Task<string> GenerateHtmlAsync(int formId)
        {
            var form = await GetFormAsync(formId);
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine($"    <title>{form.Title}</title>");
            html.AppendLine("    <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css\">");
            html.AppendLine("    <script src=\"https://cdn.jsdelivr.net/npm/chart.js\"></script>");
            html.AppendLine("    <style>");
            html.AppendLine(GenerateCSS());
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <div class=\"dynamic-form\">");
            html.AppendLine($"        <h1>{form.Title}</h1>");

            if (!string.IsNullOrEmpty(form.Description))
            {
                html.AppendLine($"        <p class=\"form-description\">{form.Description}</p>");
            }

            html.AppendLine("        <form>");

            foreach (var component in form.Components.OrderBy(c => c.Order))
            {
                html.AppendLine(GenerateComponentHtml(component));
            }

            html.AppendLine("            <div class=\"form-actions\">");
            html.AppendLine("                <button type=\"submit\" class=\"btn btn-primary\">Submit</button>");
            html.AppendLine("                <button type=\"reset\" class=\"btn btn-secondary\">Reset</button>");
            html.AppendLine("            </div>");
            html.AppendLine("        </form>");
            html.AppendLine("    </div>");
            html.AppendLine("    <script>");
            html.AppendLine(GenerateJavaScript(form.Components.Where(c => c.Type == ComponentType.Chart).ToList()));
            html.AppendLine("    </script>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private async Task<int> GetNextOrderAsync(int formId)
        {
            var maxOrder = await _context.FormComponents
                .Where(c => c.FormId == formId)
                .MaxAsync(c => (int?)c.Order) ?? 0;
            return maxOrder + 1;
        }

        private string GenerateComponentHtml(FormComponent component)
        {
            var properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(component.Properties) ?? new Dictionary<string, object>();
            var html = new StringBuilder();

            html.AppendLine("            <div class=\"form-group\">");

            switch (component.Type)
            {
                case ComponentType.Text:
                    var placeholder = properties.GetValueOrDefault("placeholder", "").ToString();
                    html.AppendLine($"                <label>{component.Label}</label>");
                    html.AppendLine($"                <input type=\"text\" name=\"{component.ComponentId}\" placeholder=\"{placeholder}\" class=\"form-control\" />");
                    break;

                case ComponentType.Textarea:
                    var textareaPlaceholder = properties.GetValueOrDefault("placeholder", "").ToString();
                    var rows = properties.GetValueOrDefault("rows", 4).ToString();
                    html.AppendLine($"                <label>{component.Label}</label>");
                    html.AppendLine($"                <textarea name=\"{component.ComponentId}\" placeholder=\"{textareaPlaceholder}\" rows=\"{rows}\" class=\"form-control\"></textarea>");
                    break;

                case ComponentType.Date:
                    html.AppendLine($"                <label>{component.Label}</label>");
                    html.AppendLine($"                <input type=\"date\" name=\"{component.ComponentId}\" class=\"form-control\" />");
                    break;

                case ComponentType.Select:
                    html.AppendLine($"                <label>{component.Label}</label>");
                    html.AppendLine($"                <select name=\"{component.ComponentId}\" class=\"form-control\">");
                    if (properties.ContainsKey("options") && properties["options"] is Newtonsoft.Json.Linq.JArray options)
                    {
                        foreach (var option in options)
                        {
                            html.AppendLine($"                    <option value=\"{option}\">{option}</option>");
                        }
                    }
                    html.AppendLine("                </select>");
                    break;

                case ComponentType.Checkbox:
                    html.AppendLine($"                <label class=\"checkbox-label\">");
                    html.AppendLine($"                    <input type=\"checkbox\" name=\"{component.ComponentId}\" />");
                    html.AppendLine($"                    {component.Label}");
                    html.AppendLine("                </label>");
                    break;

                case ComponentType.Radio:
                    html.AppendLine($"                <label class=\"group-label\">{component.Label}</label>");
                    html.AppendLine("                <div class=\"radio-group\">");
                    if (properties.ContainsKey("options") && properties["options"] is Newtonsoft.Json.Linq.JArray radioOptions)
                    {
                        foreach (var option in radioOptions)
                        {
                            html.AppendLine($"                    <label class=\"radio-label\">");
                            html.AppendLine($"                        <input type=\"radio\" name=\"{component.ComponentId}\" value=\"{option}\" />");
                            html.AppendLine($"                        {option}");
                            html.AppendLine("                    </label>");
                        }
                    }
                    html.AppendLine("                </div>");
                    break;

                case ComponentType.Table:
                    html.AppendLine($"                <h3>{component.Label}</h3>");
                    html.AppendLine("                <table class=\"data-table\">");
                    html.AppendLine("                    <thead>");
                    html.AppendLine("                        <tr>");
                    html.AppendLine("                            <th>Column 1</th>");
                    html.AppendLine("                            <th>Column 2</th>");
                    html.AppendLine("                            <th>Column 3</th>");
                    html.AppendLine("                        </tr>");
                    html.AppendLine("                    </thead>");
                    html.AppendLine("                    <tbody>");
                    html.AppendLine("                        <tr>");
                    html.AppendLine("                            <td>Sample Data 1</td>");
                    html.AppendLine("                            <td>Sample Data 2</td>");
                    html.AppendLine("                            <td>Sample Data 3</td>");
                    html.AppendLine("                        </tr>");
                    html.AppendLine("                        <tr>");
                    html.AppendLine("                            <td>Sample Data 4</td>");
                    html.AppendLine("                            <td>Sample Data 5</td>");
                    html.AppendLine("                            <td>Sample Data 6</td>");
                    html.AppendLine("                        </tr>");
                    html.AppendLine("                    </tbody>");
                    html.AppendLine("                </table>");
                    break;

                case ComponentType.Chart:
                    var chartType = properties.GetValueOrDefault("chartType", "bar").ToString();
                    html.AppendLine($"                <h3>{component.Label}</h3>");
                    html.AppendLine($"                <div class=\"chart-container\">");
                    html.AppendLine($"                    <canvas id=\"{component.ComponentId}\" data-chart-type=\"{chartType}\"></canvas>");
                    html.AppendLine("                </div>");
                    break;
            }

            html.AppendLine("            </div>");
            return html.ToString();
        }

        private string GenerateCSS()
        {
            return @"
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            max-width: 800px;
            margin: 0 auto;
            padding: 2rem;
            background: #f5f5f5;
            color: #333;
        }
        .dynamic-form {
            background: white;
            padding: 2rem;
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }
        h1 {
            color: #2c3e50;
            text-align: center;
            margin-bottom: 1rem;
        }
        .form-description {
            text-align: center;
            color: #666;
            margin-bottom: 2rem;
        }
        .form-group {
            margin-bottom: 1.5rem;
        }
        label {
            display: block;
            margin-bottom: 0.5rem;
            font-weight: 500;
            color: #495057;
        }
        .group-label {
            font-weight: 600;
            color: #2c3e50;
        }
        .form-control {
            width: 100%;
            padding: 0.75rem;
            border: 1px solid #ced4da;
            border-radius: 4px;
            font-size: 1rem;
            transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
        }
        .form-control:focus {
            outline: none;
            border-color: #3498db;
            box-shadow: 0 0 0 2px rgba(52, 152, 219, 0.25);
        }
        .checkbox-label, .radio-label {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            cursor: pointer;
            font-weight: normal;
            margin-bottom: 0.5rem;
        }
        .radio-group {
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
            margin-left: 1rem;
        }
        .data-table {
            width: 100%;
            border-collapse: collapse;
            border: 1px solid #dee2e6;
            margin-top: 0.5rem;
        }
        .data-table th,
        .data-table td {
            padding: 0.75rem;
            text-align: left;
            border: 1px solid #dee2e6;
        }
        .data-table th {
            background-color: #f8f9fa;
            font-weight: 600;
            color: #495057;
        }
        .data-table tbody tr:nth-child(even) {
            background-color: #f8f9fa;
        }
        .chart-container {
            width: 100%;
            height: 300px;
            position: relative;
            margin-top: 0.5rem;
        }
        .form-actions {
            margin-top: 2rem;
            padding-top: 2rem;
            border-top: 1px solid #dee2e6;
            display: flex;
            gap: 1rem;
            justify-content: center;
        }
        .btn {
            padding: 0.75rem 1.5rem;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 1rem;
            font-weight: 500;
            text-decoration: none;
            display: inline-block;
            text-align: center;
            transition: all 0.3s ease;
        }
        .btn-primary {
            background: #3498db;
            color: white;
        }
        .btn-primary:hover {
            background: #2980b9;
        }
        .btn-secondary {
            background: #6c757d;
            color: white;
        }
        .btn-secondary:hover {
            background: #5a6268;
        }
        @media (max-width: 768px) {
            body {
                padding: 1rem;
            }
            .dynamic-form {
                padding: 1rem;
            }
            .form-actions {
                flex-direction: column;
            }
        }";
        }

        private string GenerateJavaScript(List<FormComponent> chartComponents)
        {
            var js = new StringBuilder();
            js.AppendLine("document.addEventListener('DOMContentLoaded', function() {");

            foreach (var component in chartComponents)
            {
                var properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(component.Properties) ?? new Dictionary<string, object>();
                var chartType = properties.GetValueOrDefault("chartType", "bar").ToString();

                js.AppendLine($"    const canvas{component.Id} = document.getElementById('{component.ComponentId}');");
                js.AppendLine($"    if (canvas{component.Id}) {{");
                js.AppendLine($"        new Chart(canvas{component.Id}, {{");
                js.AppendLine($"            type: '{chartType}',");
                js.AppendLine("            data: {");
                js.AppendLine("                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May'],");
                js.AppendLine("                datasets: [{");
                js.AppendLine("                    label: 'Sample Data',");
                js.AppendLine("                    data: [12, 19, 3, 5, 2],");

                if (chartType == "pie" || chartType == "doughnut")
                {
                    js.AppendLine("                    backgroundColor: [");
                    js.AppendLine("                        'rgba(231, 76, 60, 0.8)',");
                    js.AppendLine("                        'rgba(52, 152, 219, 0.8)',");
                    js.AppendLine("                        'rgba(241, 196, 15, 0.8)',");
                    js.AppendLine("                        'rgba(46, 204, 113, 0.8)',");
                    js.AppendLine("                        'rgba(155, 89, 182, 0.8)'");
                    js.AppendLine("                    ]");
                }
                else
                {
                    js.AppendLine("                    backgroundColor: 'rgba(52, 152, 219, 0.6)',");
                    js.AppendLine("                    borderColor: 'rgba(52, 152, 219, 1)',");
                    js.AppendLine("                    borderWidth: 1");
                }

                js.AppendLine("                }]");
                js.AppendLine("            },");
                js.AppendLine("            options: {");
                js.AppendLine("                responsive: true,");
                js.AppendLine("                maintainAspectRatio: false");
                js.AppendLine("            }");
                js.AppendLine("        });");
                js.AppendLine("    }");
            }

            js.AppendLine("});");
            return js.ToString();
        }
    }
}