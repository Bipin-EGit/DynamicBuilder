using System.ComponentModel.DataAnnotations;

namespace DynamicFormBuilderMVC.Models
{
    public class DynamicForm
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = "Untitled Form";
        
        public string? Description { get; set; }
        
        public virtual ICollection<FormComponent> Components { get; set; } = new List<FormComponent>();
        
        public virtual ICollection<EmailSchedule> EmailSchedules { get; set; } = new List<EmailSchedule>();
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
    }
}