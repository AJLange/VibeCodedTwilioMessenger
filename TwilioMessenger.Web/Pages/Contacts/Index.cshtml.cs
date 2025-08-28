using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TwilioMessenger.Core.Models;
using TwilioMessenger.Core.Services;
using TwilioMessenger.Web.Models;

namespace TwilioMessenger.Web.Pages.Contacts
{
    public class IndexModel : PageModel
    {
        private readonly IContactRepository _contactRepository;

        public List<ContactViewModel> Contacts { get; private set; } = new();
        public string? StatusMessage { get; set; }
        public bool IsSuccess { get; set; }

        public IndexModel(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var contacts = await _contactRepository.GetAllContactsAsync();
            
            Contacts = contacts.Select(c => new ContactViewModel
            {
                Id = c.Id,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                Type = c.Type,
                IsOptedIn = c.IsOptedIn
            }).ToList();

            StatusMessage = TempData["StatusMessage"] as string;
            IsSuccess = TempData["IsSuccess"] as bool? ?? false;

            return Page();
        }
    }
}