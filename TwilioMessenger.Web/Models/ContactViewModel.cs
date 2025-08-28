using System.ComponentModel.DataAnnotations;
using TwilioMessenger.Core.Models;

namespace TwilioMessenger.Web.Models
{
    public class ContactViewModel
    {
        public string? Id { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Phone number must be in E.164 format (e.g., +12345678901)")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Display(Name = "Contact Type")]
        public ContactType Type { get; set; } = ContactType.SMS;
        
        [Display(Name = "Opted In")]
        public bool IsOptedIn { get; set; } = true;
    }
}