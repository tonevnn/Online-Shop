/*using AutoMapper.Internal;
using FinalProject.Utils.Mail;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class MailController : Controller
    {
        private readonly IMailService mailService;

        public MailController(IMailService mailService)
        {
            this.mailService = mailService;
        }
        }
        [HttpPost("welcome")]
        public async Task<IActionResult> SendWelcomeMail([FromForm] WelcomeRequest request)
        {
            try
            {
                await mailService.SendWelcomeEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
*/