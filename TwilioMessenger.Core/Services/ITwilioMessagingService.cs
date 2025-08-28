using System.Threading.Tasks;
using TwilioMessenger.Core.Models;

namespace TwilioMessenger.Core.Services
{
    public interface ITwilioMessagingService
    {
        Task<MessageResult> SendSmsAsync(string to, string message);
        Task<MessageResult> SendWhatsAppAsync(string to, string message);
        Task<MessageResult> SendBulkSmsAsync(string[] to, string message);
        Task<MessageResult> SendBulkWhatsAppAsync(string[] to, string message);
        Task<MessageResult> SendToAllContactsAsync(string message);
        Task<MessageResult> SendToContactTypeAsync(ContactType contactType, string message);
    }
}