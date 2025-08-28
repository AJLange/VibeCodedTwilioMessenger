using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TwilioMessenger.Core.Models;
using TwilioMessenger.Core.Services;

namespace TwilioMessenger.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IContactRepository _contactRepository;

    public int TotalContacts { get; private set; }
    public int SmsContacts { get; private set; }
    public int WhatsAppContacts { get; private set; }

    public IndexModel(ILogger<IndexModel> logger, IContactRepository contactRepository)
    {
        _logger = logger;
        _contactRepository = contactRepository;
    }

    public async Task OnGetAsync()
    {
        var contacts = await _contactRepository.GetAllContactsAsync();
        TotalContacts = contacts.Count();
        SmsContacts = contacts.Count(c => c.Type == ContactType.SMS);
        WhatsAppContacts = contacts.Count(c => c.Type == ContactType.WhatsApp);
    }
}
