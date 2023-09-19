using demoUser.Services.Interfaces;
using MimeKit;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoUser.Services.Services
{
    public class MailService : IMailService
    {
        public bool SendMail(string EmailTo, string EmailToName, string EmailSubject, string EmailBody)
        {
            try
            {
                //using (MimeMessage emailMessage = new MimeMessage())
                //{
                //    MailboxAddress emailFrom = new MailboxAddress(, _mailSettings.SenderEmail);
                //    emailMessage.From.Add(emailFrom);
                //    MailboxAddress emailTo = new MailboxAddress(EmailToName, EmailTo);
                //    emailMessage.To.Add(emailTo);
                //}
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
