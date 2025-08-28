using System.Collections.Generic;

namespace TwilioMessenger.Core.Models
{
    public class MessageResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? MessageId { get; set; }
        public string? To { get; set; }
        public List<string> FailedNumbers { get; } = new();
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
    }
}