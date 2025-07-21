namespace DynamicFormBuilderMVC.Models.ViewModels
{
    public class FormBuilderViewModel
    {
        public DynamicForm Form { get; set; } = new DynamicForm();
        public List<FormComponent> Components { get; set; } = new List<FormComponent>();
        public string GeneratedHtml { get; set; } = string.Empty;
    }

    public class ComponentCreateViewModel
    {
        public ComponentType Type { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Properties { get; set; } = "{}";
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int FormId { get; set; }
    }

    public class ComponentUpdateViewModel
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Properties { get; set; } = "{}";
        public int PositionX { get; set; }
        public int PositionY { get; set; }
    }

    public class EmailScheduleViewModel
    {
        public string Email { get; set; } = string.Empty;
        public EmailFrequency Frequency { get; set; }
        public string Time { get; set; } = "09:00";
        public int FormId { get; set; }
    }

    public class FormPreviewViewModel
    {
        public DynamicForm Form { get; set; } = new DynamicForm();
        public string GeneratedHtml { get; set; } = string.Empty;
        public List<EmailSchedule> EmailSchedules { get; set; } = new List<EmailSchedule>();
    }
}