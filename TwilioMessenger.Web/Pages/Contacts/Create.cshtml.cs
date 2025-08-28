using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TwilioMessenger.Core.Models;
using TwilioMessenger.Core.Services;
using TwilioMessenger.Web.Models;

namespace TwilioMessenger.Web.Pages.Contacts
{
    public class CreateModel : PageModel
    {
        private readonly IContactRepository _contactRepository;
        private readonly ILogger<CreateModel> _logger;

        [BindProperty]
        public ContactViewModel Contact { get; set; } = new();

        public CreateModel(IContactRepository contactRepository, ILogger<CreateModel> logger)
        {
            _contactRepository = contactRepository;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingContact = await _contactRepository.GetContactByPhoneNumberAsync(Contact.PhoneNumber);
            if (existingContact != null)
            {
                ModelState.AddModelError(string.Empty, "A contact with this phone number already exists.");
                return Page();
            }

            var contact = new Contact
            {
                Name = Contact.Name,
                PhoneNumber = Contact.PhoneNumber,
                Type = Contact.Type,
                IsOptedIn = Contact.IsOptedIn
            };

            try
            {
                await _contactRepository.AddContactAsync(contact);
                await _contactRepository.SaveChangesAsync();
                
                TempData["StatusMessage"] = "Contact created successfully.";
                TempData["IsSuccess"] = true;
                
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the contact. Please try again.");
                return Page();
            }
        }
    }
}