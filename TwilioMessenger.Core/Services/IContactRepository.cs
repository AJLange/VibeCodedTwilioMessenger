using System.Collections.Generic;
using System.Threading.Tasks;
using TwilioMessenger.Core.Models;

namespace TwilioMessenger.Core.Services
{
    public interface IContactRepository
    {
        Task<IEnumerable<Contact>> GetAllContactsAsync();
        Task<IEnumerable<Contact>> GetContactsByTypeAsync(ContactType type);
        Task<IEnumerable<Contact>> GetOptedInContactsAsync();
        Task<Contact?> GetContactByIdAsync(string id);
        Task<Contact?> GetContactByPhoneNumberAsync(string phoneNumber);
        Task AddContactAsync(Contact contact);
        Task UpdateContactAsync(Contact contact);
        Task DeleteContactAsync(string id);
        Task SaveChangesAsync();
    }
}