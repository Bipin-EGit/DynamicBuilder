using System.ComponentModel.DataAnnotations;

namespace DynamicFormBuilderMVC.Models
{
    public class FormComponent
    {
        public int Id { get; set; }
        
        [Required]
        public string ComponentId { get; set; } = string.Empty;
        
        [Required]
        public ComponentType Type { get; set; }
        
        [Required]
        public string Label { get; set; } = string.Empty;
        
        public string Properties { get; set; } = "{}"; // JSON properties
        
        public int PositionX { get; set; }
        
        public int PositionY { get; set; }
        
        public int Width { get; set; } = 200;
        
        public int Height { get; set; } = 40;
        
        public int Order { get; set; }
        
        public int FormId { get; set; }
        
        public virtual DynamicForm? Form { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum ComponentType
    {
        Text = 1,
        Textarea = 2,
        Date = 3,
        Select = 4,
        Checkbox = 5,
        Radio = 6,
        Table = 7,
        Chart = 8
    }
}