using System;
using System.Collections.Generic;
using SharedTypes.DataContracts;

namespace HR.Business.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(EmailData data);
        void SendEmail(EmailData data, Dictionary<string, byte[]> attachments);
    }
}
