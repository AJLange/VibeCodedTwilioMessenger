using System.ComponentModel.DataAnnotations;
using TwilioMessenger.Core.Models;

namespace TwilioMessenger.Web.Models
{
    public class MessageViewModel
    {
        [Required(ErrorMessage = "Message text is required")]
        [Display(Name = "Message Text")]
        [MinLength(1, ErrorMessage = "Message cannot be empty")]
        [MaxLength(1600, ErrorMessage = "Message is too long (maximum 1600 characters)")]
        public string MessageText { get; set; } = string.Empty;
        
        [Display(Name = "Send To")]
        public MessageRecipientType RecipientType { get; set; } = MessageRecipientType.AllContacts;
        
        [Display(Name = "Specific Recipient")]
        public string? SpecificRecipient { get; set; }
    }
    
    public enum MessageRecipientType
    {
        [Display(Name = "All Contacts")]
        AllContacts,
        
        [Display(Name = "SMS Contacts Only")]
        SmsOnly,
        
        [Display(Name = "WhatsApp Contacts Only")]
        WhatsAppOnly,
        
        [Display(Name = "Specific Recipient")]
        SpecificRecipient
    }
}