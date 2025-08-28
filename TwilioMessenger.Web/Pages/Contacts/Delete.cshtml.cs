using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TwilioMessenger.Core.Services;
using TwilioMessenger.Web.Models;

namespace TwilioMessenger.Web.Pages.Contacts
{
    public class DeleteModel : PageModel
    {
        private readonly IContactRepository _contactRepository;
        private readonly ILogger<DeleteModel> _logger;

        [BindProperty]
        public ContactViewModel Contact { get; set; } = new();

        public DeleteModel(IContactRepository contactRepository, ILogger<DeleteModel> logger)
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
            if (string.IsNullOrEmpty(Contact.Id))
            {
                return NotFound();
            }

            var contact = await _contactRepository.GetContactByIdAsync(Contact.Id);

            if (contact == null)
            {
                return NotFound();
            }

            try
            {
                await _contactRepository.DeleteContactAsync(Contact.Id);
                await _contactRepository.SaveChangesAsync();
                
                TempData["StatusMessage"] = "Contact deleted successfully.";
                TempData["IsSuccess"] = true;
                
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contact {ContactId}", Contact.Id);
                TempData["StatusMessage"] = "An error occurred while deleting the contact. Please try again.";
                TempData["IsSuccess"] = false;
                return RedirectToPage("./Index");
            }
        }
    }
}