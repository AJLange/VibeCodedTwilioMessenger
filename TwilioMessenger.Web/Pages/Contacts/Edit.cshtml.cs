using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TwilioMessenger.Core.Models;
using TwilioMessenger.Core.Services;
using TwilioMessenger.Web.Models;

namespace TwilioMessenger.Web.Pages.Contacts
{
    public class EditModel : PageModel
    {
        private readonly IContactRepository _contactRepository;
        private readonly ILogger<EditModel> _logger;

        [BindProperty]
        public ContactViewModel Contact { get; set; } = new();

        public EditModel(IContactRepository contactRepository, ILogger<EditModel> logger)
        {
            _contactRepository = contactRepository;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var contact = await _contactRepository.GetContactByIdAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            Contact = new ContactViewModel
            {
                Id = contact.Id,
                Name = contact.Name,
                PhoneNumber = contact.PhoneNumber,
                Type = contact.Type,
                IsOptedIn = contact.IsOptedIn
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var contact = await _contactRepository.GetContactByIdAsync(Contact.Id ?? string.Empty);

            if (contact == null)
            {
                return NotFound();
            }

            // Check if phone number is being changed and if it already exists
            if (contact.PhoneNumber != Contact.PhoneNumber)
            {
                var existingContact = await _contactRepository.GetContactByPhoneNumberAsync(Contact.PhoneNumber);
                if (existingContact != null && existingContact.Id != Contact.Id)
                {
                    ModelState.AddModelError(string.Empty, "A contact with this phone number already exists.");
                    return Page();
                }
            }

            contact.Name = Contact.Name;
            contact.PhoneNumber = Contact.PhoneNumber;
            contact.Type = Contact.Type;
            contact.IsOptedIn = Contact.IsOptedIn;

            try
            {
                await _contactRepository.UpdateContactAsync(contact);
                await _contactRepository.SaveChangesAsync();
                
                TempData["StatusMessage"] = "Contact updated successfully.";
                TempData["IsSuccess"] = true;
                
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact {ContactId}", Contact.Id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the contact. Please try again.");
                return Page();
            }
        }
    }
}