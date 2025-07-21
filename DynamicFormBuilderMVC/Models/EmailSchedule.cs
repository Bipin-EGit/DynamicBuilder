using System.ComponentModel.DataAnnotations;

namespace DynamicFormBuilderMVC.Models
{
    public class EmailSchedule
    {
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public EmailFrequency Frequency { get; set; }
        
        [Required]
        public TimeSpan Time { get; set; }
        
        public bool IsEnabled { get; set; } = true;
        
        public int FormId { get; set; }
        
        public virtual DynamicForm? Form { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastSentAt { get; set; }
        
        public DateTime? NextSendAt { get; set; }
    }

    public enum EmailFrequency
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }
}