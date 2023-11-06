using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Utils.Mail
{
public interface IMailService
{
    Task SendWelcomeEmailAsync(WelcomeRequest request);
}
}
