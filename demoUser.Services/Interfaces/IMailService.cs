using demoUser.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoUser.Services.Interfaces
{
    public interface IMailService
    {
        bool sendMail(MailSettingsDTO settingsDTO, MailData mailData);
    }
}
