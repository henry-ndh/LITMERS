using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFU.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailAffiliateAsync(string email, string subject, string message);
    }
}
