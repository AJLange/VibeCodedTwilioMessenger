using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TwilioMessenger.Core.Models;
using TwilioMessenger.Core.Services;
using TwilioMessenger.Web.Models;

namespace TwilioMessenger.Web.Pages
{
    public class SendMessageModel : PageModel
    {
        private readonly ITwilioMessagingService _messagingService;
        private readonly IContactRepository _contactRepository;
        private readonly ILogger<SendMessageModel> _logger;

        [BindProperty]
        public MessageViewModel Message { get; set; } = new();
        
        public List<ContactViewModel> Contacts { get; set; } = new();
        public string? StatusMessage { get; set; }
        public bool IsSuccess { get; set; }
        public MessageResult? RecentResult { get; set; }

        public SendMessageModel(
            ITwilioMessagingService messagingService,
            IContactRepository contactRepository,
            ILogger<SendMessageModel> logger)
        {
            _messagingService = messagingService;
            _contactRepository = contactRepository;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            await LoadContactsAsync();
            
            StatusMessage = TempData["StatusMessage"] as string;
            IsSuccess = TempData["IsSuccess"] as bool? ?? false;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadContactsAsync();
                return Page();
            }

            try
            {
                MessageResult result;

                switch (Message.RecipientType)
                {
                    case MessageRecipientType.AllContacts:
                        result = await _messagingService.SendToAllContactsAsync(Message.MessageText);
                        break;
                        
                    case MessageRecipientType.SmsOnly:
                        result = await _messagingService.SendToContactTypeAsync(ContactType.SMS, Message.MessageText);
                        break;
                        
                    case MessageRecipientType.WhatsAppOnly:
                        result = await _messagingService.SendToContactTypeAsync(ContactType.WhatsApp, Message.MessageText);
                        break;
                        
                    case MessageRecipientType.SpecificRecipient:
                        if (string.IsNullOrEmpty(Message.SpecificRecipient))
                        {
                            ModelState.AddModelError("Message.SpecificRecipient", "Please select a recipient.");
                            await LoadContactsAsync();
                            return Page();
                        }
                        
                        // Determine if this is an SMS or WhatsApp contact
                        var contact = await _contactRepository.GetContactByPhoneNumberAsync(Message.SpecificRecipient);
                        if (contact == null)
                        {
                            ModelState.AddModelError(string.Empty, "Selected contact was not found.");
                            await LoadContactsAsync();
                            return Page();
                        }
                        
                        if (contact.Type == ContactType.SMS)
                        {
                            result = await _messagingService.SendSmsAsync(contact.PhoneNumber, Message.MessageText);
                        }
                        else
                        {
                            result = await _messagingService.SendWhatsAppAsync(contact.PhoneNumber, Message.MessageText);
                        }
                        break;
                        
                    default:
                        ModelState.AddModelError(string.Empty, "Invalid recipient type selected.");
                        await LoadContactsAsync();
                        return Page();
                }

                RecentResult = result;
                StatusMessage = result.Success 
                    ? $"Message sent successfully to {result.SuccessCount} recipient(s)." 
                    : "Failed to send message.";
                IsSuccess = result.Success;

                await LoadContactsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                StatusMessage = "An error occurred while sending the message.";
                IsSuccess = false;
                await LoadContactsAsync();
                return Page();
            }
        }

        private async Task LoadContactsAsync()
        {
            var contacts = await _contactRepository.GetOptedInContactsAsync();
            
            Contacts = contacts.Select(c => new ContactViewModel
            {
                Id = c.Id,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                Type = c.Type,
                IsOptedIn = c.IsOptedIn
            }).ToList();
        }
    }
}