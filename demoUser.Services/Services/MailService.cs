using demoUser.Infrastructure.DTO;
using demoUser.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoUser.Services.Services
{
    public class MailService : IMailService
    {
        public bool sendMail(MailSettingsDTO settingsDTO, MailData mailData)
        {
			try
			{
                var emailSender = new MimeMessage();
                emailSender.From.Add(MailboxAddress.Parse(settingsDTO.SenderEmail));
                emailSender.To.Add(MailboxAddress.Parse(mailData.To));
                emailSender.Subject = mailData.Subject;
                //emailSender.Body = new TextPart(TextFormat.Html) { Text = htmlTemplate };

                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.HtmlBody = mailData.Body;

                emailSender.Body = emailBodyBuilder.ToMessageBody();

                using (SmtpClient mailClient = new SmtpClient())
                {
                    mailClient.Connect(settingsDTO.Host, settingsDTO.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    mailClient.Authenticate(settingsDTO.UserName, settingsDTO.Password);
                    mailClient.Send(emailSender);
                    mailClient.Disconnect(true);
                }
                return true;
			}
			catch
			{
                return false;
			}
        }
    }
}
