using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using TwilioMessenger.Core.Models;

namespace TwilioMessenger.Core.Services
{
    public class TwilioMessagingService : ITwilioMessagingService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhone;
        private readonly string _whatsAppFromPhone;
        private readonly IContactRepository _contactRepository;

        public TwilioMessagingService(
            string accountSid, 
            string authToken, 
            string fromPhone, 
            string whatsAppFromPhone,
            IContactRepository contactRepository)
        {
            _accountSid = accountSid;
            _authToken = authToken;
            _fromPhone = fromPhone;
            _whatsAppFromPhone = whatsAppFromPhone;
            _contactRepository = contactRepository;

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task<MessageResult> SendSmsAsync(string to, string message)
        {
            try
            {
                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(_fromPhone),
                    to: new PhoneNumber(to)
                );

                return new MessageResult
                {
                    Success = true,
                    MessageId = messageResource.Sid,
                    To = to,
                    SuccessCount = 1
                };
            }
            catch (Exception ex)
            {
                return new MessageResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    To = to,
                    FailureCount = 1
                };
            }
        }

        public async Task<MessageResult> SendWhatsAppAsync(string to, string message)
        {
            try
            {
                // Format for WhatsApp is "whatsapp:+1234567890"
                var whatsAppTo = to.StartsWith("whatsapp:") ? to : $"whatsapp:{to}";
                var whatsAppFrom = _whatsAppFromPhone.StartsWith("whatsapp:") ? 
                    _whatsAppFromPhone : $"whatsapp:{_whatsAppFromPhone}";

                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(whatsAppFrom),
                    to: new PhoneNumber(whatsAppTo)
                );

                return new MessageResult
                {
                    Success = true,
                    MessageId = messageResource.Sid,
                    To = to,
                    SuccessCount = 1
                };
            }
            catch (Exception ex)
            {
                return new MessageResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    To = to,
                    FailureCount = 1
                };
            }
        }

        public async Task<MessageResult> SendBulkSmsAsync(string[] to, string message)
        {
            var result = new MessageResult();
            
            foreach (var number in to)
            {
                var singleResult = await SendSmsAsync(number, message);
                
                if (singleResult.Success)
                {
                    result.SuccessCount++;
                }
                else
                {
                    result.FailureCount++;
                    result.FailedNumbers.Add(number);
                }
            }

            result.Success = result.SuccessCount > 0;
            return result;
        }

        public async Task<MessageResult> SendBulkWhatsAppAsync(string[] to, string message)
        {
            var result = new MessageResult();
            
            foreach (var number in to)
            {
                var singleResult = await SendWhatsAppAsync(number, message);
                
                if (singleResult.Success)
                {
                    result.SuccessCount++;
                }
                else
                {
                    result.FailureCount++;
                    result.FailedNumbers.Add(number);
                }
            }

            result.Success = result.SuccessCount > 0;
            return result;
        }

        public async Task<MessageResult> SendToAllContactsAsync(string message)
        {
            var contacts = await _contactRepository.GetOptedInContactsAsync();
            var result = new MessageResult();

            foreach (var contact in contacts)
            {
                var singleResult = contact.Type == ContactType.SMS
                    ? await SendSmsAsync(contact.PhoneNumber, message)
                    : await SendWhatsAppAsync(contact.PhoneNumber, message);

                if (singleResult.Success)
                {
                    result.SuccessCount++;
                }
                else
                {
                    result.FailureCount++;
                    result.FailedNumbers.Add(contact.PhoneNumber);
                }
            }

            result.Success = result.SuccessCount > 0;
            return result;
        }

        public async Task<MessageResult> SendToContactTypeAsync(ContactType contactType, string message)
        {
            var contacts = (await _contactRepository.GetOptedInContactsAsync())
                .Where(c => c.Type == contactType)
                .ToList();
            
            var result = new MessageResult();

            foreach (var contact in contacts)
            {
                var singleResult = contactType == ContactType.SMS
                    ? await SendSmsAsync(contact.PhoneNumber, message)
                    : await SendWhatsAppAsync(contact.PhoneNumber, message);

                if (singleResult.Success)
                {
                    result.SuccessCount++;
                }
                else
                {
                    result.FailureCount++;
                    result.FailedNumbers.Add(contact.PhoneNumber);
                }
            }

            result.Success = result.SuccessCount > 0;
            return result;
        }
    }
}