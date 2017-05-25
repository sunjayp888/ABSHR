using HR.Business.EmailServiceReference;
using System;
using System.Collections.Generic;

namespace HR.Business
{
    public class EmailService : Interfaces.IEmailService
    {
        private readonly string[] _overrideEmailAddresses;

        public EmailService(string overrideEmailAddresses)
        {
            if (!string.IsNullOrEmpty(overrideEmailAddresses))
            {
                _overrideEmailAddresses = overrideEmailAddresses.Split(';');
            }
        }

        public void SendEmail(EmailData data)
        {
            UseOverrideEmailDataIfSet(data);

            var client = new EmailServiceClient();
            client.SendEmail(data);
        }

        public void SendEmail(EmailData data, List<Guid> docGuids)
        {
            UseOverrideEmailDataIfSet(data);

            var client = new EmailServiceClient();
            client.SendEmailWithAttachments(data, docGuids);
        }

        public void SendEmail(EmailData data, Dictionary<string, byte[]> attachments)
        {
            UseOverrideEmailDataIfSet(data);

            var client = new EmailServiceClient();
            client.SendEmailWithSimpleAttachments(data, attachments);
        }
        
        private void UseOverrideEmailDataIfSet(EmailData data)
        {
            if (_overrideEmailAddresses != null)
            {
                data.ToAddressList.Clear();
                data.ToAddressList.AddRange(_overrideEmailAddresses);
                if (data.CCAddressList != null)
                    data.CCAddressList.Clear();
                if (data.BCCAddressList != null)
                    data.BCCAddressList.Clear();
            }
        }
    }
}
