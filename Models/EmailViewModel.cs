using System.ComponentModel.DataAnnotations;

namespace Leasing.Models
{
    public class EmailViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string AgentEmail { get; set; } = string.Empty;
        public string AgentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your name")]
        [Display(Name = "Your Name")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Your Email")]
        public string UserEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a message")]
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;
    }
}