using System;
using System.Text.Json.Serialization;

namespace TwilioMessenger.Core.Models
{
    public class Contact
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string Name { get; set; } = string.Empty;
        
        public string PhoneNumber { get; set; } = string.Empty;
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContactType Type { get; set; } = ContactType.SMS;
        
        public bool IsOptedIn { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public enum ContactType
    {
        SMS,
        WhatsApp
    }
}