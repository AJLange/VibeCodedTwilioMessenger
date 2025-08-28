using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TwilioMessenger.Core.Models;

namespace TwilioMessenger.Core.Services
{
    public class FileContactRepository : IContactRepository
    {
        private readonly string _filePath;
        private List<Contact> _contacts = new();
        private bool _isLoaded = false;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public FileContactRepository(string filePath)
        {
            _filePath = filePath;
        }

        private async Task LoadContactsAsync()
        {
            if (_isLoaded)
                return;

            if (!File.Exists(_filePath))
            {
                _contacts = new List<Contact>();
                await SaveChangesAsync();
            }
            else
            {
                try
                {
                    var json = await File.ReadAllTextAsync(_filePath);
                    _contacts = JsonSerializer.Deserialize<List<Contact>>(json, _jsonOptions) ?? new List<Contact>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading contacts: {ex.Message}");
                    _contacts = new List<Contact>();
                }
            }

            _isLoaded = true;
        }

        public async Task<IEnumerable<Contact>> GetAllContactsAsync()
        {
            await LoadContactsAsync();
            return _contacts.ToList();
        }

        public async Task<IEnumerable<Contact>> GetContactsByTypeAsync(ContactType type)
        {
            await LoadContactsAsync();
            return _contacts.Where(c => c.Type == type).ToList();
        }

        public async Task<IEnumerable<Contact>> GetOptedInContactsAsync()
        {
            await LoadContactsAsync();
            return _contacts.Where(c => c.IsOptedIn).ToList();
        }

        public async Task<Contact?> GetContactByIdAsync(string id)
        {
            await LoadContactsAsync();
            return _contacts.FirstOrDefault(c => c.Id == id);
        }

        public async Task<Contact?> GetContactByPhoneNumberAsync(string phoneNumber)
        {
            await LoadContactsAsync();
            return _contacts.FirstOrDefault(c => c.PhoneNumber == phoneNumber);
        }

        public async Task AddContactAsync(Contact contact)
        {
            await LoadContactsAsync();
            
            if (string.IsNullOrEmpty(contact.Id))
            {
                contact.Id = Guid.NewGuid().ToString();
            }
            
            _contacts.Add(contact);
        }

        public async Task UpdateContactAsync(Contact contact)
        {
            await LoadContactsAsync();
            
            var existingContact = _contacts.FirstOrDefault(c => c.Id == contact.Id);
            if (existingContact == null)
                throw new KeyNotFoundException($"Contact with ID {contact.Id} not found.");

            var index = _contacts.IndexOf(existingContact);
            _contacts[index] = contact;
        }

        public async Task DeleteContactAsync(string id)
        {
            await LoadContactsAsync();
            
            var contact = _contacts.FirstOrDefault(c => c.Id == id);
            if (contact != null)
            {
                _contacts.Remove(contact);
            }
        }

        public async Task SaveChangesAsync()
        {
            await LoadContactsAsync();
            
            var directoryPath = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            var json = JsonSerializer.Serialize(_contacts, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}