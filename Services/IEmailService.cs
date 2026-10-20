using Leasing.Models;

namespace Leasing.Services
{
    public interface IEmailService
    {
        Task<bool> SendInquiryEmailAsync(EmailViewModel model);
    }
}